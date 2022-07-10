using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies.Mobs
{
    public class Lemurian
    {
        public static bool enabled = true;

        public Lemurian()
        {
            if (!enabled) return;

            On.EntityStates.LemurianMonster.Bite.FixedUpdate += (orig, self) =>
            {
                bool hasBit = self.hasBit;
                orig(self);

                //Fire this when the hasBit state changes.
                if (self.isAuthority && hasBit != self.hasBit && self.characterDirection && SneedUtils.SneedUtils.ShouldRunInfernoChange())
                {
                    if (self.healthComponent)
                    {
                        self.healthComponent.TakeDamageForce(self.characterDirection.forward * 1600f, true, false);
                    }
                }
            };
        }
    }
}
