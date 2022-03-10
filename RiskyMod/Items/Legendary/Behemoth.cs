using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Items.Legendary
{
    public class Behemoth
    {
        public static bool enabled = true;
        public Behemoth()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.OnHitAll += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Behemoth")
                    );

                //Remove range Scaling
                c.GotoNext(
                    x => x.MatchLdcR4(1.5f),
                    x => x.MatchLdcR4(2.5f),
                    x => x.MatchLdloc(3)
                    );
                c.Index += 3;
                c.EmitDelegate<Func<int, int>>((itemCount) =>
                {
                    return 1;
                });

                //Add damage scaling
                c.GotoNext(
                    x => x.MatchLdcR4(0.6f)
                    );
                c.Index++;
                c.Emit(OpCodes.Ldloc_3);    //itemCount
                c.EmitDelegate<Func<float, int, float>>((origDamage, itemCount) =>
                {
                    return origDamage + 0.4f * (itemCount - 1);
                });
            };
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Behemoth);
        }
    }
}
