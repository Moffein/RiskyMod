using RoR2;
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
                if (self.expRewardCoefficient < 0.11f)
                {
                    self.expRewardCoefficient = 0.11f / (Run.instance.stageClearCount < 5 ? Mathf.Pow(1.2f, Run.instance.stageClearCount) : 2.5f);
                }
                orig(self);
            };
        }
    }
}
