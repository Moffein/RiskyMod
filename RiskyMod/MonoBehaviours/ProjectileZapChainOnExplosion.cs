using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine;

namespace RiskyMod.MonoBehaviours
{
    [RequireComponent(typeof(ProjectileController), typeof(ProjectileExplosion), typeof(ProjectileDamage))]
    public class ProjectileZapChainOnExplosion : MonoBehaviour
    {
        //Add the hook for firing this
        private static bool initialized = false;
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            On.RoR2.Projectile.ProjectileExplosion.OnBlastAttackResult += (orig, self, blastAttack, result) =>
            {
                orig(self, blastAttack, result);
                ProjectileZapChainOnExplosion pzc = self.gameObject.GetComponent<ProjectileZapChainOnExplosion>();
                if (pzc)
                {
                    if (!pzc.requireHit || result.hitCount > 0)
                    {
                        pzc.Fire();
                    }
                }
            };
        }

        public void Awake()
        {
            projectileController = base.GetComponent<ProjectileController>();
            projectileDamage = base.GetComponent<ProjectileDamage>();
            projectileExplosion = base.GetComponent<ProjectileExplosion>();
            moddedDamageTypeHolderComponent = base.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
        }

        public void Fire()
        {
            List<HealthComponent> bouncedObjects = new List<HealthComponent>();

            //Need to individually find all targets for the first bounce.
            for (int i = 0; i < initialTargets; i++)
            {
                LightningOrb lightning = new LightningOrb
                {
                    bouncedObjects = bouncedObjects,
                    attacker = projectileController.owner,
                    inflictor = projectileController.owner,
                    damageValue = projectileDamage.damage * damageCoefficient,
                    procCoefficient = procCoefficient,
                    teamIndex = projectileController.teamFilter ? projectileController.teamFilter.teamIndex : TeamIndex.None,
                    isCrit = projectileDamage.crit,
                    procChainMask = default,
                    lightningType = LightningOrb.LightningType.Ukulele,
                    damageColorIndex = DamageColorIndex.Default,
                    bouncesRemaining = maxBounces,
                    targetsToFindPerBounce = 2,
                    range = range,
                    origin = base.transform.position,
                    damageType = projectileDamage.damageType,
                    speed = 120f
                };

                if (moddedDamageTypeHolderComponent)
                {
                    moddedDamageTypeHolderComponent.CopyTo(lightning);
                }

                HurtBox hurtBox = lightning.PickNextTarget(base.transform.position);

                //Fire orb if HurtBox is found.
                if (hurtBox)
                {
                    lightning.target = hurtBox;
                    OrbManager.instance.AddOrb(lightning);
                    lightning.bouncedObjects.Add(hurtBox.healthComponent);
                }
            }
        }

        private DamageAPI.ModdedDamageTypeHolderComponent moddedDamageTypeHolderComponent;
        private ProjectileController projectileController;
        private ProjectileDamage projectileDamage;
        private ProjectileExplosion projectileExplosion;

        public float damageCoefficient;
        public float procCoefficient;
        public float range;
        public int initialTargets;
        public int targetsPerBounce;
        public int maxBounces;
        public bool requireHit;
    }
}
