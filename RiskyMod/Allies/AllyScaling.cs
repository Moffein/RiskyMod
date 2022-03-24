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

            Debug.Log("\nModifying Drone Scaling");

            bool ignoreScaling = (ally.tags & AllyTag.DontModifyScaling) == AllyTag.DontModifyScaling;

            if (!ignoreScaling)
            {
                Debug.Log("IgnoreScaling = false");
                if (noVoidDeath) cb.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
                if (noOverheat) cb.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;

                if ((ally.tags & AllyTag.Drone) == AllyTag.Drone)
                {
                    cb.baseMaxShield += cb.baseMaxHealth * 0.08f;

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
