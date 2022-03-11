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

            ModifyPassive();
        }

        private void ModifyPassive()
        {
            if (!modifyCorruption) return;

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
            On.RoR2.VoidSurvivorController.OnDamageDealtServer += (orig, self, damageReport) =>
            {
                return;
            };

            //Kills increase Corruption.
            //Hoping this adds an additional layer of depth to using Void Fiend's Corruption
        }
    }
}
