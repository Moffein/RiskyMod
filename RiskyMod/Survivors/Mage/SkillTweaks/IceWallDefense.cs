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

            DefenseMatrixManager.enabled = true;

            modifiedIceWallPillarProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIcewallPillarProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyModIceWallPillarProjectile", true);
            modifiedIceWallPillarProjectile.AddComponent<IceWallDefenseComponent>();

            //Increase size or else projectiles go through the cracks
            BoxCollider pillarCollider = modifiedIceWallPillarProjectile.GetComponent<BoxCollider>();
            pillarCollider.size = new Vector3(2.5f, 2.5f, 7f);
            //Debug.Log("Collider Size: " + pillarCollider.size);  //1, 1, 6.1
            //Debug.Log("Transform Scale: " + modifiedIceWallPillarProjectile.transform.localScale);  //1, 1, 1

            modifiedIceWallPillarProjectile.gameObject.layer = LayerIndex.entityPrecise.intVal; //Needs to be on this layer to work with DefenseMatrixManager
            pillarCollider.enabled = false; //Gets toggled via DefenseMatrixManager, disabled by default so it doesnt block collision.

            Content.Content.projectilePrefabs.Add(modifiedIceWallPillarProjectile);

            modifiedIceWallWalkerProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageIcewallWalkerProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyModIceWallWalkerProjectile", true);
            ProjectileMageFirewallWalkerController walker = modifiedIceWallWalkerProjectile.GetComponent<ProjectileMageFirewallWalkerController>();
            if (walker) walker.firePillarPrefab = modifiedIceWallPillarProjectile;
            Content.Content.projectilePrefabs.Add(modifiedIceWallWalkerProjectile);

            EntityStates.RiskyMod.Mage.Weapon.PrepIceWall.projectilePrefab = modifiedIceWallWalkerProjectile;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Mage.Weapon.PrepWall", "projectilePrefab", modifiedIceWallWalkerProjectile);

            //Build deletion effect
            GameObject deletionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXFrozen.prefab").WaitForCompletion().InstantiateClone("RiskyModIceWallDeletionEffect", false);
            EffectComponent ec = deletionEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_captain_drone_zap";

            Content.Content.effectDefs.Add(new EffectDef(deletionEffect));
            Survivors.Mage.Components.IceWallDefenseComponent.projectileDeletionEffectPrefab = deletionEffect;
        }
    }
}
