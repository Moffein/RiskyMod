using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using System;

namespace RiskyMod.Items.Uncommon
{
    public class WillOWisp
    {
        public static bool enabled = true;
        public WillOWisp()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "ExplodeOnDeath")
                    ))
                {
                    //Disable Proc Coefficient
                    if (RiskyMod.disableProcChains)
                    {
                        if (c.TryGotoNext(
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
                            return 16f;
                        });
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: WillOWisp IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.ExplodeOnDeath);
        }
    }
}
