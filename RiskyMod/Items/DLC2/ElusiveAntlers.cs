using RoR2;
using UnityEngine;
using System;

namespace RiskyMod.Items.DLC2
{
    public class ElusiveAntlers
    {
        public static bool enabled = true;
        public ElusiveAntlers()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            On.RoR2.CharacterBody.GetElusiveAntlersCurrentMaxStack += CharacterBody_GetElusiveAntlersCurrentMaxStack;
        }

        private int CharacterBody_GetElusiveAntlersCurrentMaxStack(On.RoR2.CharacterBody.orig_GetElusiveAntlersCurrentMaxStack orig, CharacterBody self)
        {
            int num = self.inventory ? self.inventory.GetItemCount(DLC2Content.Items.SpeedBoostPickup) : 0;
            if (num <= 0)
            {
                return 3;
            }
            return 1 + 2 * num;
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC2Content.Items.SpeedBoostPickup);
        }
    }
}
