using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
namespace RiskyMod.Survivors.Loader
{
    public class LoaderPunchBrake
    {
        public static bool enabled = false;
        public static float stopCoefficient = 0.2f;
        public LoaderPunchBrake()
        {
            if (!enabled) return;
            On.EntityStates.Loader.BaseSwingChargedFist.OnMeleeHitAuthority += (orig, self) =>
            {
                orig(self);
                if (self.inputBank && self.inputBank.skill1.down)
                {
                    //self.characterMotor.disableAirControlUntilCollision = false;
                    //self.characterMotor.velocity *= stopCoefficient;
                    self.punchVelocity = Vector3.zero;
                    self.fixedAge = self.duration;
                }
            };

            On.EntityStates.Loader.BaseSwingChargedFist.OnExit += (orig, self) =>
            {
                orig(self);
                if (self.isAuthority && self.inputBank && self.inputBank.skill1.down)
                {
                    self.characterMotor.disableAirControlUntilCollision = false;
                    self.characterMotor.velocity *= stopCoefficient;
                }
            };

            IL.EntityStates.Loader.SwingZapFist.OnExit += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<CharacterMotor>("ApplyForce")
                    );
                c.Index -= 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Vector3, EntityStates.Loader.SwingZapFist, Vector3>>((force, self) =>
                {
                    if (self.inputBank && self.inputBank.skill1.down)
                    {
                        return Vector3.zero;
                    }
                    return force;
                });
            };
        }
    }
}
