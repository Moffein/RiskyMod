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
                int hookCount = 1;
                int actual = 0;
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "ExplodeOnDeath")
                    ))
                {
                    //Disable Proc Coefficient
                    if (RiskyMod.disableProcChains)
                    {
                        hookCount++;
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
                            actual++;
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
                        actual++;
                    }

                    if (RiskyMod.disableProcChains)
                    {
                        hookCount++;
                        if (c.TryGotoNext(
                            x => x.MatchStfld<DelayBlast>("falloffModel")
                            ))
                        {
                            c.EmitDelegate<Func<BlastAttack.FalloffModel, BlastAttack.FalloffModel>>((f) =>
                            {
                                return BlastAttack.FalloffModel.None;
                            });
                            actual++;
                        }
                    }
                }

                if (hookCount != actual)
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
