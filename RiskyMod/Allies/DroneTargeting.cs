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
            ModifyAI(LoadMasterObject("EngiTurretMaster"));
            ModifyAI(LoadMasterObject("EngiWalkerTurretMaster"));
            ModifyAI(LoadMasterObject("EngiBeamTurretMaster"));

            //Drones
            ModifyAI(LoadMasterObject("Turret1Master"));
            ModifyAI(LoadMasterObject("Drone1Master"));
            ModifyAI(LoadMasterObject("MegaDroneMaster"));
            ModifyAI(LoadMasterObject("DroneMissileMaster"));
            ModifyAI(LoadMasterObject("FlameDroneMaster"));

            //Item Allies
            ModifyAI(LoadMasterObject("DroneBackupMaster"));
            ModifyAI(LoadMasterObject("SquidTurretMaster"));
            ModifyAI(LoadMasterObject("RoboBallGreenBuddyMaster"));
            ModifyAI(LoadMasterObject("RoboBallRedBuddyMaster"));

            ModifyAI(LoadMasterObject("BeetleGuardAllyMaster"), AlliesCore.beetleGlandDontRetaliate);
        }

        private void ModifyAI(GameObject masterObject, bool dontRetaliate = true)
        {
            AimAtEnemy(masterObject.GetComponents<AISkillDriver>());
            if (dontRetaliate) DontRetaliate(masterObject.GetComponents<BaseAI>());
        }

        private void AimAtEnemy(AISkillDriver[] skillDrivers)
        {
            foreach (var skillDriver in skillDrivers) skillDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
        }

        public static void DontRetaliate(BaseAI[] baseAIs)
        {
            foreach (var baseAI in baseAIs) baseAI.neverRetaliateFriendlies = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObject LoadMasterObject(string mastername)
        {
            return LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/" + mastername);
        }
    }
}
