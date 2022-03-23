using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RiskyMod.Drones
{
    public class DroneScaling
    {
        public static bool enabled = true;

        public delegate void ChangeDroneBodyScaling(CharacterBody body);
        public static ChangeDroneBodyScaling ChangeDroneBodyScalingActions;

        public DroneScaling()
        {
            if (!enabled) return;
            DronesCore.ModifyAlliesActions += ModifyAllies;
        }

        private void ModifyAllies(List<BodyIndex> bodies)
        {
            foreach (BodyIndex i in bodies)
            {
                ChangeScaling(BodyCatalog.GetBodyPrefab(i));
            }
        }

        private void ChangeScaling(GameObject go)
        {
            CharacterBody cb = go.GetComponent<CharacterBody>();
            if (!cb) return;
            if (ChangeDroneBodyScalingActions != null) ChangeDroneBodyScalingActions.Invoke(cb);
            bool useShield = cb.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical);

            //Specific changes
            switch (cb.name)
            {
                case "MegaDroneBody":
                    cb.baseDamage *= 0.8f;
                    break;
                case "SquidTurretBody":
                    cb.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                    cb.baseMaxHealth = 720f;
                    break;
                case "Turret1Body":
                    cb.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                    cb.baseMaxHealth *= 1.2f;
                    break;
                case "RoboBallGreenBuddyBody":
                case "RoboBallRedBuddyBody":
                    cb.baseDamage *= 0.8f;  //These already do good damage in Vanilla.
                    break;
                default:
                    break;
            }
            cb.levelDamage = cb.baseDamage * 0.3f;

            if (useShield)
            {
                cb.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.ImmuneToVoidDeath;
                cb.baseMaxShield += cb.baseMaxHealth * 0.08f;
                cb.levelMaxShield = cb.baseMaxShield * 0.3f;
            }
            cb.levelMaxHealth = cb.baseMaxHealth * 0.2f;    //0.3f is standard scaling

            cb.baseRegen = cb.baseMaxHealth / 40f;  //Drones take a fixed amount of time to regen to full.
            cb.levelRegen = cb.baseRegen * 0.2f;    //Change to 0.2 (standard scaling) now that HP is 0.2/level

            cb.autoCalculateLevelStats = false; //Nonstandard stat scaling is used, so this needs to be set
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObject LoadBody(string bodyname)
        {
            return LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/" + bodyname);
        }
    }
}
