using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using System;
using RoR2.Orbs;

namespace RiskyMod.Items.Uncommon
{
    public class Ukulele
    {
        public static bool enabled = true;
        public Ukulele()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "ChainLightning")
                    );

                //Add damage scaling
                /*c.GotoNext(
                    x => x.MatchLdcR4(0.8f)
                    );
                c.Index++;
                c.Emit(OpCodes.Ldloc, 11);  //Ukulele Count
                c.EmitDelegate<Func<float, int, float>>((origDamage, itemCount) =>
                {
                    return origDamage + origDamage * 0.3f * (itemCount - 1);
                });*/

                //Change proc coefficient
                c.GotoNext(
                    x => x.MatchLdcR4(0.2f)
                    );
                c.Index++;
                c.EmitDelegate<Func<float, float>> ((originalProcCoefficient) =>
                {
                    return RiskyMod.disableProcChains ? 0.1f : originalProcCoefficient;
                });

                //Remove range scaling
                c.GotoNext(
                    x => x.MatchStfld<RoR2.Orbs.LightningOrb>("range")
                    );
                c.EmitDelegate<Func<float, float>>((originalRange) =>
                {
                    return 5f;  //Added onto 20
                });

            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.ChainLightning);
        }
    }
}
