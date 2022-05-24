using RoR2;
using UnityEngine;
using R2API;

namespace RiskyMod.Tweaks.RunScaling
{
    public class LoopBossArmor
    {
        public static bool enabled = true;
        public LoopBossArmor()
        {
            if (!enabled) return;
            RecalculateStatsAPI.GetStatCoefficients += LoopArmor;
        }

        private static void LoopArmor(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isChampion)// && sender.isBoss
            {
                if (sender.inventory && sender.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) <= 0 && sender.inventory.GetItemCount(RoR2Content.Items.AdaptiveArmor) <= 0)
                {
                    args.armorAdd += 50f + 10f * (Run.instance.stageClearCount - 5);
                }
            }
        }
    }
}