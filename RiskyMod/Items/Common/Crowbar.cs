using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Common
{
    public class Crowbar
    {
        public static bool enabled = true;
        public static DamageAPI.ModdedDamageType IgnoreCrowbar;
        public static CrowbarManager crowbarManager;
        public static float damageCoefficient = 0.45f;
        public static ModdedProcType CrowbarProc;

        public Crowbar()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

            IgnoreCrowbar = DamageAPI.ReserveDamageType();
            CrowbarProc = ProcTypeAPI.ReserveProcType();

            //Remove vanilla effect
            IL.RoR2.HealthComponent.TakeDamageProcess += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Crowbar")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Crowbar TakeDamage IL Hook failed");
                }
            };

            //Effect handled in OnCharacterDeath (for removal)
            RoR2.Run.onRunStartGlobal += InitCrowbarManager;

            RoR2.Stage.onStageStartGlobal += ClearCrowbarList;

            //Modify Ring Threshold
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "ElementalRingsReady"),
                     x => x.MatchCallvirt<CharacterBody>("HasBuff")
                    )
                &&
                c.TryGotoPrev(MoveType.After,
                    x => x.MatchLdcR4(4f)
                    ))
                {
                    c.Emit(OpCodes.Ldarg_1);//damageinfo
                    c.Emit(OpCodes.Ldloc_1);//attacker body
                    c.EmitDelegate<Func<float, DamageInfo, CharacterBody, float>>((ringThreshold, damageInfo, attackerBody) =>
                    {
                        if (Crowbar.enabled && damageInfo.procChainMask.HasModdedProc(CrowbarProc) && damageInfo.attacker && attackerBody)
                        {
                            Inventory inv = attackerBody.inventory;
                            if (inv)
                            {
                                int crowbarCount = inv.GetItemCount(RoR2Content.Items.Crowbar);
                                if (crowbarCount > 0)
                                {
                                    ringThreshold *= Crowbar.GetCrowbarMult(crowbarCount);
                                }
                            }
                        }

                        if (damageInfo.damageType.damageType.HasFlag(DamageType.DoT)) ringThreshold = Mathf.Infinity;
                        return ringThreshold;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Crowbar OnHitEnemy IL Hook failed");
                }
            };

            TakeDamage.ModifyInitialDamageInventoryActions += CrowbarDamageBoost;
        }

        private void InitCrowbarManager(Run self)
        {
            if (!NetworkServer.active) return;
            Crowbar.crowbarManager = self.gameObject.GetComponent<CrowbarManager>();
            if (!Crowbar.crowbarManager) Crowbar.crowbarManager = self.gameObject.AddComponent<CrowbarManager>();
        }

        private void ClearCrowbarList(Stage obj)
        {
            if (NetworkServer.active)
            {
                if (Crowbar.crowbarManager)
                {
                    Crowbar.crowbarManager.ClearList();
                }
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Crowbar);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Crowbar);
        }

        public static float GetCrowbarMult(int crowbarCount)
        {
            return 1f + damageCoefficient * crowbarCount;
        }

        private static void CrowbarDamageBoost(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory attackerInventory)
        {
            int crowbarCount = attackerInventory.GetItemCount(RoR2Content.Items.Crowbar);
            if (crowbarCount > 0)
            {
                if (self.body != attackerBody
                    && damageInfo.procCoefficient > 0f
                    && damageInfo.damage > 0f
                    && !damageInfo.HasModdedDamageType(Crowbar.IgnoreCrowbar)
                    && !damageInfo.procChainMask.HasModdedProc(CrowbarProc))
                {
                    if (Crowbar.crowbarManager.CanApplyCrowbar(self, attackerBody))
                    {
                        damageInfo.damage *= GetCrowbarMult(crowbarCount);
                        EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.crowbarImpactEffectPrefab, damageInfo.position, -damageInfo.force, true);
                        damageInfo.procChainMask.AddModdedProc(CrowbarProc);
                    }
                }
            }
        }
    }

    public class CrowbarManager : MonoBehaviour
    {
        private class CrowbarTarget
        {
            public HealthComponent victim;
            public HashSet<CharacterBody> attackers;
            public CrowbarTarget(HealthComponent vict, CharacterBody attacker = null)
            {
                victim = vict;
                attackers = new HashSet<CharacterBody>();
                if (attacker)
                {
                    attackers.Add(attacker);
                }
            }
        }
        private List<CrowbarTarget> targetList;

        public void FixedUpdate()
        {
            if (targetList.Count > 0)
            {
                List<CrowbarTarget> toRemove = new List<CrowbarTarget>();
                foreach (CrowbarTarget ct in targetList)
                {
                    //Don't count Barrier here
                    if ((ct.victim.health + ct.victim.shield) >= ct.victim.fullCombinedHealth)
                    {
                        toRemove.Add(ct);
                    }
                }
                foreach(CrowbarTarget ct in toRemove)
                {
                    targetList.Remove(ct);
                }
            }
        }

        public void Awake()
        {
            targetList = new List<CrowbarTarget>();
        }

        public void Remove(HealthComponent victim)
        {
            CrowbarTarget toRemove = null;
            foreach (CrowbarTarget ct in targetList)
            {
                if (ct.victim == victim)
                {
                    toRemove = ct;
                    break;
                }
            }

            targetList.Remove(toRemove);
        }

        public void ClearList()
        {
            targetList.Clear();
        }

        public bool CanApplyCrowbar(HealthComponent victim, CharacterBody attackerBody)
        {
            bool applyCrowbar = true;

            bool foundVictim = false;
            foreach (CrowbarTarget ct in targetList)
            {
                if (ct.victim == victim)
                {
                    foundVictim = true;
                    if (ct.attackers.Contains(attackerBody))
                    {
                        applyCrowbar = false;
                    }
                    else
                    {
                        ct.attackers.Add(attackerBody);
                    }
                    break;
                }
            }

            if (!foundVictim)
            {
                targetList.Add(new CrowbarTarget(victim, attackerBody));
            }

            return applyCrowbar;
        }
    }
}
