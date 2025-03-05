using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.Items.DLC2
{
    public class UnstableTransmitter
    {
        public static bool enabled = true;

        public UnstableTransmitter()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            IL.RoR2.TeleportOnLowHealthBehavior.TryProc += TeleportOnLowHealthBehavior_TryProc;
        }

        private void TeleportOnLowHealthBehavior_TryProc(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(0.35f)))
            {
                c.EmitDelegate<Func<float, float>>(x => 1f);
            }
            else
            {
                Debug.LogError("RiskyMod: Unstable Transmitter IL Hook failed.");
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC2Content.Items.TeleportOnLowHealth);
        }
    }
}
