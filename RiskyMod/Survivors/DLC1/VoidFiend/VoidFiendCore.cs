using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.DLC1.VoidFiend
{
    public class VoidFiendCore
    {
        public static bool enabled = true;

        public static bool corruptMeterTweaks = true;
        public static bool corruptOnKill = true;
        public static bool noCorruptHeal = true;
        public static bool noCorruptCrit = true;
        public static bool removeCorruptArmor = true;

        public static bool secondaryMultitask = true;

        public static BodyIndex bodyIndex;
        public static GameObject bodyPrefab;

        public VoidFiendCore()
        {
            if (!enabled) return;

            bodyPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBody.prefab").WaitForCompletion();

            RoR2Application.onLoad += OnLoad;

            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void OnLoad()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("VoidSurvivorBody");
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPassive(sk);
            ModifySecondaries(sk);
            ModifyUtilities(sk);
        }

        private void ModifyPassive(SkillLocator sk)
        {

            if (corruptMeterTweaks)
            {
                //Reduce passive Corruption gain since kills now give a decent boost to build rate
                On.RoR2.VoidSurvivorController.OnEnable += (orig, self) =>
                {
                    orig(self);
                    self.corruptionPerSecondInCombat = 100f/60f;   //60s for full corruption
                    self.corruptionPerSecondOutOfCombat = 100f/60f;
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
                SkillDef passive = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC1/VoidSurvivor/VoidSurvivorPassive.asset").WaitForCompletion();
                passive.keywordTokens = new string[] { "KEYWORD_VOIDCORRUPTION_RISKYMOD" };

                //Kills increase Corruption.
                //Hoping this adds an additional layer of depth to using Void Fiend's Corruption
                AssistManager.AssistManager.HandleAssistCompatibleActions += CorruptionAssist;
                GlobalEventManager.onCharacterDeathGlobal += CorruptionOnKill;
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

        private void CorruptionAssist(CharacterBody attackerBody, CharacterBody victimBody, DamageType? assistDamageType, DamageTypeCombo? assistDamageTypeCombo, HashSet<DamageAPI.ModdedDamageType> assistModdedDamageTypes, CharacterBody killerBody, DamageInfo damageInfo)
        {
            if (attackerBody == killerBody || attackerBody.bodyIndex != bodyIndex) return;
            VoidSurvivorController vsc = attackerBody.gameObject.GetComponent<VoidSurvivorController>();
            if (vsc) vsc.AddCorruption(4f);
        }

        private void CorruptionOnKill(DamageReport report)
        {
            if (report.attacker && report.attackerBodyIndex == bodyIndex)
            {
                VoidSurvivorController vsc = report.attacker.GetComponent<VoidSurvivorController>();
                if (vsc) vsc.AddCorruption(4f);
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
    }
}
