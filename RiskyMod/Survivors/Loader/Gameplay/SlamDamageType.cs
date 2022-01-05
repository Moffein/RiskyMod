using MonoMod.Cil;
using R2API;
using RoR2;
using System;

namespace RiskyMod.Survivors.Loader
{
    public class SlamDamageType
    {
        public SlamDamageType()
        {
            IL.EntityStates.Loader.GroundSlam.DetonateAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>(orig =>
                {
                    orig.AddModdedDamageType(SharedDamageTypes.AntiFlyingForce);
                    return orig;
                });
            };
        }
    }
}
