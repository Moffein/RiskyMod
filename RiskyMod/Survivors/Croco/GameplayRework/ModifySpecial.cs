using System;
using UnityEngine;
using RoR2;
using R2API;
using MonoMod.Cil;
using RoR2.Projectile;
using Mono.Cecil.Cil;
using RiskyMod.SharedHooks;
using UnityEngine.Networking;
using RoR2.Orbs;
using RoR2.Stats;

namespace RiskyMod.Survivors.Croco
{
    public class ModifySpecial
    {
        private static GameObject diseaseProjectile;
        private static GameObject diseaseScepterProjectile;

        public static DamageAPI.ModdedDamageType Epidemic;
        public static DamageAPI.ModdedDamageType EpidemicScepter;
        public static BuffDef EpidemicDebuff;

        public ModifySpecial()
        {
            SetupDamageType();

            //diseaseProjectile = Resources.Load<GameObject>("prefabs/projectiles/crocodiseaseprojectile");

            diseaseProjectile = Resources.Load<GameObject>("prefabs/projectiles/crocodiseaseprojectile").InstantiateClone("RiskyMod_CrocoDiseaseProjectile", true);
            DamageAPI.ModdedDamageTypeHolderComponent mdc = diseaseProjectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(Epidemic);
            ProjectileAPI.Add(diseaseProjectile);

            diseaseScepterProjectile = Resources.Load<GameObject>("prefabs/projectiles/crocodiseaseprojectile").InstantiateClone("RiskyMod_CrocoDiseaseScepterProjectile", true);
            DamageAPI.ModdedDamageTypeHolderComponent mdc2 = diseaseScepterProjectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc2.Add(EpidemicScepter);
            ProjectileAPI.Add(diseaseScepterProjectile);
            EntityStates.RiskyMod.Croco.FireDiseaseProjectileScepter.projectilePrefab = diseaseScepterProjectile;

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.FireDiseaseProjectile", "projectilePrefab", diseaseProjectile);

            IL.EntityStates.Croco.FireSpit.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<ProjectileManager>("FireProjectile")
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<FireProjectileInfo, EntityStates.Croco.FireSpit, FireProjectileInfo>>((projectileInfo, self) =>
                {
                    if (self.projectilePrefab == diseaseProjectile)
                    {
                        projectileInfo.damageTypeOverride = default;
                    }
                    return projectileInfo;
                });
            };

