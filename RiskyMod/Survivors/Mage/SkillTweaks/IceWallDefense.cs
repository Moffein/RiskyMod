using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RiskyMod.Survivors.Mage.Components;
using RoR2.Projectile;

namespace RiskyMod.Survivors.Mage.SkillTweaks
{
    public class IceWallDefense
    {
        public static bool enabled = true;
        public static GameObject modifiedIceWallWalkerProjectile;
        public static GameObject modifiedIceWallPillarProjectile;

        public IceWallDefense()
        {
            if (!enabled) return;

            modifiedIceWallPillarProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIcewallPillarProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyModIceWallPillarProjectile", true);
            modifiedIceWallPillarProjectile.AddComponent<IceWallProjectileDeleter>();
            Content.Content.projectilePrefabs.Add(modifiedIceWallPillarProjectile);

            modifiedIceWallWalkerProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIcewallWalkerProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyModIceWallWalkerProjectile", true);
            ProjectileMageFirewallWalkerController walker = modifiedIceWallWalkerProjectile.GetComponent<ProjectileMageFirewallWalkerController>();
            if (walker) walker.firePillarPrefab = modifiedIceWallPillarProjectile;
            Content.Content.projectilePrefabs.Add(modifiedIceWallWalkerProjectile);

            EntityStates.RiskyMod.Mage.Weapon.PrepIceWall.projectilePrefab = modifiedIceWallWalkerProjectile;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.PrepWall", "projectilePrefab", modifiedIceWallWalkerProjectile);
        }
    }
}
