using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Items.Legendary
{
    public class MeatHook
    {
        public static bool enabled = true;
        public MeatHook()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BounceNearby);

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BounceNearby")
                    );

                //Add damage scaling.
                c.GotoNext(
                    x => x.MatchLdcR4(1f),
                    x => x.MatchStloc(37),
                    x => x.MatchLdarg(1),
                    x => x.MatchLdfld<DamageInfo>("damage")
                    );
                c.Index++;
                c.Emit(OpCodes.Ldloc, 13);  //itemcount
                c.EmitDelegate<Func<float, int, float>>((damage, itemCount) =>
                {
                    return damage + 0.3f * damage * (itemCount - 1);
                });

                if (RiskyMod.disableProcChains)
                {
                    //Remove proc coefficient
                    c.GotoNext(
                        x => x.MatchStfld<RoR2.Orbs.BounceOrb>("procCoefficient")
                        );
                    c.Index--;
                    c.Next.Operand = 0f;
                }
            };
        }
    }
}
