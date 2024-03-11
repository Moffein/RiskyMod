using MonoMod.Cil;
using System;
using RoR2;

namespace RiskyMod.Survivors.Treebot
{
    public class SelfDamageTweaks
    {
        public static bool enabled = true;

        public SelfDamageTweaks()
        {
            if (!enabled) return;

            //Seed Barrage
            IL.EntityStates.Treebot.Weapon.FireMortar2.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(
                     x => x.MatchCallvirt<HealthComponent>("TakeDamage")
                    ))
                {
                    c.EmitDelegate<Func<DamageInfo, DamageInfo>>((damageInfo) =>
                    {
                        damageInfo.damageType &= ~DamageType.BypassArmor;
                        return damageInfo;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: TreeBot SelfDamageTweaks FireMortar2 IL Hook failed");
                }
            };

            //Bramble Volley does not have the BypassArmor damagetype.

            //Tangling Growth does not have the BypassArmor damagetype.
        }
    }
}
