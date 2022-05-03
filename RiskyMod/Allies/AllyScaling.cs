using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RiskyMod.Allies
{
    public class AllyScaling
    {
        public static bool normalizeDroneDamage = true;

        public static bool noVoidDeath = true;
        public static bool noOverheat = true;

        public delegate void ChangeAllyScaling(AllyInfo ally);
        public static ChangeAllyScaling ChangeAllyScalingActions;

        public AllyScaling()
        {
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
            GameObject bodyPrefab = BodyCatalog.GetBodyPrefab(ally.bodyIndex);
            CharacterBody cb = null;
            if (bodyPrefab)
            {
                cb = bodyPrefab.GetComponent<CharacterBody>();
            }
            if (!bodyPrefab || !cb) return;

            if (noVoidDeath) cb.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
            if (noOverheat) cb.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;

            bool ignoreScaling = (ally.tags & AllyTag.DontModifyScaling) == AllyTag.DontModifyScaling;
            if (!ignoreScaling)
            {
                if ((ally.tags & AllyTag.Drone) == AllyTag.Drone)
                {
                    //Don't like how normalization is split between AllyScaling and AlliesCore
                    if (normalizeDroneDamage)
                    {
                        cb.baseDamage = 12f;
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

                //Drones always regen to full in 40s
                if ((ally.tags & AllyTag.DontModifyRegen) != AllyTag.DontModifyRegen)
                {
                    cb.baseRegen = cb.baseMaxHealth / 40f;
                    cb.levelRegen = cb.baseRegen * 0.2f;
                }

                //Set Level Stats
                cb.levelDamage = cb.baseDamage * 0.3f;
                cb.levelMaxHealth = cb.baseMaxHealth * 0.2f;
                cb.levelMaxShield = cb.baseMaxShield * 0.2f;
                cb.autoCalculateLevelStats = false;
            }

            //Can be used by external mods who want to do their own thing with custom allies?
            if (ChangeAllyScalingActions != null) ChangeAllyScalingActions.Invoke(ally);
        }
    }
}
