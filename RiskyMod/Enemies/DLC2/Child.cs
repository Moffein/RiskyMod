using R2API;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.DLC2
{
    public class Child
    {
        public static bool enabled = true;

        public Child()
        {
            if (!enabled) return;
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/Child/EntityStates.ChildMonster.FireTrackingSparkBall.asset", "bombDamageCoefficient", "4");

            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Child/ChildTrackingSparkBall.prefab").WaitForCompletion().InstantiateClone("RiskyModChildTrackingSparkball", true);
            var pie = projectilePrefab.GetComponent<ProjectileExplosion>();
            pie.falloffModel = RoR2.BlastAttack.FalloffModel.SweetSpot;
            R2API.DamageAPI.ModdedDamageTypeHolderComponent mdc = projectilePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(SharedDamageTypes.SweetSpotModifier);
            Content.Content.projectilePrefabs.Add(projectilePrefab);
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/Child/EntityStates.ChildMonster.FireTrackingSparkBall.asset", "projectilePrefab", projectilePrefab);
        }
    }
}
