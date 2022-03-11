using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.DLC1.VoidFiend
{
    public class VoidFiendCore
    {
        public static GameObject bodyPrefab;
        public static bool enabled = true;

        public static bool noCorruptHealing = true;
        public static bool instantTransition = true;

        public VoidFiendCore()
        {
            if (!enabled) return;

            //bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/VoidSurvivor");

            ModifyPassive();
        }

        private void ModifyPassive()
        {
            if (noCorruptHealing)
            {
                On.RoR2.VoidSurvivorController.OnCharacterHealServer += (orig, self, healthComponent, amount, proc) =>
                {
                    return;
                };
            }
            if (instantTransition)
            {
                //VoidSurvivor stuff seems to be missing from LegacyResourcesAPI
                //Debug.Log(SneedUtils.SneedUtils.GetEntityStateFieldString("VoidSurvivor.EnterCorruptionTransition", "duration"));
                //Debug.Log(SneedUtils.SneedUtils.GetEntityStateFieldString("VoidSurvivor.ExitCorruptionTransition", "duration"));

                On.EntityStates.VoidSurvivor.CorruptionTransitionBase.OnEnter += (orig, self) =>
                {
                     self.duration = 0f;    //Exiting has 0 duration
                     orig(self);
                };
            }
        }
    }
}
