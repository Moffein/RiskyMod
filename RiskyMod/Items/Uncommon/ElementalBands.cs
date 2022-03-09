using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace RiskyMod.Items.Uncommon
{
    public class ElementalBands
    {
        public static bool enabled = true;

        public static float initialDamageCoefficientFire = 2.5f;
        public static float stackDamageCoefficientFire = 1.5f;

        public static float initialDamageCoefficientIce = 2f;
        public static float stackDamageCoefficientIce = 1.2f;

        public ElementalBands()
        {
            if (!enabled) return;
            On.RoR2.ItemCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.IceRing);
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.FireRing);
            };

            float initialDamageFire = initialDamageCoefficientFire - stackDamageCoefficientFire;
            float initialDamageIce = initialDamageCoefficientIce - stackDamageCoefficientIce;

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
                c.Next.Operand =stackDamageCoefficientIce;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + initialDamageIce;
                });

                //Jump to FireRing
                c.GotoNext(
                     x => x.MatchLdstr("Prefabs/Projectiles/FireTornado")
                    );

                //Change FireRing damage
                c.GotoNext(
                     x => x.MatchLdcR4(3f)
                    );
                c.Next.Operand = stackDamageCoefficientFire;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + initialDamageFire;
                });
            };
        }
    }
}
