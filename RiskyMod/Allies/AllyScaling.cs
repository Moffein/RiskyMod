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
            CharacterBody cb = null;
            if (bodyPrefab)
            {
                cb = bodyPrefab.GetComponent<CharacterBody>();
            }
            if (!bodyPrefab || !cb) return;

            bool ignoreScaling = (ally.tags & AllyTag.DontModifyScaling) == AllyTag.DontModifyScaling;

            if (!ignoreScaling)
            {
                if (noVoidDeath) cb.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
                if (noOverheat) cb.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;

                if ((ally.tags & AllyTag.Drone) == AllyTag.Drone)
                {
                    //Don't like how normalization is split between AllyScaling and AlliesCore
                    if (normalizeDroneDamage)
                    {
                        cb.baseDamage = 12f;
                        //Account for normalized damage values
                        //This should probably be somewhere else since it's not strictly dependent on the body being passed in
                        if (cb.name == "BackupDroneBody")
                        {
                            cb.baseDamage = 8.5f; //Shares firing state with Gunner Drones, so needs lower damage. Technically makes drone parts worse on this.
                        }
                        else if (cb.name == "Turret1Body")
                        {
                            cb.baseMaxHealth *= 1.2f;
                        }
                        else if (cb.name == "Drone1Body" || cb.name == "Drone2Body")
                        {
                            cb.baseMaxHealth = 170f;    //vanilla is 150
                        }
                        //RoboBallBuddyGreen/Red get damage reduced from 15 -> 12, same as the earlier build with the 0.8x damage penalty
                    }
                }

                if ((ally.tags & AllyTag.Turret) == AllyTag.Turret)
                {
                    cb.bodyFlags |= CharacterBody.BodyFlags.ResistantToAOE;
                }

                if ((ally.tags & AllyTag.UseShield) == AllyTag.UseShield)
                {
                    cb.baseMaxShield += cb.baseMaxHealth * 0.08f;
                }

                //Specific Tweaks
                switch (cb.name)
                {
                    case "SquidTurretBody":
                        cb.baseMaxHealth = 720f;
                        break;
                }
                //Gunner Turrets used to have a 20% HP bonus, but see how they perform with the new turret resistance thing
                //Megadrone and RoboBalls need 0.8x damage penalty

                //Drones always regen to full in 40s
                if (!((ally.tags & AllyTag.DontModifyRegen) == AllyTag.DontModifyRegen))
                {
                    cb.baseRegen = cb.baseMaxHealth / 30f;
                    cb.levelRegen = cb.baseRegen * 0.2f;
                }

                //Set Level Stats
                cb.levelDamage = cb.baseDamage * 0.3f;
                cb.levelMaxHealth = cb.baseMaxHealth * 0.2f;
                cb.levelMaxShield = cb.baseMaxShield * 0.2f;
                cb.autoCalculateLevelStats = false;
            }
        }
    }
}
