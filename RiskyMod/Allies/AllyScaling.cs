using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RiskyMod.Allies
{
    public class AllyScaling
    {
        public static bool enabled = true;
        public static bool normalizeDroneDamage = true;

        public static bool noVoidDeath = true;
        public static bool noOverheat = true;

        public delegate void ChangeAllyScaling(AllyInfo ally);
        public static ChangeAllyScaling ChangeAllyScalingActions;

        public AllyScaling()
        {
            if (!enabled) return;
            AlliesCore.ModifyAlliesActions += ModifyAllies;
        }

        private void ModifyAllies(List<AllyInfo> allies)
        {
            foreach (AllyInfo ally in allies)
            {
                ChangeScaling(ally);
            }
        }

        private void ChangeScaling(AllyInfo ally)
        {
            //Can be used by external mods who want to do their own thing with custom allies?
            if (ChangeAllyScalingActions != null) ChangeAllyScalingActions.Invoke(ally);

            GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(ally.bodyIndex);
            CharacterBody cb = bodyPrefab.GetComponent<CharacterBody>();
            if (!cb) return;

            bool ignoreScaling = (ally.tags & AllyTag.DontModifyScaling) == AllyTag.DontModifyScaling;

            if (!ignoreScaling)
            {
                if (noVoidDeath) cb.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
                if (noOverheat) cb.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;

                if ((ally.tags & AllyTag.Drone) == AllyTag.Drone)
                {
                    cb.baseMaxShield += cb.baseMaxHealth * 0.08f;

                    if (normalizeDroneDamage)
                    {
                        cb.baseDamage = 12f;
                        //Account for normalized damage values
                        //This should probably be somewhere else since it's not strictly dependent on the body being passed in
                        switch (cb.name)
                        {

                            case "Drone1Body":
                                //These feel weak, so I won't bother adjusting their damage downwards
                                //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireTurret", "damageCoefficient", "0.4166666667");   //Damage 10 -> 12, coef 0.5 -> 0.4167
                                break;
                            case "BackupDroneBody":
                                cb.baseDamage = 7f; //Keep baseDamage the same since the firing state is tied to Drone1Body
                                break;
                            case "Turret1Body":
                                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireGatling", "damageCoefficient", "0.45");   //Damage 18 -> 12, coef 0.3 -> 0.45
                                break;
                            //case "EmergencyDroneBody":    //Shares stats and state with Drone2Body. No need to run this twice.
                            case "Drone2Body":
                                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.HealBeam", "healCoefficient", "1.7");   //Damage 10 -> 12, coef 2 -> 1.6667
                                break;
                            case "FlameDroneBody":
                                //Can't figure out how to change this. Flamethrower state might be tied to Artificer.
                                break;
                            case "MegaDroneBody":
                                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireMegaTurret", "damageCoefficient", "2.1");   //Damage 14 -> 12, coef 2.2 -> 2.5667 mult by 0.8
                                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireTwinRocket", "damageCoefficient", "3.75");   //Damage 14 -> 12, coef 4 -> 4.6667 mult by 0.8
                                break;
                            case "MissileDroneBody":
                                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireMissileBarrage", "damageCoefficient", "1.17");   //Damage 14 -> 12, coef 1 -> 1.166666667
                                break;
                        }
                    }
                }

                if ((ally.tags & AllyTag.Turret) == AllyTag.Turret)
                {
                    cb.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                }

                //Specific Tweaks
                switch (cb.name)
                {
                    case "SquidTurretBody":
                        cb.baseMaxHealth = 720f;
                        break;
                    case "RoboBallGreenBuddyBody":
                    case "RoboBallRedBuddyBody":
                        cb.baseDamage *= 0.8f;
                        break;
                }
                //Gunner Turrets used to have a 20% HP bonus, but see how they perform with the new turret resistance thing
                //Megadrone and RoboBalls need 0.8x damage penalty

                //Drones always regen to full in 40s
                cb.baseRegen = cb.baseMaxHealth / 40f;

                //Set Level Stats
                cb.levelDamage = cb.baseDamage * 0.3f;
                cb.levelMaxHealth = cb.baseMaxHealth * 0.2f;
                cb.levelMaxShield = cb.baseMaxShield * 0.2f;
                cb.levelRegen = cb.baseRegen * 0.2f;
                cb.autoCalculateLevelStats = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private GameObject LoadBody(string bodyname)
        {
            return LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/" + bodyname);
        }
    }
}
