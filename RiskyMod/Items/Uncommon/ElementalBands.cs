using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace Risky_Mod.Items.Uncommon
{
    public class ElementalBands
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "IceRing")
                    );

                //Change IceRing damage
                c.GotoNext(
                     x => x.MatchLdcR4(2.5f)
                    );
                c.Next.Operand = 1.2f;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + 0.8f;
                });

                //Jump to FireRing
                c.GotoNext(
                     x => x.MatchLdstr("Prefabs/Projectiles/FireTornado")
                    );

                //Change FireRing damage
                c.GotoNext(
                     x => x.MatchLdcR4(3f)
                    );
                c.Next.Operand = 1.5f;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + 1f;
                });
            };

            LanguageAPI.Add("ITEM_ICERING_DESC", "Hits that deal <style=cIsDamage>more than 400% damage</style> also blasts enemies with a <style=cIsDamage>runic ice blast</style>, <style=cIsUtility>slowing</style> them by <style=cIsUtility>80%</style> for <style=cIsUtility>3s</style> <style=cStack>(+3s per stack)</style> and dealing <style=cIsDamage>200%</style> <style=cStack>(+120% per stack)</style> TOTAL damage. Recharges every <style=cIsUtility>10</style> seconds.");
            LanguageAPI.Add("ITEM_FIRERING_DESC", "Hits that deal <style=cIsDamage>more than 400% damage</style> also blasts enemies with a <style=cIsDamage>runic flame tornado</style>, dealing <style=cIsDamage>250%</style> <style=cStack>(+150% per stack)</style> TOTAL damage over time. Recharges every <style=cIsUtility>10</style> seconds.");
        }
    }
}
