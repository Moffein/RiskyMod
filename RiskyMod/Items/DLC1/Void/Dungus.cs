using System;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.DLC1.Void
{
    public class Dungus
    {
        public static bool enabled = true;
        public Dungus()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

			IL.RoR2.MushroomVoidBehavior.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(0.01f)
                    );
                c.Next.Operand = 0.005f;
                c.GotoNext(MoveType.After,
                     x => x.MatchMul()
                    );
                c.EmitDelegate<Func<float, float>>(healPercent => healPercent + 0.005f);
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.MushroomVoid);
        }
    }
}
