using R2API;
using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks
{
    public class PlayerControlledMonsters
    {
        public static bool enabled = true;
        public PlayerControlledMonsters()
        {
            if (!enabled) return;

            RecalculateStatsAPI.GetStatCoefficients += PlayerControlledMonsterStats;
        }

        private static void PlayerControlledMonsterStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isPlayerControlled && sender.baseRegen == 0f && sender.levelRegen == 0f)
            {
                args.baseRegenAdd += 2f + 0.5f * sender.level;
                args.armorAdd += 20f;
            }
        }
    }
}
