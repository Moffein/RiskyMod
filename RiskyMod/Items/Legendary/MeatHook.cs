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
            if (!enabled || !RiskyMod.disableProcChains) return;

            //ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "BounceNearby")
                    )
                &&
                c.TryGotoNext(
                    x => x.MatchStfld<RoR2.Orbs.BounceOrb>("procCoefficient")
                    ))
                {
                    c.Index--;
                    c.Next.Operand = 0.1f;
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: MeatHook IL Hook failed");
                }
            };
        }
        /*private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.BounceNearby);
        }*/
    }
}
