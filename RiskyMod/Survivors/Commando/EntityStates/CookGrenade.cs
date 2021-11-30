using R2API.Networking;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RiskyMod.Commando
{
    public class CookGrenade : BaseState
    {
        public static float totalFuseTime = 3f;
        public static string beepSoundString = "Play_commando_M2_grenade_beep";

        //Things that happen when you overcook
        public static float damageCoefficient = 12f;
        public static float force = 2000f;
        public static float selfHPDamagePercent = 0.6f; //Remove this fraction of combined HP
        public static float selfForce = 4500f;
        public static GameObject overcookExplosionEffectPrefab;
        public static GameObject grenadeIndicatorPrefab = Resources.Load<GameObject>("prefabs/effects/muzzleflashes/MuzzleflashFMJ");
        public static float selfBlastRadius = 14f;

        private float stopwatch;
        private bool grenadeThrown = false;
        private Animator animator;
        private Transform rightHand;

        public float radiusInternal;
        public float selfForceInternal;

        public virtual void LoadStats()
        {
            radiusInternal = selfBlastRadius;
            selfForceInternal = selfForce;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            LoadStats();
            animator = base.GetModelAnimator();
            if (animator)
            {
                base.PlayAnimation("Gesture, Additive", "ThrowGrenade", "FireFMJ.playbackRate", totalFuseTime * 10f);
                base.PlayAnimation("Gesture, Override", "ThrowGrenade", "FireFMJ.playbackRate", totalFuseTime * 10f);
            }

            ChildLocator childLocator = base.GetModelChildLocator();
            if (childLocator)
            {
                rightHand = childLocator.FindChild("HandR");
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch > 1f && base.fixedAge < totalFuseTime)
            {
                stopwatch -= 1f;
                Util.PlaySound(beepSoundString, base.gameObject);
                if (rightHand)
                {
                    EffectManager.SpawnEffect(grenadeIndicatorPrefab, new EffectData { origin = rightHand.position }, false);
                }
            }

            if (base.isAuthority)
            {
                base.StartAimMode(2f);
                if (base.inputBank && base.inputBank.skill4.down)
                {
                    if (base.fixedAge > totalFuseTime)
                    {
                        if (base.characterBody && base.healthComponent)
                        {
                            OvercookExplosion();
                        }
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
                else
                {
                    SwapToThrowGrenade();
                    return;
                }
            }
        }

        public virtual void SwapToThrowGrenade()
        {
            grenadeThrown = true;
            this.outer.SetNextState(new ThrowGrenade { fuseTime = base.fixedAge });
        }

        public override void OnExit()
        {
            if (!grenadeThrown && animator)
            {
                base.PlayAnimation("Gesture, Additive", "ThrowGrenade", "FireFMJ.playbackRate", 0.5f);
                base.PlayAnimation("Gesture, Override", "ThrowGrenade", "FireFMJ.playbackRate", 0.5f);
            }
            base.OnExit();
        }

        private void OvercookExplosion()
        {
            EffectManager.SpawnEffect(overcookExplosionEffectPrefab, new EffectData { origin = base.transform.position, scale = radiusInternal }, true);
            new BlastAttack
            {
                radius = radiusInternal,
                attackerFiltering = AttackerFiltering.NeverHit,
                baseDamage = this.damageStat * damageCoefficient,
                falloffModel = BlastAttack.FalloffModel.None,
                procCoefficient = 1f,
                baseForce = force,
                crit = base.RollCrit(),
                damageType = DamageType.Generic,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                position = base.transform.position,
                teamIndex = base.teamComponent.teamIndex,
                procChainMask = default(ProcChainMask)
            }.Fire();

            DamageInfo selfDamage = new DamageInfo
            {
                attacker = null,
                crit = false,
                damage = base.healthComponent.fullCombinedHealth * CookGrenade.selfHPDamagePercent,
                force = base.GetAimRay().direction * selfForceInternal,
                damageType = DamageType.AOE,
                inflictor = null,
                procCoefficient = 0f,
                procChainMask = default(ProcChainMask),
                position = base.transform.position
            };
            NetworkingHelpers.DealDamage(selfDamage, base.characterBody.mainHurtBox, true, false, false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
