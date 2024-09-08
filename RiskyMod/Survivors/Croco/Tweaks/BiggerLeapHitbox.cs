using EntityStates.Croco;

namespace RiskyMod.Survivors.Croco.Tweaks
{
    public class BiggerLeapHitbox
    {
        public static bool enabled = true;
        public BiggerLeapHitbox()
        {
            if (!enabled) return;
            On.EntityStates.Croco.BaseLeap.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (self.isAuthority && self.characterMotor)
                {
                    if (!(self.detonateNextFrame || self.characterMotor.Motor.GroundingStatus.IsStableOnGround && !self.characterMotor.Motor.LastGroundingStatus.IsStableOnGround)
                    && self.fixedAge >= BaseLeap.minimumDuration)
                    {
                        if (SneedUtils.SneedUtils.IsEnemyInSphere(4f, self.characterBody.footPosition, self.GetTeam(), true))
                        {
                            self.detonateNextFrame = true;
                        }
                    }
                }
            };
        }
    }
}
