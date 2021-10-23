using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace Risky_Mod.Drones
{
    public class DronesCore
    {
        public static bool enabled = true;
        public void Modify()
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
                /*case "Turret1Body":
                    cb.baseRegen = cb.baseMaxHealth / 20f;  //Stationary, cannot dodge. Needs regen.
                    break;
                case "FlameDroneBody":
                    cb.baseRegen = cb.baseMaxHealth / 20f;  //Has to get close to deal damage.
                    break;*/
                default:
                    break;
            }

            //This makes their performance stay the same on every stage. (Everything's HP increases 30% per level)
            cb.levelRegen = cb.baseRegen * 0.3f;
            cb.levelDamage = cb.baseDamage * 0.2f;
            //cb.levelArmor += 3f;  //Give armor on levelup if they are still dying like flies
        }

        //Makes backup drones scale with ambient level like all other drones.
        private void FixBackupScaling()
        {
            On.RoR2.EquipmentSlot.FireDroneBackup += (orig, self) =>
            {
                int sliceCount = 4;
                float num = 25f;
                if (NetworkServer.active)
                {
                    float y = Quaternion.LookRotation(self.GetAimRay().direction).eulerAngles.y;
                    float d = 3f;
                    foreach (float num2 in new DegreeSlices(sliceCount, 0.5f))
                    {
                        Quaternion rotation = Quaternion.Euler(0f, y + num2, 0f);
                        Quaternion rotation2 = Quaternion.Euler(0f, y + num2 + 180f, 0f);
                        Vector3 position = self.transform.position + rotation * (Vector3.forward * d);
                        CharacterMaster characterMaster = self.SummonMaster(Resources.Load<GameObject>("Prefabs/CharacterMasters/DroneBackupMaster"), position, rotation2);
                        if (characterMaster)
                        {
                            characterMaster.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = num + UnityEngine.Random.Range(0f, 3f);

                            if (characterMaster.inventory)
                            {
                                characterMaster.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel.itemIndex);
                            }
                        }
                    }
                }
                self.subcooldownTimer = 0.5f;
                return true;
            };
        }

        private GameObject LoadBody(string bodyname)
        {
            return Resources.Load<GameObject>("prefabs/characterbodies/" + bodyname);
        }
    }
}
