using RoR2;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine;

namespace RiskyMod.Enemies.Bosses
{
    public class Grandparent
    {
        public static bool enabled = true;
        public Grandparent()
        {
            if (!enabled) return;

            On.RoR2.CharacterBody.UpdateBuffs += (orig, self, deltaTime) =>
            {
                bool hadSun = self.HasBuff(RoR2Content.Buffs.Overheat);

                orig(self, deltaTime);

                if (NetworkServer.active && hadSun && !self.HasBuff(RoR2Content.Buffs.Overheat))
                {
                    DotController dotController = DotController.FindDotController(self.gameObject);

                    if (dotController)
                    {
                        for (int i = dotController.dotStackList.Count - 1; i >= 0; i--)
                        {
                            DotController.DotStack dotStack = dotController.dotStackList[i];
                            if (dotStack.dotIndex == DotController.DotIndex.Burn)
                            {
                                dotStack.damage = 0f;
                                dotStack.timer = 0f;
                                dotController.RemoveDotStackAtServer(i);
                            }
                        }
                    }
                }
            };
        }
    }
}
