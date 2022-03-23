using RoR2;
using R2API;
using UnityEngine;
using MonoMod.Cil;

namespace RiskyMod.Items.Common
{
    public class Gasoline
    {
        public static bool enabled = true;
        public Gasoline()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += (il) =>
            {
                ILCursor c = new ILCursor(il);

                //Increase base range
                c.GotoNext(
                x => x.MatchLdcR4(8f)
                    );
                c.Next.Operand = 12f;

                //Remove range scaling
                c.GotoNext(
                     x => x.MatchLdcR4(4f)
                    );
                c.Next.Operand = 0f;
            };
		}
		private static void ModifyItem()
		{
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.IgniteOnKill);
		}
	}
}
