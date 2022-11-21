using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.CharacterAI;
using RiskyMod.MonoBehaviours;
using RoR2.Skills;
using UnityEngine.Networking;
using RoR2.Projectile;
using System.Linq;
using EntityStates.TitanMonster;

namespace RiskyMod.Enemies.Bosses
{
    public class Titan
    {
        public static bool enabled = true;
        public static float laserTrackingSpeed = 30f;   //xi is 10

        public Titan()
        {
            if (!enabled) return;
            FasterAnims();
            LaserRework();
            ModifyAI();
            FistRework();
            RockTargeting();
        }

        private void FasterAnims()
        {
            //Debug.Log("\nFireFist");
            //SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Titan/EntityStates.TitanMonster.FireFist.asset").WaitForCompletion());
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Titan/EntityStates.TitanMonster.FireFist.asset", "exitDuration", "1.5"); //Vanilla 3

            //Debug.Log("\nRechargeRocks");
            //SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Titan/EntityStates.TitanMonster.RechargeRocks.asset").WaitForCompletion());
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Titan/EntityStates.TitanMonster.RechargeRocks.asset", "baseDuration", "5"); //Vanilla 9

            //Debug.Log("\nChargeMegaLaser");
            //SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Titan/EntityStates.TitanMonster.ChargeMegaLaser.asset").WaitForCompletion());
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Titan/EntityStates.TitanMonster.ChargeMegaLaser.asset", "baseDuration", "2"); //Vanilla 3

            //Debug.Log("\nFireMegaLaser");
            //SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Titan/EntityStates.TitanMonster.FireMegaLaser.asset").WaitForCompletion());
        }

        private void LaserRework()
        {
            //Xi Construct is 10 damageCoefficient, 10 fireFrequency, 13 base damage
            //Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Titan/TitanBodyLaser.asset").WaitForCompletion().baseRechargeInterval = 15f;   //Vanilla 20
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Titan/EntityStates.TitanMonster.FireMegaLaser.asset", "damageCoefficient", "1.5");    //Vanilla 1
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Titan/EntityStates.TitanMonster.FireMegaLaser.asset", "fireFrequency", "10");    //Vanilla 8

            TrackingRework();
            MegaLaserAttackSpeed();
            LaserRadius();
            //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.MajorConstruct.Weapon.FireLaser");
        }

        private void MegaLaserAttackSpeed()
        {
            IL.EntityStates.TitanMonster.FireMegaLaser.FixedUpdate += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdsfld<EntityStates.TitanMonster.FireMegaLaser>("fireFrequency")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);    //self
                    c.EmitDelegate<Func<float, EntityStates.TitanMonster.FireMegaLaser, float>>((freq, self) =>
                    {
                        return freq * self.attackSpeedStat;
                    });

