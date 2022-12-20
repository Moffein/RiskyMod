using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Skills;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.DLC1.VoidFiend
{
    public class VoidFiendCore
    {
        public static bool enabled = true;

        public static bool fasterCorruptTransition = true;
        public static bool corruptMeterTweaks = true;
        public static bool corruptOnKill = true;
        public static bool noCorruptHeal = true;
        public static bool noCorruptCrit = true;
        public static bool removeCorruptArmor = true;

        public static bool secondaryMultitask = true;

        public static bool modifyCorruptCrush = true;

        public static BodyIndex bodyIndex;
        public static GameObject bodyPrefab;

        public VoidFiendCore()
        {
            if (!enabled) return;

            bodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBody.prefab").WaitForCompletion();

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                bodyIndex = BodyCatalog.FindBodyIndex("VoidSurvivor");
            };

            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPassive(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
            ModifySpecials(sk);
        }

        private void ModifyPassive(SkillLocator sk)
        {
            if (fasterCorruptTransition)
            {
                //Tweak Corruption transition duration.
                On.EntityStates.VoidSurvivor.CorruptionTransitionBase.OnEnter += (orig, self) =>
                {
                    if (self.duration > 0f) //Exiting has 0 duration by default
                    {
                        self.duration = 0.5f;  //Default enter duration is 1f
                    }
                    orig(self);
                };
            }

            if (corruptMeterTweaks)
            {
                //Reduce passive Corruption gain since kills now give a decent boost to build rate
                On.RoR2.VoidSurvivorController.OnEnable += (orig, self) =>
                {
                    orig(self);
                    self.corruptionPerSecondInCombat = 1.6666666667f;   //60s for full corruption
                    self.corruptionPerSecondOutOfCombat = 1.6666666667f;
                    self.corruptionFractionPerSecondWhileCorrupted = -1f / 12f;

                    //Debug.Log(self.maxCorruption); //100
                    //Debug.Log(self.corruptionPerSecondInCombat);   //3
                    //Debug.Log(self.corruptionPerSecondOutOfCombat);    //3
                    //Debug.Log(self.corruptionFractionPerSecondWhileCorrupted); //-0.06666667
                    //Debug.Log(self.corruptionPerCrit); //2
                };
            }

            if (corruptOnKill)
            {
                //Which part changes the keyword? PassiveSkill doesn't seem to be it.
                //"KEYWORD_VOIDCORRUPTION_RISKYMOD"
                SkillDef passive = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/VoidSurvivor/VoidSurvivorPassive.asset").WaitForCompletion();
                passive.keywordTokens = new string[] { "KEYWORD_VOIDCORRUPTION_RISKYMOD" };

                //Kills increase Corruption.
                //Hoping this adds an additional layer of depth to using Void Fiend's Corruption
                AssistManager.HandleAssistActions += CorruptionAssist;
            }

            if (noCorruptHeal)
            {
                //Remove healing affecting Corruption.
                On.RoR2.VoidSurvivorController.OnCharacterHealServer += (orig, self, healthComponent, amount, proc) =>
                {
                    return;
                };
            }

            if (noCorruptCrit)
            {
                //Remove crits affecting Corruption.
                //Not showstopping like Healing reducing Corruption, but it's odd how you're forced to get a specific few items to utilize this.
                //Pretty much gives Corruption for building certain items, rather than something that's actively a part of your gameplay.
                On.RoR2.VoidSurvivorController.OnDamageDealtServer += (orig, self, damageReport) =>
                {
                    return;
                };
            }

            if (removeCorruptArmor)
            {
                R2API.RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
                {
                    if (sender.HasBuff(DLC1Content.Buffs.VoidSurvivorCorruptMode))
                    {
                        args.armorAdd -= 100f;
                    }
                };
            }
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (secondaryMultitask)
            {
                //Based on https://thunderstore.io/package/Bog/VoidFiendTweaks/
                EntityStateMachine cannonStateMachine = bodyPrefab.AddComponent<EntityStateMachine>();
                cannonStateMachine.customName = "RightArmCannon";
                cannonStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));
                cannonStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.BaseState));

                NetworkStateMachine nsm = bodyPrefab.GetComponent<NetworkStateMachine>();
                nsm.stateMachines = nsm.stateMachines.Append(cannonStateMachine).ToArray();

                SkillDef m2 = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/VoidSurvivor/ChargeMegaBlaster.asset").WaitForCompletion();
                m2.activationStateMachineName = "RightArmCannon";

                SkillDef corruptM2 = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/VoidSurvivor/FireCorruptDisk.asset").WaitForCompletion();
                corruptM2.activationStateMachineName = "RightArmCannon";
            }
        }

        private void ModifyUtilities(SkillLocator sk)
        {
            new UtilityFallImmune();
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (modifyCorruptCrush)
            {
                SkillDef crushHealth = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/VoidSurvivor/CrushHealth.asset").WaitForCompletion();
                crushHealth.baseMaxStock = 1;
                crushHealth.baseRechargeInterval = 0;
                crushHealth.rechargeStock = 1;

                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC1/VoidSurvivor/EntityStates.VoidSurvivor.Weapon.ChargeCrushHealth.asset", "baseDuration", "0.3"); //vanilla is 1
            }
        }

        private void CorruptionAssist(CharacterBody attackerBody, CharacterBody victimBody, CharacterBody killerBody)
        {
            VoidSurvivorController vsc = attackerBody.gameObject.GetComponent<VoidSurvivorController>();
            if (vsc)
            {
                vsc.AddCorruption(4f);
            }
        }
    }
}
