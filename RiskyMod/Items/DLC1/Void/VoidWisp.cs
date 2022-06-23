using MonoMod.Cil;
using System;
using RoR2;

namespace RiskyMod.Items.DLC1.Void
{
    public class VoidWisp
    {
        public static bool enabled = true;
        public VoidWisp()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "ExplodeOnDeathVoid")
                    ))
                {
                    //Disable Proc Coefficient
                    if (RiskyMod.disableProcChains)
                    {
                        if(c.TryGotoNext(
                            x => x.MatchStfld<DelayBlast>("position")
                            ))
                        {
                            c.Index--;
                            c.EmitDelegate<Func<DelayBlast, DelayBlast>>((db) =>
                            {
                                db.procCoefficient = 0f;
                                return db;
                            });
                        }
                    }

                    //Disable Radius Scaling
                    if(c.TryGotoNext(
                         x => x.MatchStfld<RoR2.DelayBlast>("radius")
                        ))
                    {
                        c.EmitDelegate<Func<float, float>>((oldRadius) =>
                        {
                            return 12f;
                        });
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: VoidWisp IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.ExplodeOnDeathVoid);
        }
    }
}
