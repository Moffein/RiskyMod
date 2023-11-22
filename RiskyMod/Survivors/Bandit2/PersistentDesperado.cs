using MonoMod.Cil;
using RiskyMod.Survivors.Bandit2.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Bandit2
{
    public class PersistentDesperado
    {
        public static bool enabled = true;
        public static float damagePerBuff = 0.01f;

        public PersistentDesperado()
        {
            if (!enabled) return;

            IL.EntityStates.Bandit2.Weapon.FireSidearmSkullRevolver.ModifyBullet += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdcR4(0.1f)
                    ))
                {
                    c.Next.Operand = damagePerBuff;
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: PersistentDesperado IL Hook failed");
                }
            };
        }

        public static float GetDesperadoMult(CharacterBody cb)
        {
            int buffCount = cb.GetBuffCount(RoR2Content.Buffs.BanditSkull.buffIndex);
            if (PersistentDesperado.enabled)
            {
                return 1f + buffCount * damagePerBuff;
            }
            else
            {
                return 1f + buffCount * 0.1f;
            }
        }
    }
}
