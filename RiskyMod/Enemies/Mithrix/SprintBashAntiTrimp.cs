using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies.Mithrix
{
    public class SprintBashAntiTrimp
    {
        public static bool enabled = false;

        public SprintBashAntiTrimp()
        {
            if (!enabled) return;

            On.EntityStates.BrotherMonster.SprintBash.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (self.isAuthority && self.characterMotor && self.characterMotor.velocity.y > 0f)
                {
                    self.characterMotor.velocity.y = 0f;
                }
            };
        }
    }
}
