using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class Scorpion
    {
        public static bool enabled = true;

        public Scorpion()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int scorpionCount = sender.GetBuffCount(DLC1Content.Buffs.PermanentDebuff);
            args.armorAdd += scorpionCount; //Vanilla is -= 2 * scorpionCount
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.PermanentDebuffOnHit);
        }
    }
}
