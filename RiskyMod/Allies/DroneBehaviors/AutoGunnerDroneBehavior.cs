using RoR2;
using RoR2.Orbs;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.CharacterAI;

namespace RiskyMod.Allies.DroneBehaviors
{
    public class AutoGunnerDroneBehavior : MonoBehaviour
    {
        public static float searchInterval = 1f;
        public static float maxActivationDistance = 60f;
        public static int shotsPerBarrage = 4;

        public static float damageCoefficient = 0.8f;
        public static float baseFireInterval = 0.15f;

        public static GameObject fireEffectPrefab;
        public static GameObject orbEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/DroneWeapons/ChainGunOrbEffect.prefab").WaitForCompletion();

        public SkillLocator skillLocator;
        public CharacterBody characterBody;
        public float searchStopwatch;
        public float cooldownStopwatch;
        public float fireStopwatch;
        public int shotsLoaded;
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

            shotsLoaded = shotsPerBarrage;
            searchStopwatch = 0f;
            cooldownStopwatch = 0f;
            fireStopwatch = 0f;
            fireInterval = baseFireInterval;
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

        public void FixedUpdate()
        {
            if (NetworkServer.active)// && !characterBody.isPlayerControlled
            {
                if (!firingBarrage)
                {
                    //Reloading takes priority
                    if (shotsLoaded < shotsPerBarrage)
                    {
                        cooldownStopwatch += Time.fixedDeltaTime;
                        if (cooldownStopwatch >= skillLocator.primary.CalculateFinalRechargeInterval())
                        {
                            cooldownStopwatch = 0f;
                            shotsLoaded = Mathf.FloorToInt(shotsPerBarrage * Mathf.Max(characterBody.attackSpeed, 1f));
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
                        FireBullet();
                    }
                }
            }
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

            return targetHurtBox != default;
        }

        public void FireBullet()
        {
            Ray aimRay = characterBody.inputBank ? characterBody.inputBank.GetAimRay() : default;
            if (targetHurtBox != default)
            {
                ChainGunOrb chainGunOrb = new ChainGunOrb(orbEffect);
                chainGunOrb.damageValue = characterBody.damage * AutoGunnerDroneBehavior.damageCoefficient * (AlliesCore.normalizeDroneDamage ? 1f : 1.2f);
                chainGunOrb.isCrit = characterBody.RollCrit();
                chainGunOrb.teamIndex = characterBody.teamComponent.teamIndex;
                chainGunOrb.attacker = base.gameObject;
                chainGunOrb.procCoefficient = 1f;
                chainGunOrb.procChainMask = default;//damageInfo.procChainMask;
                chainGunOrb.origin = aimRay.origin;
                chainGunOrb.speed = 600f;   //Drone Parts is 600f
                chainGunOrb.bouncesRemaining = 0;
                chainGunOrb.bounceRange = 30f;
                chainGunOrb.damageCoefficientPerBounce = 1f;
                chainGunOrb.targetsToFindPerBounce = 1;
                chainGunOrb.canBounceOnSameTarget = false;
                chainGunOrb.damageColorIndex = DamageColorIndex.Default;

                chainGunOrb.target = targetHurtBox;
                OrbManager.instance.AddOrb(chainGunOrb);

                if (AutoGunnerDroneBehavior.fireEffectPrefab)
                {
                    EffectManager.SimpleMuzzleFlash(AutoGunnerDroneBehavior.fireEffectPrefab, base.gameObject, "Muzzle", true);
                }
            }

            shotsLoaded--;
            if (shotsLoaded <= 0)
            {
                firingBarrage = false;
                targetHurtBox = default;
            }
        }
    }
}
