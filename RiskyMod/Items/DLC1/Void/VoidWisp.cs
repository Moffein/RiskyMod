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
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "ExplodeOnDeathVoid")
                    );

                //Disable Proc Coefficient
                if (RiskyMod.disableProcChains)
                {
                    c.GotoNext(
                        x => x.MatchStfld<DelayBlast>("position")
                        );
                    c.Index--;
                    c.EmitDelegate<Func<DelayBlast, DelayBlast>>((db) =>
                    {
                        db.procCoefficient = 0f;
                        return db;
                    });
                }

                //Disable Radius Scaling
                c.GotoNext(
                     x => x.MatchStfld<RoR2.DelayBlast>("radius")
                    );
                c.EmitDelegate<Func<float, float>>((oldRadius) =>
                {
                    return 12f;
                });
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.ExplodeOnDeathVoid);
        }
    }
}
