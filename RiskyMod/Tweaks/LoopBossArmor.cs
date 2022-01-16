using RoR2;
using UnityEngine;

namespace RiskyMod.Tweaks
{
    public class LoopBossArmor
    {
        public static bool enabled = true;
        public LoopBossArmor()
        {
            if (!enabled) return;
            On.RoR2.CharacterBody.Start += (orig, self) =>
            {
                orig(self);
                if (Run.instance.stageClearCount > 5 && self.isBoss && self.isChampion)
                {
                    if (self.inventory
                    && self.inventory.GetItemCount(RoR2Content.Items.AdaptiveArmor.itemIndex) <= 0
                    && self.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger.itemIndex) <= 0)
                    {
                        self.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor.itemIndex);
                    }
                }
            };
        }
    }
}
