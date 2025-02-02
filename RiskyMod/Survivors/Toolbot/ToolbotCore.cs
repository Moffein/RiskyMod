using RoR2;
using UnityEngine;
using R2API;
using MonoMod.Cil;
using System;
using RoR2.Skills;
using EntityStates;
using RiskyMod.SharedHooks;
using Mono.Cecil.Cil;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;

namespace RiskyMod.Survivors.Toolbot
{
    public class ToolbotCore
    {
        public static bool enabled = true;
        public static bool enableRebarChanges = true;
        public static bool enableScrapChanges = true;

        public static bool sawPhysics = true;
        public static bool sawBarrierOnHit = true;
        public static bool sawHitbox = true;

        public static bool enableSecondarySkillChanges = true;

        public static bool enablePowerModeChanges = true;

        public static BuffDef PowerModeBuff;
        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/ToolbotBody");

        public ToolbotCore()
        {
            if (!enabled) return;

            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());

            //Vanilla Titan kill times
            //Nailgun takes 26.14s
                //19s with Shift included
                //13.3s power mode

            //Rebar takes 65.8s
                //37.6 with quickswap

            //Scrap Launcher takes 37.8s
                //25.34s with quickswap
                //19.75s with power mode
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            RebarChanges(sk);
            SawChanges(sk);
        }

        private void RebarChanges(SkillLocator sk)
        {
            if (!enableRebarChanges) return;
            SkillDef railgunDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodyFireSpear.asset").WaitForCompletion();
            railgunDef.skillDescriptionToken = "TOOLBOT_PRIMARY_ALT1_DESCRIPTION_RISKYMOD";
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Toolbot.FireSpear", "damageCoefficient", "6.6");
        }

        private void SawChanges(SkillLocator sk)
        {
            if (sawPhysics)
            {
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Toolbot.FireBuzzsaw", "selfForceMagnitude", "0");
                IL.EntityStates.Toolbot.FireBuzzsaw.FixedUpdate += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(
                         x => x.MatchCallvirt<CharacterMotor>("ApplyForce")
                         ))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<int, EntityStates.Toolbot.FireBuzzsaw, int>>((orig, self) =>
                        {
                            if (self.characterMotor && !self.characterMotor.isGrounded && self.characterMotor.velocity.y <= 0f)
                            {
                                //self.characterMotor.velocity.y = 0f;
                                self.SmallHop(self.characterMotor, 2.5f);
                            }
                            return orig;
                        });
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("RiskyMod: Toolbot SawPhysics IL Hook failed");
                    }
                };
            }

            SkillDef sawDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotBodyFireBuzzsaw.asset").WaitForCompletion();
            sawDef.cancelSprintingOnActivation = false;

            if (sawHitbox)
            {
                HitBoxGroup[] hitboxes = ToolbotCore.bodyPrefab.GetComponentsInChildren<HitBoxGroup>();
                foreach (HitBoxGroup h in hitboxes)
                {
                    if (h.groupName.Contains("Buzzsaw"))
                    {
                        h.hitBoxes[0].transform.localScale *= 1.5f;
                    }
                }
            }

            if (sawBarrierOnHit)
            {
                On.EntityStates.Toolbot.FireBuzzsaw.OnEnter += (orig, self) =>
                {
                    orig(self);
                    self.attack.AddModdedDamageType(SharedDamageTypes.SawBarrier);
                };

                IL.EntityStates.Toolbot.FireBuzzsaw.FixedUpdate += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(MoveType.After,
                         x => x.MatchLdfld(typeof(EntityStates.Toolbot.FireBuzzsaw), "hitOverlapLastTick")
                         ))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<bool, EntityStates.Toolbot.FireBuzzsaw, bool>>((hitEnemy, self) =>
                        {
                            if (hitEnemy && self.isAuthority && self.healthComponent)
                            {
                                self.healthComponent.AddBarrierAuthority(self.healthComponent.fullCombinedHealth * 0.003f);
                            }
                            return hitEnemy;
                        });
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("RiskyMod: Toolbot SawBarrier IL Hook failed");
                    }
                };

                sawDef.skillDescriptionToken = "TOOLBOT_PRIMARY_ALT3_DESCRIPTION_RISKYMOD";
            }
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (!enableSecondarySkillChanges) return;
            On.EntityStates.Toolbot.AimStunDrone.ModifyProjectile += AimStunDrone_ModifyProjectile;
        }

        private void AimStunDrone_ModifyProjectile(On.EntityStates.Toolbot.AimStunDrone.orig_ModifyProjectile orig, EntityStates.Toolbot.AimStunDrone self, ref FireProjectileInfo fireProjectileInfo)
        {
            orig(self, ref fireProjectileInfo);
            if (fireProjectileInfo.damageTypeOverride.HasValue)
            {
                DamageTypeCombo combo = fireProjectileInfo.damageTypeOverride.Value;
                combo.AddModdedDamageType(SharedDamageTypes.AntiFlyingForce);
                fireProjectileInfo.damageTypeOverride = combo;
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (enablePowerModeChanges)
            {
                SkillDef powerSkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Toolbot/ToolbotDualWield.asset").WaitForCompletion();
                BuffDef powerDef = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/SmallArmorBoost");

                powerSkillDef.skillDescriptionToken = "TOOLBOT_SPECIAL_ALT_DESCRIPTION_RISKYMOD";

                PowerModeBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_PowerModeBuff",
                false,
                false,
                false,
                new Color(0.839f, 0.788f, 0.227f),
                powerDef.iconSprite
                );

                RecalculateStatsAPI.GetStatCoefficients += HandlePowerMode;
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Toolbot.ToolbotDualWieldBase", "bonusBuff", PowerModeBuff);
            }
        }

        private void HandlePowerMode(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(PowerModeBuff.buffIndex))
            {
                args.armorAdd += 60f;
            }
        }
    }
}
