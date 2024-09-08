using RoR2;
using System;

namespace RiskyMod.Items.DLC1.Uncommon
{
    public class Ignition
    {
        public static bool enabled = true;

        public Ignition()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            SharedHooks.OnHitEnemy.OnHitAttackerInventoryActions += BurnChance;
        }

        private void BurnChance(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody, Inventory attackerInventory)
        {
            if (attackerInventory.GetItemCount(DLC1Content.Items.StrengthenBurn) > 0 && Util.CheckRoll(5f * damageInfo.procCoefficient, attackerBody.master))
            {
                InflictDotInfo inflictDotInfo = new InflictDotInfo
                {
                    victimObject = victimBody.gameObject,
                    attackerObject = damageInfo.attacker,
                    totalDamage = new float?(damageInfo.damage * 0.5f),
                    dotIndex = DotController.DotIndex.Burn,
                    damageMultiplier = 1f
                };
                StrengthenBurnUtils.CheckDotForUpgrade(attackerInventory, ref inflictDotInfo);
                DotController.InflictDot(ref inflictDotInfo);
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.StrengthenBurn);
        }
    }
}
