using EntitiyStates.RiskyMod.Captain;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RiskyMod.Survivors.Captain
{
    public class CaptainCore
    {
        public static bool enabled = true;
        public static bool enablePrimarySkillChanges = true;

        public CaptainCore()
        {
            if (!enabled) return;
            new Microbots();
            new CaptainOrbitalHiddenRealms();
            ModifySkills(RoR2Content.Survivors.Captain.bodyPrefab.GetComponent<SkillLocator>());

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
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            if (!enablePrimarySkillChanges) return;

            R2API.ContentAddition.AddEntityState<ChargeShotgun>(out bool x);
            R2API.ContentAddition.AddEntityState<FireShotgun>(out bool y);

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
            R2API.ContentAddition.AddSkillDef(shotgunDef);
            Skills.Shotgun = shotgunDef;
            sk.primary._skillFamily.variants[0].skillDef = Skills.Shotgun;
        }
    }

    public class Skills
    {
        public static SkillDef Shotgun;
    }
}
