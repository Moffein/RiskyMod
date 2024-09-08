using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.Survivors.Croco;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Survivors.Croco.Tweaks
{
    public class UtilityKnockdown
    {
        public static bool enabled = true;

        public UtilityKnockdown()
        {
            if (!enabled) return;
            IL.EntityStates.Croco.BaseLeap.DetonateAuthority += BaseLeap_DetonateAuthority;
        }

        private void BaseLeap_DetonateAuthority(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(
                 x => x.MatchCallvirt<BlastAttack>("Fire")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<BlastAttack, EntityStates.Croco.BaseLeap, BlastAttack>>((blastAttack, self) =>
                {
                    blastAttack.AddModdedDamageType(SharedDamageTypes.AntiFlyingForce);
                    return blastAttack;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("RiskyMod: Croco UtilityKnockdown BaseLeap IL Hook failed");
            }
        }
    }
}
