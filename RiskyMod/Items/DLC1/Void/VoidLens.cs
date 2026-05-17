using MonoMod.Cil;
using R2API;
using RoR2;
using SneedHooks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Items.DLC1.Void
{
    public class VoidLens
    {
        public static bool enabled = true;
        public static R2API.ModdedProcType VoidLensRepeat;
        public static GameObject effectPrefab;

        public VoidLens()
        {
            if (!enabled) return;
            VoidLensRepeat = R2API.ProcTypeAPI.ReserveProcType();
            RoR2.Run.onRunStartGlobal += Run_onRunStartGlobal;
            RoR2.Stage.onServerStageComplete += Stage_onServerStageComplete;
            GlobalEventManager.onServerDamageDealt += GlobalEventManager_onServerDamageDealt;

            SneedHooks.ModifyFinalDamage.ModifyFinalDamageAttackerActions += LowerCritDamageAttacker;

            SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += GiveCrit;
            IL.RoR2.HealthComponent.TakeDamageProcess += RemoveVanillaEffect;

            ItemsCore.ModifyItemDefActions += ModifyItem;
            BuildVfx();
        }

        private void RemoveVanillaEffect(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC1Content.Items), "CritGlassesVoid")))
            {
                c.EmitDelegate<Func<ItemDef, ItemDef>>(item => null);
            }
            else
            {
                Debug.LogError("RiskyMod: CritGlassesVoid IL Hook failed.");
            }
        }

        private void GiveCrit(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            args.critAdd += 10f * inventory.GetItemCountEffective(DLC1Content.Items.CritGlassesVoid);
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, DLC1Content.Items.CritGlassesVoid);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.CritGlassesVoid);
        }

        private void BuildVfx()
        {
            GameObject effect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/CritGlassesVoid/CritGlassesVoidExecuteEffect.prefab").WaitForCompletion().InstantiateClone("RiskyModVoidCritEffect", false);
            UnityEngine.Object.Destroy(effect.GetComponent<ShakeEmitter>());
            effect.GetComponent<EffectComponent>().soundName = "";

            var slash1 = effect.transform.GetChild(4);

            var slash2 = effect.transform.GetChild(5);

            var sparks = effect.transform.Find("OmniSparks");
            var stars = effect.transform.Find("Small Stars");
            var shards = effect.transform.Find("Shards");
            var ring = effect.transform.Find("Ring");
            var light = effect.transform.Find("Point Light");
            var pp = effect.transform.Find("PP");
            var damageNumbers = effect.transform.Find("FakeDamageNumbers");
            var softGlow = effect.transform.Find("SoftGlow");

            slash1.transform.localScale = 0.5f * Vector3.up;
            slash2.transform.localScale = 0.5f * Vector3.up;
            sparks.transform.localScale = 0.5f * Vector3.one;
            ring.transform.localScale = 0.25f * Vector3.one;
            stars.transform.localScale = 0.25f * Vector3.one;
            shards.transform.localScale = 0.25f * Vector3.one;

            UnityEngine.Object.Destroy(light.gameObject);
            UnityEngine.Object.Destroy(softGlow.gameObject);
            UnityEngine.Object.Destroy(pp.gameObject);
            UnityEngine.Object.Destroy(damageNumbers.gameObject);

            Content.Content.effectDefs.Add(new EffectDef(effect));
            effectPrefab = effect;
        }

        private void LowerCritDamageAttacker(ModifyFinalDamage.DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, HealthComponent victim, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.crit && attackerBody.inventory && attackerBody.inventory.GetItemCountEffective(DLC1Content.Items.CritGlassesVoid) > 0)
            {
                damageModifierArgs.damageMultFinal *= 0.5f;
            }
        }

        private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
        {
            if (!damageReport.damageInfo.rejected
                && damageReport.damageInfo.crit
                && damageReport.damageInfo.procCoefficient > 0f
                && damageReport.damageInfo.damage > 0f
                && !damageReport.damageInfo.procChainMask.HasModdedProc(VoidLensRepeat)
                && damageReport.attackerBody
                && damageReport.attackerBody.inventory
                && damageReport.attackerBody.inventory.GetItemCountEffective(DLC1Content.Items.CritGlassesVoid) > 0
                && damageReport.victimBody
                && damageReport.victimBody.healthComponent
                && damageReport.victimBody.healthComponent.alive
                && VoidLensDamageManager.instance)
            {
                VoidLensDamageManager.instance.EnqueueDamage(damageReport.damageInfo, damageReport.victimBody.healthComponent);
            }
        }

        private void Stage_onServerStageComplete(Stage obj)
        {
            if (VoidLensDamageManager.instance) VoidLensDamageManager.instance.ClearDamageQueue();
        }

        private void Run_onRunStartGlobal(RoR2.Run run)
        {
            if (VoidLensDamageManager.instance) UnityEngine.Object.Destroy(VoidLensDamageManager.instance);

            var voidLensManager = run.GetComponent<VoidLensDamageManager>();
            if (!voidLensManager)
            {
                VoidLensDamageManager.instance = run.gameObject.AddComponent<VoidLensDamageManager>();
            }
            else
            {
                VoidLensDamageManager.instance = voidLensManager;
            }
        }

        public class VoidLensDamageManager : MonoBehaviour
        {
            public static VoidLensDamageManager instance;

            private List<QueuedDamage> queuedDamages = new List<QueuedDamage>();
            private class QueuedDamage
            {
                public float timer;
                public DamageInfo damageInfo;
                public Vector3 origVictimPosition, direction;
                public HealthComponent victim;
            }

            private void OnDestroy()
            {
                if (instance == this) instance = null;
            }

            private void FixedUpdate()
            {
                if (!NetworkServer.active)
                {
                    return;
                }

                if (queuedDamages.Count > 0)
                {
                    bool shouldRemove = true;

                    for (int i = 0; i < queuedDamages.Count; i++)
                    {
                        var queuedDamage = queuedDamages[i];

                        queuedDamage.timer -= Time.fixedDeltaTime;
                        if (queuedDamage.timer <= 0f)
                        {
                            shouldRemove = true;

                            if (queuedDamage.victim && queuedDamage.victim.alive)
                            {
                                ProcessDamage(queuedDamage);
                            }
                        }
                    }

                    if (shouldRemove)
                    {
                        for (int i = queuedDamages.Count - 1; i >= 0; i--)
                        {
                            if (queuedDamages[i].timer <= 0f)
                            {
                                queuedDamages.RemoveAt(i);
                            }
                        }
                    }
                }
            }

            private void ProcessDamage(QueuedDamage queued)
            {
                if (!GlobalEventManager.instance || !queued.victim) return;

                //Update damage position
                if (queued.damageInfo.inflictedHurtbox)
                {
                    queued.damageInfo.position = queued.damageInfo.inflictedHurtbox.transform.position;
                }
                else
                {
                    queued.damageInfo.position = queued.victim.transform.position + (queued.damageInfo.position - queued.origVictimPosition);
                }

                if (effectPrefab)
                {
                    EffectManager.SimpleEffect(effectPrefab, queued.damageInfo.position, Util.QuaternionSafeLookRotation(queued.direction), true);
                }

                queued.victim.TakeDamage(queued.damageInfo);
                GlobalEventManager.instance.OnHitEnemy(queued.damageInfo, queued.victim.gameObject);
            }

            public void EnqueueDamage(DamageInfo damage, HealthComponent victim)
            {
                var copiedDamage = new DamageInfo
                {
                    attacker = damage.attacker,
                    crit = damage.crit,
                    canRejectForce = damage.canRejectForce,
                    damage = damage.damage,
                    damageColorIndex = DamageColorIndex.Void,
                    damageType = damage.damageType,
                    delayedDamageSecondHalf = damage.delayedDamageSecondHalf,
                    dotIndex = damage.dotIndex,
                    firstHitOfDelayedDamageSecondHalf = damage.firstHitOfDelayedDamageSecondHalf,
                    force = damage.force,
                    inflictedHurtbox = damage.inflictedHurtbox,
                    inflictor = damage.inflictor,
                    physForceFlags = damage.physForceFlags,
                    procChainMask = default,
                    position = damage.position,
                    procCoefficient = damage.procCoefficient
                };
                copiedDamage.procChainMask.AddModdedProc(VoidLensRepeat);

                queuedDamages.Add(new QueuedDamage
                {
                    victim = victim,
                    damageInfo = copiedDamage,
                    origVictimPosition = victim.transform.position,
                    timer = 0.2f
                });
            }

            public void ClearDamageQueue()
            {
                queuedDamages.Clear();
            }
        }
    }
}
