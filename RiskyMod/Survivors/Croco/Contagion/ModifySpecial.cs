using EntityStates.RiskyMod.Croco;
using RoR2.Skills;
using R2API;
using RiskyMod.SharedHooks;
using RiskyMod.Survivors.Croco.Contagion.Components;
using RoR2;
using RoR2.Projectile;
using RoR2.Stats;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using EntityStates;

namespace RiskyMod.Survivors.Croco.Contagion
{
    public class ModifySpecial
    {
        private static GameObject diseaseProjectile;
        private static GameObject diseaseScepterProjectile;

        public static DamageAPI.ModdedDamageType ScepterDamage; //Used for the heal on kill
        public static DamageAPI.ModdedDamageType EpidemicDamage;
        public static DamageAPI.ModdedDamageType EpidemicScepterDamage;
        public static BuffDef EpidemicDebuff;


        public ModifySpecial()
        {
            SetupDamageType();
            diseaseProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocodiseaseprojectile").InstantiateClone("RiskyMod_CrocoDiseaseProjectile", true);
            diseaseProjectile = ModifyDiseaseProjectile(diseaseProjectile, false);
            Content.Content.projectilePrefabs.Add(diseaseProjectile);

            if (SoftDependencies.ScepterPluginLoaded)
            {
                diseaseScepterProjectile = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/crocodiseaseprojectile").InstantiateClone("RiskyMod_CrocoDiseaseScepterProjectile", true);
                diseaseScepterProjectile = ModifyDiseaseProjectile(diseaseScepterProjectile, true);
                var mdc = diseaseScepterProjectile.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                if (!mdc) mdc = diseaseScepterProjectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                mdc.Add(ScepterDamage);
                Content.Content.projectilePrefabs.Add(diseaseScepterProjectile);
                EntityStates.RiskyMod.Croco.FireDiseaseProjectileScepter.projectilePrefab = diseaseScepterProjectile;
            }
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.FireDiseaseProjectile", "projectilePrefab", diseaseProjectile);
            SetupEpidemicVFX();
        }

        private GameObject ModifyDiseaseProjectile(GameObject go, bool isScepter)
        {
            ProjectileProximityBeamController pbc = go.GetComponent<ProjectileProximityBeamController>();
            var damageOverride = go.AddComponent<EpidemicDamageOverrideComponent>();
            damageOverride.damageType = EpidemicDamage;
            if (isScepter)
            {
                damageOverride.damageType = EpidemicScepterDamage;
                pbc.attackRange *= 1.5f;
            }
            return go;
        }

        private void SetupDamageType()
        {
            EpidemicDamage = DamageAPI.ReserveDamageType();
            EpidemicScepterDamage = DamageAPI.ReserveDamageType();
            ScepterDamage = DamageAPI.ReserveDamageType();
            OnHitEnemy.OnHitAttackerActions += ApplyEpidemic;

            EpidemicDebuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_EpidemicDebuff",
                true,
                false,
                true,
                new Color(243f / 255f, 202f / 255f, 107f / 255f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Entangle").iconSprite
                );
        }

        private static void ApplyEpidemic(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            bool isScepter = damageInfo.HasModdedDamageType(EpidemicScepterDamage);
            bool isDisease = isScepter || damageInfo.HasModdedDamageType(EpidemicDamage);
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
    }
}
