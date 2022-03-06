using RoR2;
using RoR2.CharacterAI;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RiskyMod.Drones
{
    public class DroneScaling
    {
        public static bool enabled = true;
        public DroneScaling()
        {
            if (!enabled) return;

            //Backup
            ChangeScaling(LoadBody("BackupDroneBody"));

            //T1 drones
            ChangeScaling(LoadBody("Drone1Body"));
            ChangeScaling(LoadBody("Drone2Body"));
            ChangeScaling(LoadBody("Turret1Body"));

            //T2 drones
            ChangeScaling(LoadBody("MissileDroneBody"));
            ChangeScaling(LoadBody("FlameDroneBody"));
            ChangeScaling(LoadBody("EquipmentDroneBody"));
            ChangeScaling(LoadBody("EmergencyDroneBody"));

            //T3 drones
            ChangeScaling(LoadBody("MegaDroneBody"));

            //Squids
            ChangeScaling(LoadBody("SquidTurretBody"), false);

            //Beetle Allies
            ChangeScaling(LoadBody("BeetleGuardAllyBody"), false);
        }

        private void ChangeScaling(GameObject go, bool useShield = true)
        {
            CharacterBody cb = go.GetComponent<CharacterBody>();

            //Specific changes
            switch (cb.name)
            {
                case "MegaDroneBody":
                    cb.baseDamage *= 0.7f;
                    cb.baseMaxHealth *= 0.8f;
                    break;
                case "SquidTurretBody":
                    cb.levelArmor += 1f;
                    cb.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                    cb.baseMaxHealth = 720f;
                    break;
                case "Turret1Body":
                    cb.levelArmor += 1f;
                    cb.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                    cb.baseMaxHealth *= 1.2f;
                    break;
                default:
                    break;
            }
            cb.levelDamage = cb.baseDamage * 0.3f;

            //This makes their performance stay the same on every stage. (Everything's HP increases 30% per level, so damage and regen increase matches that.)
            if (useShield)
            {
                cb.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ImmuneToExecutes;
                cb.baseMaxShield += cb.baseMaxHealth * 0.08f;
                cb.levelMaxShield = cb.baseMaxShield * 0.3f;
            }
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;

            cb.baseRegen = cb.baseMaxHealth / 40f;  //Drones take a fixed amount of time to regen to full.
            cb.levelRegen = cb.baseRegen * 0.3f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObject LoadBody(string bodyname)
        {
            return LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/" + bodyname);
        }
    }
}
