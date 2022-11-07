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
        public static float baseHealPercent = 0.012f;
        public static float stackHealPercent = 0.006f;
        public Dungus()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

			IL.RoR2.MushroomVoidBehavior.FixedUpdate += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdcR4(0.01f)
                    ))
                {
                    c.Next.Operand = stackHealPercent * 0.5f;
                    if(c.TryGotoNext(MoveType.After,
                         x => x.MatchMul()
                        ))
                    {
                        c.EmitDelegate<Func<float, float>>(healPercent => healPercent + (baseHealPercent - stackHealPercent) * 0.5f);
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Dungus IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.MushroomVoid);
        }
    }
}
