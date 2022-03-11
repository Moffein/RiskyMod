using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.DLC1.VoidFiend
{
    public class VoidFiendCore
    {
        public static GameObject bodyPrefab;
        public static bool enabled = true;

        public static bool modifyCorruption = true;

        public VoidFiendCore()
        {
            if (!enabled) return;

            //bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/VoidSurvivor");

            ModifySkills();
        }

        private void ModifySkills()
        {
            ModifyPassive();
        }

        private void ModifyPassive()
        {
            if (!modifyCorruption) return;

            //"KEYWORD_VOIDCORRUPTION_RISKYMOD"

            //Remove healing affecting Corruption.
            On.RoR2.VoidSurvivorController.OnCharacterHealServer += (orig, self, healthComponent, amount, proc) =>
            {
                return;
            };

            //Tweak Corruption transition duration.
            On.EntityStates.VoidSurvivor.CorruptionTransitionBase.OnEnter += (orig, self) =>
            {
                if (self.duration > 0f) //Exiting has 0 duration by default
                {
                    self.duration = 0.6f;  //Default enter duration is 1f
                }
                else
                {
                    self.duration = 0.1f;   //Noticed some crashes when exiting Corrupt state so I'm setting this to a positive number to see if it fixes those. Needs futher investigation.
                }
                orig(self);
            };

            //Remove crits affecting Corruption.
            //Not showstopping like Healing reducing Corruption, but it's odd how you're forced to get a specific few items to utilize this.
            //Pretty much gives Corruption for building certain items, rather than something that's actively a part of your gameplay.
            On.RoR2.VoidSurvivorController.OnDamageDealtServer += (orig, self, damageReport) =>
            {
                return;
            };

            //Kills increase Corruption.
            //Hoping this adds an additional layer of depth to using Void Fiend's Corruption
            AssistManager.HandleAssistActions += CorruptionAssist;

            //Reduce passive Corruption gain since kills now give a decent boost to build rate
            On.RoR2.VoidSurvivorController.OnEnable += (orig, self) =>
            {
                orig(self);
                self.corruptionPerSecondInCombat = 2f;
                self.corruptionPerSecondOutOfCombat = 2f;
                self.corruptionFractionPerSecondWhileCorrupted = -1f / 12f;

                //Debug.Log(self.maxCorruption); //100
                //Debug.Log(self.corruptionPerSecondInCombat);   //3
                //Debug.Log(self.corruptionPerSecondOutOfCombat);    //3
                //Debug.Log(self.corruptionFractionPerSecondWhileCorrupted); //-0.06666667
                //Debug.Log(self.corruptionPerCrit); //2
            };
        }

        private static void CorruptionAssist(CharacterBody attackerBody, CharacterBody victimBody, CharacterBody killerBody)
        {
            VoidSurvivorController vsc = attackerBody.gameObject.GetComponent<VoidSurvivorController>();
            if (vsc)
            {
                vsc.AddCorruption(4f);
            }
        }
    }
}
