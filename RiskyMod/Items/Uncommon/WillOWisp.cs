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
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.ExplodeOnDeath);

            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "ExplodeOnDeath")
                    );

                //Change damage
                c.GotoNext(
                     x => x.MatchLdcR4(3.5f)
                    );
                c.Next.Operand = 4f;

                //Change damage per stack
                c.GotoNext(
                     x => x.MatchLdcR4(0.8f)
                    );
                c.Next.Operand = 0.6f;

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
                    return 16f;
                });

                //Disable falloff
                /*c.GotoNext(
                    x => x.MatchStfld<DelayBlast>("falloffModel")
                    );

                c.EmitDelegate<Func<BlastAttack.FalloffModel, BlastAttack.FalloffModel>>((model) =>
                {
                    return BlastAttack.FalloffModel.None;
                });*/
            };
        }
    }
}
