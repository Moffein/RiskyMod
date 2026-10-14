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
        public static float baseHealPercent = 0.006f;   //0.012 - 0.006
        public static float stackHealPercent = 0.006f;
        public Dungus()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

			IL.RoR2.MushroomVoidBehavior.FixedUpdate += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld<MushroomVoidBehavior>("healPercentagePerStack")))
                {
                    c.EmitDelegate<Func<float, float>>(f => stackHealPercent);
                    if (c.TryGotoNext(MoveType.After,
                         x => x.MatchMul()
                        ))
                    {
                        c.EmitDelegate<Func<float, float>>(healPercent => healPercent + baseHealPercent);
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
