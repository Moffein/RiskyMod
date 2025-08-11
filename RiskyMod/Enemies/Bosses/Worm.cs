using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using static RoR2.HurtBox;
using System.Linq;
using RoR2BepInExPack;

namespace RiskyMod.Enemies.Bosses
{
    public class Worm
    {
        public static bool enabled = true;

        public static GameObject MagmaWormProjectile;
        //public static GameObject ElectricWormProjectile;
        public static GameObject MagmaWormProjectileZone;
        //public static GameObject ElectricWormProjectileZone;

        public Worm()
        {
            if (!enabled) return;
            //ProjectileSetup();
            //AlwaysFireMeatballs(); //Laggy
            ProjectileSetup();

            GameObject magmaWorm = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormBody.prefab").WaitForCompletion();
            CharacterBody magmaWormBody = magmaWorm.GetComponent<CharacterBody>();
            magmaWormBody.baseDamage = 15f;
            magmaWormBody.levelDamage = 0.2f * magmaWormBody.baseDamage;

            GameObject electricWorm = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElectricWorm/ElectricWormBody.prefab").WaitForCompletion();

            SneedUtils.SneedUtils.SetPrioritizePlayers(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormMaster.prefab").WaitForCompletion());
            SneedUtils.SneedUtils.SetPrioritizePlayers(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElectricWorm/ElectricWormMaster.prefab").WaitForCompletion());

            ReduceFollowDelay(magmaWorm);
            ReduceFollowDelay(electricWorm);
        }

        private void ProjectileSetup()
        {
            MagmaWormProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaOrbProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyMod_MagmaWormProjectile", true);
            Content.Content.projectilePrefabs.Add(MagmaWormProjectile);
            ProjectileImpactExplosion pie = MagmaWormProjectile.GetComponent<ProjectileImpactExplosion>();
            pie.falloffModel = BlastAttack.FalloffModel.None;

            GameObject magmaWormPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormBody.prefab").WaitForCompletion();
            WormBodyPositions2 magmaWormController = magmaWormPrefab.GetComponent<WormBodyPositions2>();
            magmaWormController.meatballProjectile = MagmaWormProjectile;
        }

        //This causes the ground to constantly be filled with fire. Might not be suitable.
        private void ProjectileSetupUnused()
        {
            MagmaWormProjectileZone = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Molotov/MolotovProjectileDotZone.prefab").WaitForCompletion().InstantiateClone("RiskyMod_MagmaWormProjectileZone", true);
            Content.Content.projectilePrefabs.Add(MagmaWormProjectileZone);
            ProjectileDamage pd = MagmaWormProjectileZone.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            MagmaWormProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaOrbProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyMod_MagmaWormProjectile", true);
            Content.Content.projectilePrefabs.Add(MagmaWormProjectile);
            ProjectileImpactExplosion pie = MagmaWormProjectile.GetComponent<ProjectileImpactExplosion>();
            pie.fireChildren = true;
            pie.childrenProjectilePrefab = MagmaWormProjectileZone;
            pie.childrenDamageCoefficient = 1f;

            GameObject magmaWormPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormBody.prefab").WaitForCompletion();
            WormBodyPositions2 magmaWormController = magmaWormPrefab.GetComponent<WormBodyPositions2>();
            magmaWormController.meatballProjectile = MagmaWormProjectile;
        }

        private void AlwaysFireMeatballs()
        {
            On.EntityStates.MagmaWorm.SwitchStance.SetStanceParameters += (orig, self, leaping) =>
            {
                orig(self, leaping);
                if (!leaping)
                {
                    WormBodyPositions2 component = self.GetComponent<WormBodyPositions2>();
                    if (component)
                    {
                        component.shouldFireMeatballsOnImpact = true;
                    }
                }
            };
        }

        private void ReduceFollowDelay(GameObject bodyObject)
        {
            WormBodyPositions2 wbp = bodyObject.GetComponent<WormBodyPositions2>();
            if (wbp)
            {
                //Debug.Log("Follow Delay: " + wbp.followDelay);
                wbp.followDelay = 0.1f;
            }
        }
    }
}
