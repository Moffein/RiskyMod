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
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.BarrageOnBossBehaviour.UpdateExtraMissileMoneyCount += BarrageOnBossBehaviour_UpdateExtraMissileMoneyCount;
        }

        private void BarrageOnBossBehaviour_UpdateExtraMissileMoneyCount(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdcI4(50)))
            {
                c.EmitDelegate<Func<int, int>>(x => 25);
            }
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC2Content.Items.BarrageOnBoss);
        }
    }
}
