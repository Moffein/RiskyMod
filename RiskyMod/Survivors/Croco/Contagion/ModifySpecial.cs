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
        public static DamageAPI.ModdedDamageType EpidemicDamage;
        public static BuffDef EpidemicDebuff;


        public ModifySpecial()
        {
            GameObject epidemicProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoDiseaseProjectile.prefab").WaitForCompletion();
            epidemicProjectile.AddComponent<FixEpidemicDamageSource>();

            SetupDamageType();
            SetupEpidemicVFX();
        }

        private void SetupDamageType()
        {
            EpidemicDamage = DamageAPI.ReserveDamageType();
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
            if (damageInfo.HasModdedDamageType(EpidemicDamage))
            {
                //Multiple Acrids can stack Epidemic
                EpidemicDamageController ec = victimBody.gameObject.AddComponent<EpidemicDamageController>();
                ec.Setup(attackerBody, victimBody, damageInfo);

                //Tick poison achievement
                if (attackerBody.master)
                {
                    PlayerStatsComponent playerStatsComponent = attackerBody.master.playerStatsComponent;
                    if (playerStatsComponent) playerStatsComponent.currentStats.PushStatValue(StatDef.totalCrocoInfectionsInflicted, 1UL);
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
