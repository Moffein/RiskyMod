using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace Risky_Mod.Items.Boss
{
    public class ChargedPerf
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            LanguageAPI.Add("ITEM_FIREBALLSONHIT_DESC", "<style=cIsDamage>10%</style> chance on hit to call forth <style=cIsDamage>3 magma balls</style> from an enemy, dealing <style=cIsDamage>300%</style> <style=cStack>(+180% per stack)</style> damage each.");

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "LightningStrikeOnHit")
                    );

                c.GotoNext(
                    x => x.MatchLdfld<DamageInfo>("damage")
                    );
                c.Index += 3;
                c.Next.Operand = 3f;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + 2f;
                });

                if (RiskyMod.disableProcChains)
                {
                    c.GotoNext(
                        x => x.MatchStfld<RoR2.Orbs.GenericDamageOrb>("procCoefficient")
                        );
                    c.Index--;
                    c.Next.Operand = 0f;
                }
            };

            //Effect handled in SharedHooks.OnHitEnemy
        }
    }
}
