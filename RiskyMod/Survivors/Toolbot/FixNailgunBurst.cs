using RoR2;
using EntityStates;
using UnityEngine;
using EntityStates.Toolbot;

namespace RiskyMod.Survivors.Toolbot
{
    public class FixNailgunBurst
    {
        public static bool enabled = true;
        public FixNailgunBurst()
        {
            if (!enabled) return;
            On.EntityStates.Toolbot.FireNailgun.OnExit += (orig, self) =>
            {
                orig(self);
                //Manually fire shotgun burst if skill is cancelled.
                //self.outer.nextState doesn't seem to be working
                if (self.inputBank && self.inputBank.skill1.down)
                {
                    if (self.characterBody)
                    {
                        self.characterBody.SetSpreadBloom(1f, false);
                    }
                    Ray aimRay = self.GetAimRay();
                    self.FireBullet(self.GetAimRay(), NailgunFinalBurst.finalBurstBulletCount, BaseNailgunState.spreadPitchScale, BaseNailgunState.spreadYawScale);

                    Util.PlaySound(NailgunFinalBurst.burstSound, self.gameObject);
                    if (self.isAuthority)
                    {
                        float num = NailgunFinalBurst.selfForce * (self.characterMotor.isGrounded ? 0.5f : 1f) * self.characterMotor.mass;
                        self.characterMotor.ApplyForce(aimRay.direction * -num, false, false);
                    }
                    Util.PlaySound(BaseNailgunState.fireSoundString, self.gameObject);
                    Util.PlaySound(BaseNailgunState.fireSoundString, self.gameObject);
                    Util.PlaySound(BaseNailgunState.fireSoundString, self.gameObject);
                }
            };
        }
    }
}
