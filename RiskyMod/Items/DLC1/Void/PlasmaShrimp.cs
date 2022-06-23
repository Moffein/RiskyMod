using MonoMod.Cil;
using System;
using RoR2;
using RoR2.Orbs;

namespace RiskyMod.Items.DLC1.Void
{
    public class PlasmaShrimp
    {
        public static bool enabled = true;
        public PlasmaShrimp()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "MissileVoid")
                    )
                    &&
                     c.TryGotoNext(
                     x => x.MatchLdcR4(0.4f)
                    ))
                {
                    c.Next.Operand = 0.24f;
                    c.Index += 4;
                    c.EmitDelegate<Func<float, float>>(damage =>
                    {
                        return damage + 0.16f;
                    });
                    if (RiskyMod.disableProcChains)
                    {
                        if (c.TryGotoNext(
                             x => x.MatchStfld<GenericDamageOrb>("procCoefficient")
                            ))
                        {
                            c.Index--;
                            c.Next.Operand = 0f;
                        }
                    }
                    error = false;
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: PlasmaShrimp IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.MissileVoid);
        }
    }
}
