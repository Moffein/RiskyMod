﻿using R2API;
using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class PlayerControlledMonsters
    {
        public static bool enabled = true;
        public PlayerControlledMonsters()
        {
            if (!enabled) return;

            RecalculateStatsAPI.GetStatCoefficients += PlayerControlledMonsterStats;

            On.RoR2.SetStateOnHurt.SetStunInternal += (orig, self, duration) =>
            {
                if (self.targetStateMachine && self.targetStateMachine.commonComponents.characterBody && self.targetStateMachine.commonComponents.characterBody.isPlayerControlled) return;
                orig(self, duration);
            };

            On.RoR2.SetStateOnHurt.SetShockInternal += (orig, self, duration) =>
            {
                if (self.targetStateMachine && self.targetStateMachine.commonComponents.characterBody && self.targetStateMachine.commonComponents.characterBody.isPlayerControlled) return;
                orig(self, duration);
            };

            On.RoR2.SetStateOnHurt.SetPainInternal += (orig, self) =>
            {
                if (self.targetStateMachine && self.targetStateMachine.commonComponents.characterBody && self.targetStateMachine.commonComponents.characterBody.isPlayerControlled) return;
                orig(self);
            };
        }

        private static void PlayerControlledMonsterStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isPlayerControlled && sender.baseRegen == 0f && sender.levelRegen == 0f && !sender.bodyFlags.HasFlag(CharacterBody.BodyFlags.ImmuneToExecutes))
            {
                args.baseRegenAdd += 2f + 0.5f * sender.level;
                args.armorAdd += 20f;
            }
        }
    }
}
