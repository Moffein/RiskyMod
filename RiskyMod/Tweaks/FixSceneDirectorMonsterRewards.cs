using UnityEngine;

namespace RiskyMod.Tweaks
{
    public class SceneDirectorMonsterRewards
    {
        public static bool enabled = true;
        public SceneDirectorMonsterRewards()
        {
            if (!enabled) return;
            On.RoR2.SceneDirector.PopulateScene += (orig, self) =>
            {
                if (self.expRewardCoefficient < 0.2f)
                {
                    self.expRewardCoefficient = 0.2f;
                }
                orig(self);
            };
        }
    }
}
