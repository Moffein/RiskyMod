using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class SlowDownProjectilesModifyDamage
    {
        public static bool enabled = true;
        public SlowDownProjectilesModifyDamage()
        {
            if (!enabled) return;

            On.RoR2.Projectile.SlowDownProjectiles.OnTriggerEnter += (orig, self, other) =>
            {
                ProjectileDamage pd = other.GetComponent<ProjectileDamage>();
                if (pd)
                {
                    pd.damage *= self.slowDownCoefficient;
                }
                orig(self, other);
            };

            On.RoR2.Projectile.SlowDownProjectiles.OnTriggerExit += (orig, self, other) =>
            {
                ProjectileDamage pd = other.GetComponent<ProjectileDamage>();
                if (pd)
                {
                    pd.damage /= self.slowDownCoefficient;
                }
                orig(self, other);
            };
        }
    }
}
