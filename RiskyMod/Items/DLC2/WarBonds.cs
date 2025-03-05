using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC2
{
    public class WarBonds
    {
        public static bool enabled = true;

        private static bool modifiedProc = false;

        public WarBonds()
        {
            if (!enabled) return;

            //TODO: Unnerf cost
        }
    }
}
