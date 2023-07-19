using RiskyMod.SharedHooks;
using RoR2;
using R2API;
using RiskyMod.Tweaks.CharacterMechanics;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies
{
    public class SuperAttackResist
    {
        public static bool enabled = true;

        public SuperAttackResist()
        {
            if (!enabled) return;
            TakeDamage.ModifyInitialDamageNoAttackerActions += AddResist;

            SetupResists();
        }

        private static void AddResist(DamageInfo damageInfo, HealthComponent self)
        {
            if (!self.body.isPlayerControlled
                && (damageInfo.HasModdedDamageType(SharedDamageTypes.ResistedByAllies))
                && (self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                && (self.body.inventory && self.body.inventory.GetItemCount(AllyItems.AllyMarkerItem) > 0))
            {
                damageInfo.procCoefficient *= 0.25f;
                damageInfo.damage *= 0.25f;
            }
        }

        private void SetupResists()
        {
            SneedUtils.SneedUtils.AddModdedDamageTypeToProjectile(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSunderWave.prefab").WaitForCompletion(), SharedDamageTypes.ResistedByAllies);
            SneedUtils.SneedUtils.AddModdedDamageTypeToProjectile(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSunderWave, Energized.prefab").WaitForCompletion(), SharedDamageTypes.ResistedByAllies);
        }
    }
}
