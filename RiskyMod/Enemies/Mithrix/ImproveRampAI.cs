using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies.Mithrix
{
    public class ImproveRampAI
    {
        public static bool enabled = true;

        public ImproveRampAI()
        {
            if (!enabled) return;

            On.EntityStates.BrotherMonster.SprintBash.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (self.isAuthority && self.characterMotor && self.characterMotor.velocity.y != 0)
                {
                    self.characterMotor.velocity.y = 0;
                }
            };
        }
    }
}
