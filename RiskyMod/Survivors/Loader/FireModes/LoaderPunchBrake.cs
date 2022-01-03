using UnityEngine;
namespace RiskyMod.Survivors.Loader
{
    public class LoaderPunchBrake
    {
        public static bool enabled = false;
        public LoaderPunchBrake()
        {
            if (!enabled) return;
            On.EntityStates.Loader.BaseSwingChargedFist.OnMeleeHitAuthority += (orig, self) =>
            {
                orig(self);
                if (self.inputBank && self.inputBank.skill1.down)
                {
                    self.characterMotor.disableAirControlUntilCollision = false;
                    self.characterMotor.velocity = Vector3.zero;
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
                    self.characterMotor.velocity = Vector3.zero;
                }
            };
        }
    }
}
