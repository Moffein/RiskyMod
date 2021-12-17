using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Croco
{
    public class BlightStack
    {
        public static bool enabled = true;
        public BlightStack()
        {
            if (!enabled) return;
            On.RoR2.DotController.AddDot += (orig, self, attackerObject, duration, dotIndex, damageMultiplier) =>
            {
                orig(self, attackerObject, duration, dotIndex, damageMultiplier);
                if (dotIndex == DotController.DotIndex.Blight)
                {
                    for (int i = 0; i < self.dotStackList.Count; i++)
                    {
                        if (self.dotStackList[i].dotIndex == DotController.DotIndex.Blight)
                        {
                            self.dotStackList[i].timer = Mathf.Max(self.dotStackList[i].timer, duration);
                        }
                    }
                }
            };
        }
    }
}
