using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC1.Void
{
    public class Polylute
    {
        public static bool enabled = true;
        public Polylute()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchStfld<RoR2.Orbs.VoidLightningOrb>("totalStrikes")
                    ))
                {
                    c.EmitDelegate<Func<int, int>>(strikes =>
                    {
                        return (2 * strikes / 3) + 1;
                    });

                    //Change proc coefficient
                    if (RiskyMod.disableProcChains)
                    {
                        if(c.TryGotoNext(
                             x => x.MatchLdcR4(0.2f)
                            ))
                        {
                            c.Next.Operand = 0f;
                        }
                    }
                    error = false;
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Polylute IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2.DLC1Content.Items.ChainLightningVoid);
        }
    }
}
