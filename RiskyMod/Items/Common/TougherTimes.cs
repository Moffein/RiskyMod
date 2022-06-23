using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
namespace RiskyMod.Items.Common
{
    public class TougherTimes
    {
        public static bool enabled = true;
        public TougherTimes()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Change block chance
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdcR4(15f),
                     x => x.MatchLdarg(0),
                     x => x.MatchLdflda(typeof(RoR2.HealthComponent), "itemCounts"),
                     x => x.MatchLdfld(typeof(RoR2.HealthComponent.ItemCounts), "bear")
                    ))
                {
                    c.Next.Operand = 10f;
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: TougherTimes IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Bear);
        }
    }
}
