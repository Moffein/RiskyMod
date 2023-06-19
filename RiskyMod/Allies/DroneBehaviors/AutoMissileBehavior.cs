using RoR2;
using RoR2.CharacterAI;
using RoR2.Orbs;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Allies.DroneBehaviors
{
    public class AutoMissileBehavior : MonoBehaviour
    {
        public static float searchInterval = 1f;
        public static float maxActivationDistance = 80f;
        public static int missilesPerBarrage = 4;

        public static float damageCoefficient = 1.1f;
        public static float baseFireInterval = 0.15f;

        public SkillLocator skillLocator;
        public CharacterBody characterBody;
        public float searchStopwatch;
        public float cooldownStopwatch;
        public float fireStopwatch;
        public int missilesLoaded;
        public bool firingBarrage = false;
        public float fireInterval;
        public HurtBox targetHurtBox;

        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            skillLocator = base.GetComponent<SkillLocator>();
            if (!characterBody || !skillLocator)
            {
                Destroy(this);
                return;
            }

            missilesLoaded = missilesPerBarrage;
            searchStopwatch = 0f;
            cooldownStopwatch = 0f;
            fireStopwatch = 0f;
            fireInterval = baseFireInterval;
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active)// && !characterBody.isPlayerControlled
            {
                if (!firingBarrage)
                {
                    //Reloading takes priority
                    if (missilesLoaded < missilesPerBarrage)
                    {
                        cooldownStopwatch += Time.fixedDeltaTime;
                        if (cooldownStopwatch >= skillLocator.primary.CalculateFinalRechargeInterval())
                        {
                            cooldownStopwatch = 0f;
                            missilesLoaded = Mathf.FloorToInt(missilesPerBarrage * Mathf.Max(characterBody.attackSpeed, 1f));
                            fireInterval = baseFireInterval / characterBody.attackSpeed;
                        }
                    }
                    else
                    {
                        //Once loaded, search for enemies
                        searchStopwatch += Time.fixedDeltaTime;
                        if (searchStopwatch > searchInterval)
                        {
                            searchStopwatch -= searchInterval;
                            if (characterBody.teamComponent && AcquireTarget())
                            {
                                firingBarrage = true;
                                fireStopwatch = 0f;
                            }
                        }
                    }
                }
                else //Handle firing
                {
                    fireStopwatch += Time.fixedDeltaTime;
                    if (fireStopwatch >= fireInterval)
                    {
                        fireStopwatch -= fireInterval;
                        FireMissile();
                    }
                }
            }
        }

        public HurtBox GetAITarget()
        {
            if (!characterBody || characterBody.isPlayerControlled || !characterBody.master || characterBody.master.aiComponents == null || characterBody.master.aiComponents.Length <= 0) return null;

            HurtBox target = null;
            foreach (BaseAI ai in characterBody.master.aiComponents)
            {
                if (ai.currentEnemy != null)
                {
                    HurtBox hb = ai.currentEnemy.GetBestHurtBox(out bool hadLOS);
                    if (hb && hb.enabled)
                    {
                        TeamIndex myTeam = characterBody.teamComponent ? characterBody.teamComponent.teamIndex : TeamIndex.None;
                        if (myTeam != hb.teamIndex)
                        {
                            target = hb;
                            break;
                        }
                    }
                }
            }
            return target;
        }

        public bool AcquireTarget()
        {
            Ray aimRay = characterBody.inputBank ? characterBody.inputBank.GetAimRay() : default;

            HurtBox hb = GetAITarget();
            if (hb && hb.transform && (hb.transform.position - aimRay.origin).sqrMagnitude <= maxActivationDistance * maxActivationDistance)
            {
                targetHurtBox = hb;
            }
            else
            {
                BullseyeSearch search = new BullseyeSearch();

                search.teamMaskFilter = TeamMask.allButNeutral;
                search.teamMaskFilter.RemoveTeam(characterBody.teamComponent.teamIndex);

                search.filterByLoS = true;
                search.searchOrigin = aimRay.origin;
                search.sortMode = BullseyeSearch.SortMode.Angle;
                search.maxDistanceFilter = maxActivationDistance;
                search.maxAngleFilter = 360f;
                search.searchDirection = aimRay.direction;
                search.RefreshCandidates();

                targetHurtBox = search.GetResults().FirstOrDefault<HurtBox>();
            }

            return targetHurtBox != null;
        }

        public void FireMissile()
        {
            if (targetHurtBox != default)
            {
                Ray aimRay = characterBody.inputBank ? characterBody.inputBank.GetAimRay() : default;
                MicroMissileOrb missileOrb = new MicroMissileOrb();
                missileOrb.origin = aimRay.origin;
                missileOrb.damageValue = characterBody.damage * damageCoefficient * (AlliesCore.normalizeDroneDamage ? 1f : 0.857142857f);  // 12/14
                missileOrb.isCrit = characterBody.RollCrit();
                missileOrb.teamIndex = characterBody.teamComponent.teamIndex;
                missileOrb.attacker = base.gameObject;
                missileOrb.procChainMask = default;
                missileOrb.procCoefficient = 1f;
                missileOrb.damageColorIndex = DamageColorIndex.Default;
                missileOrb.target = targetHurtBox;
                missileOrb.speed = 25f; //Same as misisleprojectile. Default is 55f
                OrbManager.instance.AddOrb(missileOrb);

                if (EntityStates.Drone.DroneWeapon.FireMissileBarrage.effectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(EntityStates.Drone.DroneWeapon.FireMissileBarrage.effectPrefab, base.gameObject, "Muzzle", true);
                }

                //Technically animation is missing but no one will notice.
            }

            missilesLoaded--;
            if (missilesLoaded <= 0)
            {
                firingBarrage = false;
            }
        }
    }
}