                    if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdsfld<EntityStates.TitanMonster.FireMegaLaser>("fireFrequency")
                    ))
                    {
                        c.Emit(OpCodes.Ldarg_0);    //self
                        c.EmitDelegate<Func<float, EntityStates.TitanMonster.FireMegaLaser, float>>((freq, self) =>
                        {
                            return freq * self.attackSpeedStat;
                        });
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Titan MegaLaserAttackSpeed IL Hook failed");
                }
            };
        }

        private void LaserRadius()
        {
            IL.EntityStates.TitanMonster.FireMegaLaser.FireBullet += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                     ))
                {
                    c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                    {
                        //bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
                        bulletAttack.radius = 0.2f; //Vanilla 0
                        return bulletAttack;
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: Titan LaserRadius IL Hook failed");
                }
            };
        }

        private void ModifyAI()
        {
            GameObject masterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanMaster.prefab").WaitForCompletion();

            BaseAI ba = masterObject.GetComponent<BaseAI>();

            AISkillDriver[] aiDrivers = masterObject.GetComponents<AISkillDriver>();
            foreach(AISkillDriver ai in aiDrivers)
            {
                if (ai.skillSlot == SkillSlot.Special)
                {
                    ai.minDistance = 0f;
                    ai.maxDistance = 200f;
                    ai.aimType = AISkillDriver.AimType.AtCurrentEnemy;
                    //ai.driverUpdateTimerOverride = 10f; //laser firing = 8s, laser chargeup = 2s
                }
            }
        }

        private void FistRework()
        {
            //Copied from Aurelionite
            On.EntityStates.TitanMonster.FireFist.PlacePredictedAttack += (orig, self) =>
            {
                int fistCount = 4;
                float distanceBetweenFists = 10f;//10
                float delayBetweenFists = 0.1f;//0.1

                int num = 0;
                Vector3 predictedTargetPosition = self.predictedTargetPosition;
                Vector3 a = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f) * Vector3.forward;
                for (int i = -(fistCount / 2); i < fistCount / 2; i++)
                {
                    Vector3 vector = predictedTargetPosition + a * distanceBetweenFists * (float)i;
                    float num2 = 60f;
                    RaycastHit raycastHit;
                    if (Physics.Raycast(new Ray(vector + Vector3.up * (num2 / 2f), Vector3.down), out raycastHit, num2, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
                    {
                        vector = raycastHit.point;
                    }
                    self.PlaceSingleDelayBlast(vector, delayBetweenFists * (float)num);
                    num++;
                }
            };
        }

        private void RockTargeting()
        {
            On.RoR2.TitanRockController.Fire += (orig, self) =>
            {
                if (!(self.ownerCharacterBody && self.ownerCharacterBody.teamComponent))
                {
                    orig(self);
                }
                else
                {
                    if (NetworkServer.active)
                    {
                        Vector3 forward = Vector3.down;

                        if (self.ownerInputBank)
                        {
                            forward = self.ownerInputBank.GetAimRay().direction;
                        }

                        //Scan for targets
                        BullseyeSearch search = new BullseyeSearch();

                        search.teamMaskFilter = TeamMask.allButNeutral;
                        search.teamMaskFilter.RemoveTeam(self.ownerCharacterBody.teamComponent.teamIndex);

                        search.filterByLoS = true;
                        search.searchOrigin = self.fireTransform.position;
                        search.sortMode = BullseyeSearch.SortMode.Distance;
                        search.maxDistanceFilter = 200f;
                        search.maxAngleFilter = 360f;
                        search.searchDirection = forward;
                        search.RefreshCandidates();

                        HurtBox targetHurtBox = search.GetResults().FirstOrDefault<HurtBox>();

                        if (targetHurtBox)
                        {
                            forward = (targetHurtBox.transform.position - self.fireTransform.position).normalized;
                        }

                        float num = self.ownerCharacterBody ? self.ownerCharacterBody.damage : 1f;
                        ProjectileManager.instance.FireProjectile(self.projectilePrefab, self.fireTransform.position, Util.QuaternionSafeLookRotation(forward), self.owner, self.damageCoefficient * num, self.damageForce, self.isCrit, DamageColorIndex.Default, null, -1f);
                    }
                }
            };
        }

        private void TrackingRework()
        {
            On.EntityStates.TitanMonster.FireMegaLaser.FixedUpdate += (orig, self) =>
            {
                self.fixedAge += Time.fixedDeltaTime;
                self.fireStopwatch += Time.fixedDeltaTime;
                self.stopwatch += Time.fixedDeltaTime;

                Ray trueAimRay = self.GetAimRay();

                Vector3 muzzlePosition = trueAimRay.origin;
                if (self.muzzleTransform)
                {
                    muzzlePosition = self.muzzleTransform.position;
                }

                //Find intended Laser aim direction
                float targetDistance = Mathf.Infinity;
                Vector3 idealAimDirection = trueAimRay.direction;
                if (self.lockedOnHurtBox)
                {
                    idealAimDirection = self.lockedOnHurtBox.transform.position - muzzlePosition;
                    targetDistance = idealAimDirection.magnitude;
                    idealAimDirection.Normalize();
                }

                Vector3 currentAimDirection = (self.laserEffectEnd.transform.position - muzzlePosition).normalized;
                self.aimRay.origin = muzzlePosition;
                self.aimRay.direction = currentAimDirection;

                self.aimRay.direction = Vector3.RotateTowards(self.aimRay.direction, idealAimDirection, Titan.laserTrackingSpeed * 0.0174532924f * Time.fixedDeltaTime, float.PositiveInfinity); //constant converts euler to radians
                Vector3 laserEndPoint = muzzlePosition + FireMegaLaser.maxDistance * self.aimRay.direction;

                RaycastHit raycastHit;
                if (Physics.Raycast(self.aimRay, out raycastHit, FireMegaLaser.maxDistance, LayerIndex.world.mask | LayerIndex.entityPrecise.mask))
                {
                    laserEndPoint = raycastHit.point;
                }

                if (self.laserEffect && self.laserChildLocator)
                {
                    self.laserEffect.transform.rotation = Util.QuaternionSafeLookRotation(self.aimRay.direction);
                    self.laserEffectEnd.transform.position = laserEndPoint;
                }
                if (self.fireStopwatch > 1f / FireMegaLaser.fireFrequency)
                {
                    string targetMuzzle = "MuzzleLaser";
                    self.FireBullet(self.modelTransform, self.aimRay, targetMuzzle, FireMegaLaser.maxDistance);
                    self.UpdateLockOn();
                    self.fireStopwatch -= 1f / FireMegaLaser.fireFrequency;
                }

                bool isPlayer = self.characterBody && self.characterBody.isPlayerControlled;
                bool inputReleased = isPlayer && (!self.inputBank || !self.inputBank.skill4.down);
                bool minDurationPassed = self.stopwatch > FireMegaLaser.minimumDuration;
                bool maxDurationPassed = self.stopwatch > FireMegaLaser.maximumDuration;

                if (self.isAuthority && ((inputReleased  && minDurationPassed) || maxDurationPassed))
                {
                    self.outer.SetNextStateToMain();
                    return;
                }
            };
        }
    }
}
