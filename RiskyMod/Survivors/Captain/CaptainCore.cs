using EntitiyStates.RiskyMod.Captain;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;

namespace RiskyMod.Survivors.Captain
{
    public class CaptainCore
    {
        public static bool enabled = true;
        public static bool enablePrimarySkillChanges = true;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CaptainBody");

        public static bool modifyTaser = true;
        public static bool nukeBuff = true;

        public CaptainCore()
        {
            if (!enabled) return;
            new Microbots();
            new CaptainOrbitalHiddenRealms();
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());

            On.RoR2.EntityStateCatalog.InitializeStateFields += (orig, self) =>
            {
                orig(self);
                ChargeShotgun.chargeupVfxPrefab = EntityStates.Captain.Weapon.ChargeCaptainShotgun.chargeupVfxPrefab;
                ChargeShotgun.holdChargeVfxPrefab = EntityStates.Captain.Weapon.ChargeCaptainShotgun.holdChargeVfxPrefab;
            };
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            if (!enablePrimarySkillChanges) return;

            Content.Content.entityStates.Add(typeof(ChargeShotgun));
            Content.Content.entityStates.Add(typeof(FireShotgun));

            SkillDef shotgunDef = SkillDef.CreateInstance<SkillDef>();
            shotgunDef.activationState = new SerializableEntityStateType(typeof(ChargeShotgun));
            shotgunDef.activationStateMachineName = "Weapon";
            shotgunDef.baseMaxStock = 1;
            shotgunDef.baseRechargeInterval = 0f;
            shotgunDef.beginSkillCooldownOnSkillEnd = false;
            shotgunDef.canceledFromSprinting = false;
            shotgunDef.dontAllowPastMaxStocks = true;
            shotgunDef.forceSprintDuringState = false;
            shotgunDef.fullRestockOnAssign = true;
            shotgunDef.icon = sk.primary._skillFamily.variants[0].skillDef.icon;
            shotgunDef.interruptPriority = InterruptPriority.Any;
            shotgunDef.isCombatSkill = true;
            shotgunDef.keywordTokens = new string[] { };
            shotgunDef.mustKeyPress = false;
            shotgunDef.cancelSprintingOnActivation = true;
            shotgunDef.rechargeStock = 1;
            shotgunDef.requiredStock = 1;
            shotgunDef.skillName = "ChargeShotgun";
            shotgunDef.skillNameToken = "CAPTAIN_PRIMARY_NAME";
            shotgunDef.skillDescriptionToken = "CAPTAIN_PRIMARY_DESC_RISKYMOD";
            shotgunDef.stockToConsume = 1;
            SneedUtils.SneedUtils.FixSkillName(shotgunDef);
            Content.Content.skillDefs.Add(shotgunDef);
            Skills.Shotgun = shotgunDef;
            sk.primary._skillFamily.variants[0].skillDef = Skills.Shotgun;
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (modifyTaser)
            {
                GameObject taserPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/CaptainTazer").InstantiateClone("RiskyModCaptainTazer", true);
                DamageAPI.ModdedDamageTypeHolderComponent mdc = taserPrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
                mdc.Add(SharedDamageTypes.Slow50For5s);

                ProjectileDamage pd = taserPrefab.GetComponent<ProjectileDamage>();
                pd.damageType = DamageType.SlowOnHit | DamageType.Shock5s;  //Redundant slowonhit so that death mark triggers cleanly

                ProjectileImpactExplosion pie = taserPrefab.GetComponent<ProjectileImpactExplosion>();
                pie.blastRadius = 8f;

                Content.Content.projectilePrefabs.Add(taserPrefab);

                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Captain.Weapon.FireTazer", "projectilePrefab", taserPrefab);
                sk.secondary.skillFamily.variants[0].skillDef.skillDescriptionToken = "CAPTAIN_SECONDARY_DESC_RISKYMOD";
            }
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            if (nukeBuff)
            {
                GameObject nukePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/CaptainAirstrikeAltProjectile");
                ProjectileImpactExplosion pie = nukePrefab.GetComponent<ProjectileImpactExplosion>();
                pie.blastProcCoefficient = 3f;
            }
        }
    }

    public class Skills
    {
        public static SkillDef Shotgun;
    }
}
