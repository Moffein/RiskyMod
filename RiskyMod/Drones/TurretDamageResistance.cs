using RoR2;
using UnityEngine;

namespace RiskyMod.Drones
{
    public class TurretDamageResistance
    {
        public static bool enabled = true;
        public TurretDamageResistance()
        {
            if (!enabled) return;
            On.RoR2.HealthComponent.TakeDamage += (orig, self, di) =>
            {
                if (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                {
                    if (DronesCore.IsTurretAlly(self.body.bodyIndex))
                    {
                        di.damage *= Mathf.Min(Mathf.Max(0.1f, di.procCoefficient), 1f);
                    }
                }
                orig(self, di);
            };
        }
    }
}
