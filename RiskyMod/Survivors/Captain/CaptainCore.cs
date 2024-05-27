using EntityStates.RiskyMod.Captain;
using EntityStates;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Captain
{
    public class CaptainCore
    {
        public static bool enabled = true;
        public static bool enablePrimarySkillChanges = true;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CaptainBody");

        public static bool modifyTaser = true;
        public static bool nukeAmmopackNerf = true;
        public static bool nukeProc = true;

        public static bool disableNukeFriendlyFire = false;
        public static bool beaconRework = true;

        public static BodyIndex CaptainIndex;

        public CaptainCore()
        {
            if (!enabled) return;

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.Admiral"))
            {
                Debug.LogWarning("RiskyMod: Loading CaptainCore while Admiral is installed, there may be conflicts. Check your RiskyMod/Admiral config files and adjust accordingly.");
            }

            new Microbots();
            new CaptainOrbitalHiddenRealms();
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());

            On.RoR2.EntityStateCatalog.InitializeStateFields += (orig, self) =>
            {
                orig(self);
                ChargeShotgun.chargeupVfxPrefab = EntityStates.Captain.Weapon.ChargeCaptainShotgun.chargeupVfxPrefab;
                ChargeShotgun.holdChargeVfxPrefab = EntityStates.Captain.Weapon.ChargeCaptainShotgun.holdChargeVfxPrefab;
            };

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                CaptainIndex = BodyCatalog.FindBodyIndex("CaptainBody");
            };

            if (nukeAmmopackNerf || beaconRework)
            {
                On.RoR2.GenericSkill.ApplyAmmoPack += (orig, self) =>
                {
                    //Probably don't need to actually check for BeaconRework since vanilla beacons don't benefit from ammopacks.
                    if (CaptainCore.beaconRework && (self.skillName == "SupplyDrop1" || self.skillName == "SupplyDrop2"))
                    {
                        return;
                    }
                    else if (CaptainCore.nukeAmmopackNerf && self.activationState.stateType == typeof(EntityStates.Captain.Weapon.SetupAirstrikeAlt))
                    {
                        if (self.stock < self.maxStock)
                        {
                            self.rechargeStopwatch += Mathf.Max(20f, 0.5f * self.finalRechargeInterval);
                        }
                    }
                    else
                    {
                        orig(self);
                    }
                };
            }

            GameObject nukePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainAirstrikeAltProjectile.prefab").WaitForCompletion();
            ProjectileImpactExplosion pie = nukePrefab.GetComponent<ProjectileImpactExplosion>();
            if (nukeProc)
            {
                pie.blastProcCoefficient = 3f;
            }
            if (disableNukeFriendlyFire)
            {
                pie.blastAttackerFiltering = AttackerFiltering.AlwaysHitSelf;
            }
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            //ModifyUtilities(sk);
            ModifyBeacons(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            if (!enablePrimarySkillChanges) return;

            Content.Content.entityStates.Add(typeof(ChargeShotgun));
            Content.Content.entityStates.Add(typeof(FireShotgun));

            SkillDef captainShotgun = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CaptainShotgun.asset").WaitForCompletion();

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
            shotgunDef.icon = captainShotgun.icon;
            shotgunDef.interruptPriority = InterruptPriority.Any;
            shotgunDef.isCombatSkill = true;
            shotgunDef.keywordTokens = new string[] { };
            shotgunDef.mustKeyPress = false;
            shotgunDef.cancelSprintingOnActivation = true;
            shotgunDef.rechargeStock = 1;
            shotgunDef.requiredStock = 1;
            shotgunDef.skillName = "ChargeShotgun";
            shotgunDef.skillNameToken = "CAPTAIN_PRIMARY_NAME";
            shotgunDef.skillDescriptionToken = "CAPTAIN_PRIMARY_DESCRIPTION_RISKYMOD";
            shotgunDef.stockToConsume = 1;
            SneedUtils.SneedUtils.FixSkillName(shotgunDef);
            Content.Content.skillDefs.Add(shotgunDef);
            Skills.Shotgun = shotgunDef;

            //Leave this as a SkillDef replacement to smooth out interaction with Admiral.
            SneedUtils.SneedUtils.ReplaceSkillDef(sk.primary.skillFamily, captainShotgun, Skills.Shotgun);
        }

        public static void ScalePellets_SettingChanged(object sender, System.EventArgs e)
        {
            if (Skills.Shotgun)
                Skills.Shotgun.skillDescriptionToken = EntityStates.RiskyMod.Captain.FireShotgun.scalePellets.Value
                    ? "CAPTAIN_PRIMARY_DESCRIPTION_RISKYMOD"
                    : "CAPTAIN_PRIMARY_DESCRIPTION_VANILLA_RISKYMOD";
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (modifyTaser)
            {
                new TaserRework();

                SkillDef taserSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CaptainTazer.asset").WaitForCompletion();
                taserSkill.skillDescriptionToken = "CAPTAIN_SECONDARY_DESCRIPTION_RISKYMOD";
                //if (Tweaks.CharacterMechanics.Shock.enabled) taserSkill.keywordTokens = new string[] { "KEYWORD_SHOCKING_RISKYMOD" };
            }
        }

        private void ModifyBeacons(SkillLocator sk)
        {
            if (beaconRework)
            {
                new BeaconRework(sk);
            }
        }
    }

    public class Skills
    {
        public static SkillDef Shotgun;
    }
}
