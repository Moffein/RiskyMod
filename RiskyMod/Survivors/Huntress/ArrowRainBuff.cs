using RoR2;
using UnityEngine;
using RoR2.Projectile;
using R2API;
using RiskyMod.SharedHooks;
using System.Runtime.CompilerServices;
using EntityStates.RiskyMod.Huntress;
using MonoMod.Cil;
using System;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Huntress
{
    public class ArrowRainBuff
    {
        public static bool enabled = true;
        public static GameObject arrowRainObject;
        public static GameObject arrowRainScepterObject;

        public ArrowRainBuff()
        {
            if (!enabled) return;

            arrowRainObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRain.prefab").WaitForCompletion().InstantiateClone("RiskyModArrowRainProjectile", true);
            Content.Content.projectilePrefabs.Add(arrowRainObject);

            ProjectileDotZone arrowDotZone = arrowRainObject.GetComponent<ProjectileDotZone>();
            arrowDotZone.overlapProcCoefficient = 0.7f;
            arrowDotZone.damageCoefficient = 1f / 3f; //Adjusts DPS value to match the entitystate damagecoefficient

            HitBox hb = arrowRainObject.GetComponentInChildren<HitBox>();
            hb.transform.localPosition += 0.5f * Vector3.up;
            hb.transform.localScale = new Vector3(hb.transform.localScale.x, 2f * hb.transform.localScale.y, hb.transform.localScale.z);

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Huntress/EntityStates.Huntress.ArrowRain.asset", "damageCoefficient", "4.2");
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Huntress/EntityStates.Huntress.ArrowRain.asset", "projectilePrefab", ArrowRainBuff.arrowRainObject);

            IL.EntityStates.Huntress.ArrowRain.DoFireArrowRain += ArrowRain_DoFireArrowRain;
        }

        private void ArrowRain_DoFireArrowRain(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCallvirt<ProjectileManager>("FireProjectile")))
            {
                c.EmitDelegate<Func<DamageTypeCombo?, DamageTypeCombo?>>(combo =>
                {
                    if (combo.HasValue)
                    {
                        DamageTypeCombo modify = combo.Value;
                        modify.damageType |= DamageType.SlowOnHit;
                        modify.damageSource = DamageSource.Special;
                        modify.AddModdedDamageType(SharedDamageTypes.ProjectileRainForce);
                        return new DamageTypeCombo?(modify);
                    }
                    return combo;
                });
            }
            else
            {
                Debug.LogError("RiskyMod: ArrowRain_DoFireArrowRain IL Hook failed.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void ScepterProjectileSetup()
        {
            ArrowRainBuff.arrowRainScepterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/HuntressArrowRain.prefab").WaitForCompletion().InstantiateClone("RiskyModArrowRainScepterProjectile", true);
            Content.Content.projectilePrefabs.Add(ArrowRainBuff.arrowRainScepterObject);

            arrowRainScepterObject.transform.localScale = new Vector3(arrowRainScepterObject.transform.localScale.x * 1.5f,
                arrowRainScepterObject.transform.localScale.y,
                arrowRainScepterObject.transform.localScale.z * 1.5f);

            ProjectileDotZone arrowDotZone = ArrowRainBuff.arrowRainScepterObject.GetComponent<ProjectileDotZone>();
            arrowDotZone.overlapProcCoefficient = 0.7f;
            arrowDotZone.damageCoefficient = 1f / 3f;

            HitBox hb = ArrowRainBuff.arrowRainScepterObject.GetComponentInChildren<HitBox>();
            hb.transform.localPosition += 0.5f * Vector3.up;
            hb.transform.localScale = new Vector3(hb.transform.localScale.x, 2f * hb.transform.localScale.y, hb.transform.localScale.z);

            //Taken from https://github.com/DestroyedClone/AncientScepter/blob/master/AncientScepter/ScepterSkills/HuntressRain2.cs
            arrowRainScepterObject.GetComponent<ProjectileDamage>().damageType |= DamageType.IgniteOnHit;
            arrowRainScepterObject.GetComponent<ProjectileDotZone>().lifetime *= 1.5f;
            var fx = arrowRainScepterObject.transform.Find("FX");
            var afall = fx.Find("ArrowsFalling");
            afall.GetComponent<ParticleSystemRenderer>().material.SetVector("_TintColor", new Vector4(3f, 0.1f, 0.04f, 1.5f));
            var aimp = fx.Find("ImpaledArrow");
            aimp.GetComponent<ParticleSystemRenderer>().material.SetVector("_TintColor", new Vector4(3f, 0.1f, 0.04f, 1.5f));
            var radInd = fx.Find("RadiusIndicator");
            radInd.GetComponent<MeshRenderer>().material.SetVector("_TintColor", new Vector4(3f, 0.1f, 0.04f, 1.25f));
            var flash = fx.Find("ImpactFlashes");
            var psm = flash.GetComponent<ParticleSystem>().main;
            psm.startColor = new Color(1f, 0.7f, 0.4f);
            flash.GetComponent<ParticleSystemRenderer>().material.SetVector("_TintColor", new Vector4(3f, 0.1f, 0.04f, 1.5f));
            var flashlight = flash.Find("Point Light");
            flashlight.GetComponent<Light>().color = new Color(1f, 0.5f, 0.3f);
            flashlight.GetComponent<Light>().range = 15f;
            flashlight.gameObject.SetActive(true);

            ArrowRainScepter.projectilePrefab = arrowRainScepterObject;
        }

    }
}
