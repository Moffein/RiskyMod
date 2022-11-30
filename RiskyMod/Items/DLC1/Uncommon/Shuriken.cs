using RoR2;
using RoR2.Projectile;
using R2API;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Uncommon
{
    public class Shuriken
    {
        public static bool enabled = true;
        public static GameObject projectilePrefab;
        public Shuriken()
        {
            if (!enabled) return;

            On.RoR2.PrimarySkillShurikenBehavior.FireShuriken += (orig, self) =>
            {
                Ray aimRay = self.GetAimRay();
                FireProjectileInfo fpi = new FireProjectileInfo
                {
                    projectilePrefab = self.projectilePrefab,
                    position = aimRay.origin,
                    rotation = Util.QuaternionSafeLookRotation(aimRay.direction) * self.GetRandomRollPitch(),
                    owner = self.gameObject,
                    damage = self.body.damage * (3f + 1f * (float)self.stack),
                    force = 0f,
                    crit = Util.CheckRoll(self.body.crit, self.body.master),
                    damageColorIndex = DamageColorIndex.Item,
                    damageTypeOverride = null,
                    speedOverride = -1f,
                    procChainMask = default(ProcChainMask)
                };
                fpi.procChainMask.AddProc(ProcType.Rings);
                ProjectileManager.instance.FireProjectile(fpi);
            };
        }
    }
}
