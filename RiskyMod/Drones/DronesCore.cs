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
        }

        private void ChangeScaling(GameObject go)
        {
            CharacterBody cb = go.GetComponent<CharacterBody>();

            cb.baseRegen = cb.baseMaxHealth / 30f;  //Drones take 30s to regen to full

            //Specific changes
            switch (cb.name)
            {
                case "MegaDroneBody": //If I'm gonna pay the price of a legendary chest to buy a drone, it better be worth it.
                    cb.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
                    break;
                case "SquidTurretBody": //These bleed HP pretty fast, they need a bit more resistance to help with that.
                    cb.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                    break;
                case "Turret1Body": //These seem to die too quickly, but AOE resist makes them too tanky.
                    cb.baseRegen = cb.baseMaxHealth / 20f;
                    cb.baseMaxShield = cb.baseMaxHealth * 0.15f;
                    break;
                default:
                    break;
            }
            
            //This makes their performance stay the same on every stage. (Everything's HP increases 30% per level)
            cb.levelRegen = cb.baseRegen * 0.3f;
            cb.levelDamage = cb.baseDamage * 0.3f;
            cb.levelArmor += 3f;
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
