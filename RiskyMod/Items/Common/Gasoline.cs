using RoR2;
using R2API;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;

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
                bool error = true;

                ILCursor c = new ILCursor(il);

                //Increase base range
                if(c.TryGotoNext(
                x => x.MatchLdcR4(8f)
                    ))
                {
                    c.Next.Operand = 16f;

                    //Remove range scaling
                    if (c.TryGotoNext(
                     x => x.MatchLdcR4(4f)
                    ))
                    {
                        c.Next.Operand = 0f;

                        if (c.TryGotoNext(
                         x => x.MatchStfld(typeof(RoR2.InflictDotInfo), "damageMultiplier")
                        ))
                        {
                            c.Emit(OpCodes.Ldarg_1);
                            c.EmitDelegate<Func<float, int, float>>((damageMult, itemCount) =>
                            {
                                damageMult = 0.75f + 0.25f * itemCount;
                                return damageMult;
                            });
                            error = false;
                        }
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Gasoline IL Hook failed");
                }
            };
		}
		private static void ModifyItem()
		{
			HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.IgniteOnKill);
		}
	}
}
