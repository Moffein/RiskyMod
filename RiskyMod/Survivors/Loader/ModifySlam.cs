using System;
using MonoMod.Cil;
using RoR2;
using R2API;

namespace RiskyMod.Survivors.Loader
{
    public class ModifySlam
    {
        public ModifySlam()
        {
            IL.EntityStates.Loader.GroundSlam.DetonateAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>(blastAttack =>
                {
                    blastAttack.AddModdedDamageType(SharedDamageTypes.InterruptOnHit);
                    blastAttack.AddModdedDamageType(SharedDamageTypes.AntiFlyingForce);
                    return blastAttack;
                });
            };
        }
    }
}
