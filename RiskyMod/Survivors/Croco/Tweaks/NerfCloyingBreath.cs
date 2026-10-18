using R2API;
using R2API.Utils;
using RiskyMod.Survivors.Croco.Contagion;
using RoR2.AsyncManagement;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Croco.Tweaks
{
    public class NerfCloyingBreath
    {
        public static bool enabled;

        //public static GameObject crocoBreathModded;

        public NerfCloyingBreath()
        {
            if (!enabled) return;

            //Why does this not work, I fucking give up.
            /*crocoBreathModded = Addressables.LoadAssetAsync<GameObject>("5954b24e8404c1e4695488397f07fd14").WaitForCompletion().InstantiateClone("RiskyModCrocoBreathProjectile", true);
            crocoBreathModded.AddComponent<SuppressBlightComponent>();
            Content.Content.projectilePrefabs.Add(crocoBreathModded);*/

            //SneedUtils.SneedUtils.SetAddressableEntityStateField("784def337926e1e45b57f9bf662e7b1e", "projectilePrefab", crocoBreathModded);

            var  comp = Addressables.LoadAssetAsync<GameObject>("5954b24e8404c1e4695488397f07fd14").WaitForCompletion().AddComponent<SuppressBlightComponent>();
            if (!BlightReturns.enabled) comp.ticksPerBlight = 1;
        }

        [RequireComponent(typeof(ProjectileDamage), typeof(ProjectileDotZone))]
        public class SuppressBlightComponent : MonoBehaviour
        {
            public int ticksPerBlight = 3;  //How many damage ticks before a new blight stack is applied

            private ProjectileDamage projectileDamage;
            private ProjectileDotZone dotZone;

            private float previousResetStopwatch;
            private int tickCount;
            
            private void Awake()
            {
                projectileDamage = GetComponent<ProjectileDamage>();
            }

            private void Start()
            {
                bool shouldRunCode = projectileDamage.damageType.damageType == RoR2.DamageType.BlightOnHit;

                if (projectileDamage.damageType.HasModdedDamageType(ContagionPassive.contagionDamageType))
                {
                    shouldRunCode = true;

                    //Not even going to try to figure out how to balance Contagion's unique DoT for this.
                    projectileDamage.damageType.RemoveModdedDamageType(ContagionPassive.contagionDamageType);
                }

                if (!shouldRunCode)
                {
                    Destroy(this);
                    return;
                }

                projectileDamage.damageType.damageType = RoR2.DamageType.Generic;
                dotZone = GetComponent<ProjectileDotZone>();
                previousResetStopwatch = dotZone.resetStopwatch;
            }

            private void FixedUpdate()
            {
                if (dotZone.resetStopwatch < previousResetStopwatch)
                {
                    tickCount++;
                    if (tickCount >= ticksPerBlight)
                    {
                        projectileDamage.damageType.damageType = RoR2.DamageType.BlightOnHit;
                        tickCount = 0;
                    }
                    else
                    {
                        projectileDamage.damageType.damageType = RoR2.DamageType.Generic;
                    }
                }
                previousResetStopwatch = dotZone.resetStopwatch;
            }
        }
    }
}
