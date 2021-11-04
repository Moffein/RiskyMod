using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using MonoMod.Cil;
using System;
using System.Runtime.CompilerServices;
using RoR2.CharacterAI;

namespace RiskyMod.Drones
{
    public class DronesCore
    {
        public static bool enabled = true;
        public DronesCore()
        {
            if (!enabled) return;
            ChangeDroneScaling();
            FixDroneTargeting();
        }

        private void ChangeDroneScaling()
        {//Backup
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
                    cb.baseRegen = cb.baseMaxHealth / 20f;
                    cb.baseMaxShield = cb.baseMaxHealth * 0.15f;
                    cb.levelArmor += 2f;
                    break;
                case "SquidTurretBody":
                    cb.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                    cb.levelArmor += 4f;
                    break;
                case "Turret1Body": //Shield seems to be enough to put them in a good spot.
                    cb.baseRegen = cb.baseMaxHealth / 20f;
                    cb.baseMaxShield = cb.baseMaxHealth * 0.15f;
                    cb.levelArmor += 2f;
                    break;
                case "FlameDroneBody": //These seem to die faster than other drones.
                    //cb.baseRegen = cb.baseMaxHealth / 20f;
                    cb.levelArmor += 1f;
                    break;
                case "BeetleGuardAllyBody":
                    cb.levelArmor -= 2f;    //Queens Gland Guards get no armor bonus.
                    break;
                default:
                    break;
            }
            
            //This makes their performance stay the same on every stage. (Everything's HP increases 30% per level, so damage and regen increase matches that.)
            cb.levelRegen = cb.baseRegen * 0.3f;
            cb.levelDamage = cb.baseDamage * 0.3f;
            cb.levelArmor += 2f;
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

        //Drone targeting fixes from https://github.com/William758/ZetTweaks/blob/main/GameplayModule.cs
        #region drone_targeting
        private void FixDroneTargeting()
        {
            //Engi
            AimAtEnemy(LoadMasterObject("EngiTurretMaster"));
            AimAtEnemy(LoadMasterObject("EngiWalkerTurretMaster"));
            AimAtEnemy(LoadMasterObject("EngiBeamTurretMaster"));

            //Drones
            AimAtEnemy(LoadMasterObject("Turret1Master"));
            AimAtEnemy(LoadMasterObject("Drone1Master"));
            AimAtEnemy(LoadMasterObject("MegaDroneMaster"));
            AimAtEnemy(LoadMasterObject("DroneMissileMaster"));
            AimAtEnemy(LoadMasterObject("FlameDroneMaster"));

            //Item Allies
            AimAtEnemy(LoadMasterObject("BeetleGuardAllyMaster"));
            AimAtEnemy(LoadMasterObject("DroneBackupMaster"));
            AimAtEnemy(LoadMasterObject("SquidTurretMaster"));
            AimAtEnemy(LoadMasterObject("RoboBallGreenBuddyMaster"));
            AimAtEnemy(LoadMasterObject("RoboBallRedBuddyMaster"));
        }

        private void AimAtEnemy(GameObject masterObject)
        {
            AimAtEnemy(masterObject.GetComponents<AISkillDriver>());
        }

        private void AimAtEnemy(AISkillDriver[] skillDrivers)
        {
            foreach (var skillDriver in skillDrivers) skillDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObject LoadMasterObject(string mastername)
        {
            return Resources.Load<GameObject>("prefabs/charactermasters/" + mastername);
        }
        #endregion
    }
}
