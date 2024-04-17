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

            //diseaseProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocodiseaseprojectile");

            diseaseProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocodiseaseprojectile").InstantiateClone("RiskyMod_CrocoDiseaseProjectile", true);
            diseaseProjectile = ModifyDiseaseProjectile(diseaseProjectile, Epidemic);
            Content.Content.projectilePrefabs.Add(diseaseProjectile);

            if (SoftDependencies.ScepterPluginLoaded)
            {
                diseaseScepterProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocodiseaseprojectile").InstantiateClone("RiskyMod_CrocoDiseaseScepterProjectile", true);
                diseaseScepterProjectile = ModifyDiseaseProjectile(diseaseScepterProjectile, EpidemicScepter);
                Content.Content.projectilePrefabs.Add(diseaseScepterProjectile);
                EntityStates.RiskyModStates.Croco.FireDiseaseProjectileScepter.projectilePrefab = diseaseScepterProjectile;
            }

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.FireDiseaseProjectile", "projectilePrefab", diseaseProjectile);

            IL.EntityStates.Croco.FireSpit.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchCallvirt<ProjectileManager>("FireProjectile")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<FireProjectileInfo, EntityStates.Croco.FireSpit, FireProjectileInfo>>((projectileInfo, self) =>
                    {
                        if (self.projectilePrefab == diseaseProjectile)
                        {
                            projectileInfo.damageTypeOverride = default;
                        }
                        return projectileInfo;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Croco ModifySpecial IL Hook failed");
                }
            };

            SetupEpidemicVFX();
        }

        private GameObject ModifyDiseaseProjectile(GameObject go, DamageAPI.ModdedDamageType dt)
        {
            DamageAPI.ModdedDamageTypeHolderComponent mdc = go.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(dt);
            ProjectileProximityBeamController pbc = go.GetComponent<ProjectileProximityBeamController>();
            pbc.attackRange = CrocoCore.Cfg.Skills.Epidemic.spreadRange * (dt == EpidemicScepter ? 1.5f : 1f);
            return go;
        }

        private void SetupDamageType()
        {
            Epidemic = DamageAPI.ReserveDamageType();
            EpidemicScepter = DamageAPI.ReserveDamageType();
            OnHitEnemy.OnHitAttackerActions += ApplyEpidemic;

            EpidemicDebuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_EpidemicDebuff",
                true,
                false,
                true,
                new Color(243f / 255f, 202f / 255f, 107f / 255f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Entangle").iconSprite
                );

            RecalculateStatsAPI.GetStatCoefficients += (CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args) =>
            {
                if (sender.HasBuff(EpidemicDebuff))
                {
                    args.moveSpeedReductionMultAdd += 0.5f;
                }
            };
        }

        private void SetupEpidemicVFX()
        {
            On.RoR2.CharacterBody.OnClientBuffsChanged += (orig, self) =>
            {
                orig(self);
                if (self.HasBuff(EpidemicDebuff.buffIndex))
                {
                    if (!self.GetComponent<EpidemicVFXController>())
                    {
                        self.gameObject.AddComponent<EpidemicVFXController>();
                    }
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
                EpidemicDamageController ec = victimBody.gameObject.AddComponent<EpidemicDamageController>();
                ec.Setup(attackerBody, victimBody, damageInfo, isScepter);

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
}
