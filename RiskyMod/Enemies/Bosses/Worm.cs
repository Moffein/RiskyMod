using RoR2;
using R2API;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using static RoR2.HurtBox;
using System.Linq;

namespace RiskyMod.Enemies.Bosses
{
    public class Worm
    {
        public static bool enabled = true;

        public static BodyIndex MagmaWormIndex;
        public static BodyIndex ElectricWormIndex;

        public static GameObject MagmaWormProjectile;
        public static GameObject ElectricWormProjectile;
        public static GameObject MagmaWormProjectileZone;
        public static GameObject ElectricWormProjectileZone;

        public Worm()
        {
            if (!enabled) return;

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                MagmaWormIndex = BodyCatalog.FindBodyIndex("MagmaWormBody");
                ElectricWormIndex = BodyCatalog.FindBodyIndex("ElectricWormBody");
                if (MagmaWormIndex != BodyIndex.None) PrioritizePlayers.prioritizePlayersList.Add(MagmaWormIndex);
                if (ElectricWormIndex != BodyIndex.None) PrioritizePlayers.prioritizePlayersList.Add(ElectricWormIndex);
            };

            //ProjectileSetup();
            //AlwaysFireMeatballs(); //Laggy

            GameObject magmaWorm = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/MagmaWorm/MagmaWormBody.prefab").WaitForCompletion();
            GameObject electricWorm = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElectricWorm/ElectricWormBody.prefab").WaitForCompletion();

            FixHitbox(magmaWorm);
            FixHitbox(electricWorm);

            ReduceFollowDelay(magmaWorm);
            ReduceFollowDelay(electricWorm);
        }

        //This causes the ground to constantly be filled with fire. Might not be suitable.
        private void ProjectileSetup()
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

        private void FixHitbox(GameObject bodyPrefab)
        {
            ModelLocator ml = bodyPrefab.GetComponent<ModelLocator>();
            if (!ml || !ml.modelTransform || !ml.modelTransform.gameObject) return;

            HurtBoxGroup hbg = ml.modelTransform.gameObject.GetComponent<HurtBoxGroup>();
            if (!hbg) return;

            var allHurtboxes = ml.modelTransform.gameObject.GetComponentsInChildren<HurtBox>();

            foreach (HurtBox hb in allHurtboxes)
            {
                if (!hbg.hurtBoxes.Contains(hb))
                {
                    var list = hbg.hurtBoxes.ToList();
                    list.Add(hb);
                    hbg.hurtBoxes = list.ToArray();
                }
            }
        }
    }
}
