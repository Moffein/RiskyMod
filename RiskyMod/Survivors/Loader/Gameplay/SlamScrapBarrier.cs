using EntityStates.Loader;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Survivors.Loader
{
    public class SlamScrapBarrier
    {
        public SlamScrapBarrier()
        {
            /*On.EntityStates.Loader.BaseSwingChargedFist.OnMeleeHitAuthority += (orig, self) =>
            {
                orig(self);
                if (self.healthComponent)
                {
                    self.healthComponent.AddBarrierAuthority(LoaderMeleeAttack.barrierPercentagePerHit * self.healthComponent.fullBarrier);
                }
            };*/

            IL.EntityStates.Loader.GroundSlam.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchCall<GroundSlam>("DetonateAuthority")
                    ))
                {
                    c.Index++;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<BlastAttack.Result, EntityStates.Loader.GroundSlam, BlastAttack.Result>>((result, self) =>
                    {
                        if (result.hitCount > 0)
                        {
                            if (self.healthComponent)
                            {
                                self.healthComponent.AddBarrierAuthority(LoaderMeleeAttack.barrierPercentagePerHit * self.healthComponent.fullBarrier);
                            }
                        }
                        return result;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Loader SlamScrapBarrier IL Hook failed");
                }
            };
        }
    }
}
