using RoR2;
using R2API;
using RoR2.Projectile;
using UnityEngine;
using R2API.Utils;

namespace RiskyMod.Survivors.Mage
{
    public class ScepterHandler
    {
        public static DamageAPI.ModdedDamageType FlamethrowerScepterDamage;
        private static GameObject flamethrowerScepterProjectile;

        public static void InitFlamethrowerScepter()
        {
            if (!RiskyMod.ScepterPluginLoaded) return;
            FlamethrowerScepterDamage = DamageAPI.ReserveDamageType();
            flamethrowerScepterProjectile = BuildFlameProjectile();
            SharedHooks.OnHitAll.HandleOnHitAllActions += FlamethrowerDamage;
        }

        //Duplicated from https://github.com/DestroyedClone/AncientScepter/blob/master/AncientScepter/ScepterSkills/ArtificerFlamethrower2.cs
        //Todo: figure out how to just access that prefab
        private static GameObject BuildFlameProjectile()
        {
            GameObject projCloud = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/BeetleQueenAcid").InstantiateClone("AncientScepterMageFlamethrowerCloud");
            var pdz = projCloud.GetComponent<ProjectileDotZone>();
            pdz.lifetime = 10f;
            pdz.impactEffect = null;
            pdz.fireFrequency = 2f;
            var fxObj = projCloud.transform.Find("FX");
            fxObj.Find("Spittle").gameObject.SetActive(false);
            fxObj.Find("Decal").gameObject.SetActive(false);
            fxObj.Find("Gas").gameObject.SetActive(false);
            foreach (var x in fxObj.GetComponents<AnimateShaderAlpha>()) { x.enabled = false; }
            var fxcloud = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("prefabs/FireTrail").GetComponent<DamageTrail>().segmentPrefab, fxObj.transform);
            var psmain = fxcloud.GetComponent<ParticleSystem>().main;
            psmain.duration = 10f;
            psmain.gravityModifier = -0.05f;
            var pstartx = psmain.startSizeX;
            pstartx.constantMin *= 0.75f;
            pstartx.constantMax *= 0.75f;
            var pstarty = psmain.startSizeY;
            pstarty.constantMin *= 0.75f;
            pstarty.constantMax *= 0.75f;
            var pstartz = psmain.startSizeZ;
            pstartz.constantMin *= 0.75f;
            pstartz.constantMax *= 0.75f;
            var pslife = psmain.startLifetime;
            pslife.constantMin = 0.75f;
            pslife.constantMax = 1.5f;
            fxcloud.GetComponent<DestroyOnTimer>().enabled = false;
            fxcloud.transform.localPosition = Vector3.zero;
            fxcloud.transform.localScale = Vector3.one;
            var psshape = fxcloud.GetComponent<ParticleSystem>().shape;
            psshape.shapeType = ParticleSystemShapeType.Sphere;
            psshape.scale = Vector3.one * 1.5f;
            var psemit = fxcloud.GetComponent<ParticleSystem>().emission;
            psemit.rateOverTime = AncientScepter.AncientScepterItem.artiFlamePerformanceMode ? 4f : 20f;
            var lightObj = fxObj.Find("Point Light").gameObject;
            if (AncientScepter.AncientScepterItem.artiFlamePerformanceMode)
            {
                UnityEngine.Object.Destroy(lightObj);
            }
            else
            {
                var lightCpt = lightObj.GetComponent<Light>();
                lightCpt.color = new Color(1f, 0.5f, 0.2f);
                lightCpt.intensity = 3.5f;
                lightCpt.range = 5f;
            }

            Content.Content.projectilePrefabs.Add(projCloud);
            return projCloud;
        }

        private static void FlamethrowerDamage(GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            if (damageInfo.HasModdedDamageType(ScepterHandler.FlamethrowerScepterDamage))
            {
                ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                {
                    crit = damageInfo.crit,
                    damage = damageInfo.damage,
                    damageColorIndex = DamageColorIndex.Default,
                    damageTypeOverride = DamageType.PercentIgniteOnHit,
                    force = 0f,
                    owner = damageInfo.attacker,
                    position = damageInfo.position,
                    procChainMask = damageInfo.procChainMask,
                    projectilePrefab = flamethrowerScepterProjectile,
                    target = null
                });
            }
        }
    }
}
