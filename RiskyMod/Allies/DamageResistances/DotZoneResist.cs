using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RiskyMod.SharedHooks;
using RoR2.Projectile;

namespace RiskyMod.Allies
{
    public class DotZoneResist
    {
        public static bool enabled = true;
        public static DamageAPI.ModdedDamageType dotZoneDamage;

        public DotZoneResist()
        {
            dotZoneDamage = DamageAPI.ReserveDamageType();

            if (!enabled) return;
            TakeDamage.ModifyInitialDamageNoAttackerActions += AddResist;
        }

        public static void AddDotZoneDamageType(GameObject projectile)
        {
            ProjectileDamage pd = projectile.GetComponent<ProjectileDamage>();
            if (pd)
            {
                pd.damageType.AddModdedDamageType(dotZoneDamage);
            }
        }

        private static void AddResist(DamageInfo damageInfo, HealthComponent self)
        {
            if (!self.body.isPlayerControlled
                && damageInfo.HasModdedDamageType(dotZoneDamage)
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && (self.body.inventory && self.body.inventory.GetItemCount(AllyItems.AllyMarkerItem) > 0) )
            {
                damageInfo.damage *= 0.1f * damageInfo.procCoefficient;
            }
        }
    }
}
