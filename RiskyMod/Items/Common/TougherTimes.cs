using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
namespace RiskyMod.Items.Common
{
    public class TougherTimes
    {
        public static bool enabled = true;
        public static ItemDef itemDef = RoR2Content.Items.Bear;
        public TougherTimes()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, itemDef);

            if (RiskyMod.AIBlacklistUseVanillaBlacklist) SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.Bear, ItemTag.AIBlacklist);

            //Change block chance
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(15f),
                     x => x.MatchLdarg(0),
                     x => x.MatchLdflda(typeof(RoR2.HealthComponent), "itemCounts"),
                     x => x.MatchLdfld(typeof(RoR2.HealthComponent.ItemCounts), "bear")
                    );
                c.Next.Operand = 7.5f;
            };
        }
    }
}
