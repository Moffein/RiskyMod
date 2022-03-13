using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.DLC1.VoidFiend
{
    public class VoidFiendCore
    {
        public static GameObject bodyPrefab;
        public static bool enabled = true;

        public static bool fasterCorruptTransition = true;
        public static bool corruptOnKill = true;
        public static bool noCorruptHeal = true;
        public static bool noCorruptCrit = true;


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

            if (corruptOnKill)
            {
                //"KEYWORD_VOIDCORRUPTION_RISKYMOD"
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
