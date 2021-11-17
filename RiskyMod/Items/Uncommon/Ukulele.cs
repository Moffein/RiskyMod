using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using System;

namespace RiskyMod.Items.Uncommon
{
    public class Ukulele
    {
        public static bool enabled = true;
        public Ukulele()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.ChainLightning);

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "ChainLightning")
                    );

                //Remove proc coefficient
                c.GotoNext(
                    x => x.MatchLdcR4(0.2f)
                    );
                c.Index++;
                c.EmitDelegate<Func<float, float>> ((originalProcCoefficient) =>
                {
                    return RiskyMod.disableProcChains ? 0f : originalProcCoefficient;
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

            //LanguageAPI.Add("ITEM_CHAINLIGHTNING_DESC", "<style=cIsDamage>25%</style> chance to fire <style=cIsDamage>chain lightning</style> for <style=cIsDamage>80%</style> TOTAL damage on up to <style=cIsDamage>3 <style=cStack>(+2 per stack)</style></style> targets within <style=cIsDamage>25m</style>.");
        }
    }
}
