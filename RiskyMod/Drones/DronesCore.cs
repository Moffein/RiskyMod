using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using MonoMod.Cil;
using System;
using System.Runtime.CompilerServices;

namespace RiskyMod.Drones
{
    public class DronesCore
    {
        public static bool enabled = true;
        public DronesCore()
        {
            if (!enabled) return;

            //Backup
            FixBackupScaling();
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
            ChangeScaling(LoadBody("SquidTurretBody"));

            //Beetle Allies
            ChangeScaling(LoadBody("BeetleGuardAllyBody"));
        }

        private void ChangeScaling(GameObject go)
        {
            CharacterBody cb = go.GetComponent<CharacterBody>();

            cb.baseRegen = cb.baseMaxHealth / 30f;  //Drones take 30s to regen to full

            //Specific changes
            switch (cb.name)
            {
                case "MegaDroneBody": //If I'm gonna pay the price of a legendary chest to buy a drone, it better be worth it.
                    cb.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;
                    cb.baseMaxShield = cb.baseMaxHealth * 0.15f;
                    cb.levelArmor += 2f;
                    break;
                case "SquidTurretBody": //These still can get destroyed too quickly. Needs more resistances.
                    cb.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                    cb.levelArmor += 4f;
                    break;
                case "Turret1Body": //Shield seems to be enough to put them in a good spot.
                    cb.baseRegen = cb.baseMaxHealth / 20f;
                    cb.baseMaxShield = cb.baseMaxHealth * 0.15f;
                    cb.levelArmor += 2f;
                    break;
                case "BeetleGuardAllyBody":
                    cb.levelArmor -= 1f;    //Queens Gland Guards get no armor bonus.
                    break;
                default:
                    break;
            }
            
            //This makes their performance stay the same on every stage. (Everything's HP increases 30% per level, so damage and regen increase matches that.)
            cb.levelRegen = cb.baseRegen * 0.3f;
            cb.levelDamage = cb.baseDamage * 0.3f;
            cb.levelArmor += 1f;
            cb.levelMaxShield = cb.baseMaxShield * 0.3f;
        }

        //Makes backup drones scale with ambient level like all other drones.
        private void FixBackupScaling()
        {
            IL.RoR2.EquipmentSlot.FireDroneBackup += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchStfld<MasterSuicideOnTimer>("lifeTimer")
                    );
                c.Index -= 7;
                c.EmitDelegate<Func<CharacterMaster, CharacterMaster>>((master) =>
                {
                    if (master.inventory)
                    {
                        master.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                    }
                    return master;
                });
            };
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObject LoadBody(string bodyname)
        {
            return Resources.Load<GameObject>("prefabs/characterbodies/" + bodyname);
        }
    }
}
