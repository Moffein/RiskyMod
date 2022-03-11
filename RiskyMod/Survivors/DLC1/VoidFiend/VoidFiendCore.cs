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
            On.RoR2.VoidSurvivorController.OnCharacterHealServer += (orig, self, healthComponent, amount, proc) =>
            {
                return;
            };

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


        }
    }
}
