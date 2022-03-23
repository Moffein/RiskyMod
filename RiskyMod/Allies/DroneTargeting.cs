using RoR2;
using RoR2.CharacterAI;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RiskyMod.Allies
{
    public class DroneTargeting
    {
        public static bool enabled = true;
        //Drone targeting fixes from https://github.com/William758/ZetTweaks/blob/main/GameplayModule.cs
        public DroneTargeting()
        {
            if (!enabled) return;

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
            return LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/" + mastername);
        }
    }
}
