using UnityEngine;

namespace RiskyMod.Enemies.Mithrix
{
    public class MithrixDebuffResist
    {
        public static bool enabled = true;
        public MithrixDebuffResist()
        {
            if (!enabled) return;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, RoR2.CharacterBody self)
        {
            orig(self);
            if (self.bodyIndex != MithrixCore.brotherBodyIndex) return;

            if (self.moveSpeed > 0f)
            {
                float desiredMoveSpeed = self.baseMoveSpeed;
                if (self.isSprinting) desiredMoveSpeed *= self.sprintingSpeedMultiplier;

                if (self.moveSpeed < desiredMoveSpeed)
                {
                    self.moveSpeed = Mathf.Lerp(self.moveSpeed, desiredMoveSpeed, 0.5f);
                }
            }

            if (self.attackSpeed > 0f && self.attackSpeed < 1f)
            {
                self.attackSpeed = Mathf.Lerp(self.attackSpeed, 1f, 0.5f);
            }
        }
    }
}
