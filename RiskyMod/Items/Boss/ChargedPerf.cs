using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Items.Boss
{
    public class ChargedPerf
    {
        public static bool enabled = true;
        public static float initialDamageCoefficient = 5f;
        public static float stackDamageCoefficient = 3f;
        public ChargedPerf()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.LightningStrikeOnHit);

            float initialDamage = initialDamageCoefficient - stackDamageCoefficient;

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
                c.Next.Operand = ChargedPerf.stackDamageCoefficient;
                c.Index += 4;
                c.EmitDelegate<Func<float, float>>((damageCoefficient) =>
                {
                    return damageCoefficient + initialDamage;
                });

                if (RiskyMod.disableProcChains)
                {
                    c.GotoNext(
                        x => x.MatchCallvirt<RoR2.Orbs.OrbManager>("AddOrb")
                        );
                    c.EmitDelegate<Func<RoR2.Orbs.SimpleLightningStrikeOrb, RoR2.Orbs.SimpleLightningStrikeOrb>>((orb) =>
                    {
                        orb.procCoefficient = 0f;
                        return orb;
                    });
                }
            };
        }
    }
}