            //Remove this when DamageAPI updates.
            IL.RoR2.Orbs.LightningOrb.OnArrival += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCallvirt<RoR2.Orbs.OrbManager>("AddOrb")
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<LightningOrb, LightningOrb, LightningOrb>>((newOrb, self) =>
                {
                    if (self.HasModdedDamageType(Epidemic))
                    {
                        newOrb.AddModdedDamageType(Epidemic);
                    }
                    if (self.HasModdedDamageType(EpidemicScepter))
                    {
                        newOrb.AddModdedDamageType(EpidemicScepter);
                    }
                    return newOrb;
                });
            };
        }

        private void SetupDamageType()
        {
            Epidemic = DamageAPI.ReserveDamageType();
            EpidemicScepter = DamageAPI.ReserveDamageType();
            OnHitEnemy.OnHitAttackerActions += ApplyEpidemic;

            EpidemicDebuff = ScriptableObject.CreateInstance<BuffDef>();
            EpidemicDebuff.buffColor = new Color(243f/255f, 202f/255f, 107f/255f);
            EpidemicDebuff.canStack = false;
            EpidemicDebuff.isDebuff = true;
            EpidemicDebuff.name = "RiskyMod_EpidemicDebuff";
            EpidemicDebuff.iconSprite = RoR2Content.Buffs.Entangle.iconSprite;
            BuffAPI.Add(new CustomBuff(EpidemicDebuff));

            SharedHooks.GetStatsCoefficient.HandleStatsActions += (CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) =>
            {
                if (sender.HasBuff(EpidemicDebuff))
                {
                    args.moveSpeedReductionMultAdd += 0.5f;
                }
            };
        }

        private static void ApplyEpidemic(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            bool isScepter = damageInfo.HasModdedDamageType(EpidemicScepter);
            bool isDisease = isScepter || damageInfo.HasModdedDamageType(Epidemic);
            if (isDisease)
            {
                //Multiple Acrids can stack Epidemic
                EpidemicComponent ec = victimBody.gameObject.AddComponent<EpidemicComponent>();
                ec.owner = attackerBody;
                ec.victim = victimBody;
                if (isScepter)
                {
                    ec.SetScepter();
                }

                //Tick poison achievement
                if (attackerBody.master)
                {
                    PlayerStatsComponent playerStatsComponent = attackerBody.master.playerStatsComponent;
                    if (playerStatsComponent != null)
                    {
                        playerStatsComponent.currentStats.PushStatValue(StatDef.totalCrocoInfectionsInflicted, 1UL);
                    }
                }
            }
        }
    }

    public class EpidemicComponent : MonoBehaviour
    {
        public static int baseTickCount = 5;    //Initial hit is 1 tick
        public static int baseTickCountScepter = 9;
        public static float timeBetweenTicks = 0.5f;
        public static float baseLingerDuration = 1.5f;
        public static float damageCoefficient = 1f;

        public static float scepterHealCoefficient = 0.15f;

        public static GameObject impactEffect = Resources.Load<GameObject>("prefabs/effects/impacteffects/crocodiseaseimpacteffect");

        public CharacterBody owner;
        public CharacterBody victim;
        private int ticksRemaining;
        private float stopwatch;
        private float lingerStopwatch;
        private bool scepter = false;
        
        private void Awake()
        {
            if (!NetworkServer.active)
            {
                Destroy(this);
                return;
            }
            stopwatch = 0f;
            lingerStopwatch = 0f;
            ticksRemaining = scepter?baseTickCountScepter:baseTickCount;
        }

        private void FixedUpdate()
        {
            if (victim && !victim.HasBuff(ModifySpecial.EpidemicDebuff.buffIndex))
            {
                victim.AddBuff(ModifySpecial.EpidemicDebuff.buffIndex);
            }

            if (ticksRemaining > 0)
            {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= timeBetweenTicks)
                {
                    stopwatch -= timeBetweenTicks;
                    ticksRemaining--;
                    if (owner && victim && victim.healthComponent)
                    {
                        DamageInfo diseaseDamage = new DamageInfo
                        {
                            attacker = owner.gameObject,
                            inflictor = owner.gameObject,
                            damage = damageCoefficient * owner.damage,
                            damageColorIndex = DamageColorIndex.Default,
                            damageType = DamageType.Generic,
                            crit = owner.RollCrit(),
                            dotIndex = DotController.DotIndex.None,
                            force = 300f * Vector3.down,
                            position = victim.corePosition,
                            procChainMask = default(ProcChainMask),
                            procCoefficient = 0.5f
                        };
                        victim.healthComponent.TakeDamage(diseaseDamage);
                        GlobalEventManager.instance.OnHitEnemy(diseaseDamage, victim.gameObject);

                        EffectManager.SimpleImpactEffect(impactEffect, victim.corePosition, Vector3.up, true);
                    }
                }
            }
            else
            {
                lingerStopwatch += Time.fixedDeltaTime;
            }

            //Let the debuff last a bit longer.
            if (lingerStopwatch >= baseLingerDuration || !(victim && victim.healthComponent && victim.healthComponent.alive))
            {
                Destroy(this);
                return;
            }
        }

        private void OnDestroy()
        {
            Debug.Log("Destroying");
            //Remove Buff
            if (victim && victim.HasBuff(ModifySpecial.EpidemicDebuff.buffIndex))
            {
                victim.RemoveBuff(ModifySpecial.EpidemicDebuff.buffIndex);
            }
            if (scepter && owner && !(victim && victim.healthComponent && victim.healthComponent.alive))
            {
                Debug.Log("Triggering Scepter");
                owner.AddTimedBuff(RoR2Content.Buffs.CrocoRegen.buffIndex, 1f);
            }
        }

        public void SetScepter()
        {
            ticksRemaining = baseTickCountScepter;
            scepter = true;
        }
    }
}
