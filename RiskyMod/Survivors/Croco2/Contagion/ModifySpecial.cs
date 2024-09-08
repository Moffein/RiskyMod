using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.Projectile;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Croco2.Contagion
{
    public class ModifySpecial
    {
        private static GameObject diseaseProjectile;
        private static GameObject diseaseScepterProjectile;

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

    public class EpidemicDamageOverrideComponent : MonoBehaviour
    {
        public DamageAPI.ModdedDamageType damageType;
        private void Start()
        {
            if (!NetworkServer.active) return;
            ProjectileController pc = base.GetComponent<ProjectileController>();
            if (!pc || !pc.owner) return;
            CharacterBody ownerBody = pc.owner.GetComponent<CharacterBody>();
            if (!ownerBody || !ownerBody.skillLocator) return;
            if (!ContagionPassive.HasPassive(ownerBody.skillLocator)) return;
            ProjectileDamage pd = base.GetComponent<ProjectileDamage>();
            if (pd) pd.damageType = DamageType.Generic;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = base.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            if (!mdc) mdc = base.gameObject.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(damageType);
        }
    }
}
