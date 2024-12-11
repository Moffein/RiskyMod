using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.DLC2
{
    public class Overspill
    {
        public static bool enabled = true;
        
        public Overspill()
        {
            if (!enabled) return;

            if (RiskyMod.disableProcChains)
            {
                IL.RoR2.MeteorAttackOnHighDamageBodyBehavior.DetonateRunicLensMeteor += MeteorAttackOnHighDamageBodyBehavior_DetonateRunicLensMeteor;
            }
        }

        private void MeteorAttackOnHighDamageBodyBehavior_DetonateRunicLensMeteor(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCallvirt<BlastAttack>("Fire")))
            {
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>(ba =>
                {
                    ba.procCoefficient = 0f;
                    return ba;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("RiskyMod: Overspill IL Hook failed.");
            }
        }
    }
}
