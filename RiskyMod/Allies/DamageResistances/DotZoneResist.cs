using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RiskyMod.SharedHooks;

namespace RiskyMod.Allies
{
    public class DotZoneResist
    {
        public static bool enabled = true;
        public static DamageAPI.ModdedDamageType dotZoneDamage;

        public DotZoneResist()
        {
            dotZoneDamage = DamageAPI.ReserveDamageType();

            AddDotZoneDamageType(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MiniMushroom/SporeGrenadeProjectileDotZone.prefab").WaitForCompletion());
            AddDotZoneDamageType(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarExploder/LunarExploderProjectileDotZone.prefab").WaitForCompletion());

            if (!enabled) return;
            TakeDamage.ModifyInitialDamageActions += AddResist;
        }

        private void AddDotZoneDamageType(GameObject projectile)
        {
            DamageAPI.ModdedDamageTypeHolderComponent mdc = projectile.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            if (!mdc)
            {
                mdc = projectile.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            }
            mdc.Add(dotZoneDamage);
        }

        private static void AddResist(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (!self.body.isPlayerControlled
                && damageInfo.HasModdedDamageType(dotZoneDamage)
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && (self.body.inventory && self.body.inventory.GetItemCount(AllyItems.AllyMarkerItem) > 0) )
            {
                damageInfo.procCoefficient *= 0.25f;
                damageInfo.damage *= 0.25f;
            }
        }
    }
}
