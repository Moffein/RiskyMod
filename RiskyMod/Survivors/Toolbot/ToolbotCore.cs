using RoR2;
using UnityEngine;
using R2API;
using MonoMod.Cil;
using System;
using RoR2.Skills;
using EntityStates;
using RiskyMod.SharedHooks;
using Mono.Cecil.Cil;

namespace RiskyMod.Survivors.Toolbot
{
    public class ToolbotCore
    {
        public static bool enabled = true;
        public static bool enableNailgunChanges = true;
        public static bool enableRebarChanges = true;
        public static bool enableScrapChanges = true;
        public static bool enableSawChanges = true;

        public static bool enableSecondaryChanges = true;

        public static bool enableSpecialChanges = true;

        public static DamageAPI.ModdedDamageType StunDroneForce;
        public static DamageAPI.ModdedDamageType SawBarrier;
        public static BuffDef PowerModeBuff;

        public ToolbotCore()
        {
            if (!enabled) return;

            ModifySkills(RoR2Content.Survivors.Toolbot.bodyPrefab.GetComponent<SkillLocator>());

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
            //sk.special.skillFamily.variants[0].skillDef.baseRechargeInterval = 0f;
            ModifyPrimaries(sk);
            ModifySecondaries(sk);
            ModifySpecials(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            NailgunChanges(sk);
            RebarChanges(sk);
            ScrapChanges(sk);
            SawChanges(sk);
        }

        private void NailgunChanges(SkillLocator sk)
        {
            if (!enableNailgunChanges) return;
            sk.primary.skillFamily.variants[0].skillDef.skillDescriptionToken = "TOOLBOT_PRIMARY_DESCRIPTION_RISKYMOD";
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Toolbot.BaseNailgunState", "damageCoefficient", "0.6");
            IL.EntityStates.Toolbot.BaseNailgunState.FireBullet += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                     );
                c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                {
                    bulletAttack.radius = 0.2f;
                    bulletAttack.smartCollision = true;
                    return bulletAttack;
                });
            };
            new FixNailgunBurst();
        }

        private void RebarChanges(SkillLocator sk)
        {
            if (!enableRebarChanges) return;
            sk.primary.skillFamily.variants[1].skillDef.skillDescriptionToken = "TOOLBOT_PRIMARY_ALT1_DESCRIPTION_RISKYMOD";
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Toolbot.FireSpear", "damageCoefficient", "7.2");
        }

        private void ScrapChanges(SkillLocator sk)
        {
            if (!enableScrapChanges) return;
            sk.primary.skillFamily.variants[2].skillDef.skillDescriptionToken = "TOOLBOT_PRIMARY_ALT2_DESCRIPTION_RISKYMOD";
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Toolbot.FireGrenadeLauncher", "damageCoefficient", "3.9");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Toolbot.FireGrenadeLauncher", "maxSpread", "0");
        }

        private void SawChanges(SkillLocator sk)
        {
            if (!enableSawChanges) return;

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Toolbot.FireBuzzsaw", "selfForceMagnitude", "0");
            IL.EntityStates.Toolbot.FireBuzzsaw.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<CharacterMotor>("ApplyForce")
                     );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<int, EntityStates.Toolbot.FireBuzzsaw, int>>((orig, self) =>
                {
                    if (self.characterMotor && !self.characterMotor.isGrounded &&self.characterMotor.velocity.y < 0f)
                    {
                        self.characterMotor.velocity.y = 0f;
                        self.SmallHop(self.characterMotor, 3f);
                    }
                    return orig;
                });
            };

            SawBarrier = DamageAPI.ReserveDamageType();
            On.EntityStates.Toolbot.FireBuzzsaw.OnEnter += (orig, self) =>
            {
                orig(self);
                self.attack.AddModdedDamageType(SawBarrier);
            };
            OnHitEnemy.OnHitAttackerActions += SawBarrierOnHit;

            sk.primary.skillFamily.variants[3].skillDef.skillDescriptionToken = "TOOLBOT_PRIMARY_ALT3_DESCRIPTION_RISKYMOD";

            HitBoxGroup[] hitboxes = RoR2Content.Survivors.Toolbot.bodyPrefab.GetComponentsInChildren<HitBoxGroup>();
            foreach (HitBoxGroup h in hitboxes)
            {
                if (h.groupName.Contains("Buzzsaw"))
                {
                    h.hitBoxes[0].transform.localScale *= 1.5f;
                }
            }
        }

        private  void SawBarrierOnHit(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(SawBarrier))
            {
                if (attackerBody.healthComponent)
                {
                    attackerBody.healthComponent.AddBarrier(attackerBody.healthComponent.fullCombinedHealth * 0.006f);
                }
            }
        }

        private void ModifySecondaries(SkillLocator sk)
        {
            if (!enableSecondaryChanges) return;

            StunDroneForce = DamageAPI.ReserveDamageType();
            TakeDamage.ModifyInitialDamageActions += ApplyStunDroneForce;
            DamageAPI.ModdedDamageTypeHolderComponent md = Resources.Load<GameObject>("prefabs/projectiles/CryoCanisterProjectile").AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            md.Add(StunDroneForce);
        }
        private static void ApplyStunDroneForce(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (damageInfo.HasModdedDamageType(StunDroneForce))
            {
                Vector3 direction = Vector3.down;
                CharacterBody cb = self.body;
                if (cb && cb.isFlying)
                {
                    //Scale force to match mass
                    Rigidbody rb = cb.rigidbody;
                    if (rb)
                    {
                        direction *= Mathf.Max(rb.mass / 100f, 1f);
                        damageInfo.force += 1600f * direction;
                    }
                }
            }
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (!enableSpecialChanges) return;
            On.EntityStates.Toolbot.ToolbotStanceSwap.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.isAuthority)
                {
                    GenericSkill skill1 = self.GetPrimarySkill1();
                    skill1.stock = skill1.maxStock;

                    GenericSkill skill2 = self.GetPrimarySkill2();
                    skill2.stock = skill2.maxStock;
                }
            };

            sk.special.skillFamily.variants[1].skillDef.skillDescriptionToken = "TOOLBOT_SPECIAL_ALT_DESCRIPTION_RISKYMOD";
            PowerModeBuff = ScriptableObject.CreateInstance<BuffDef>();
            PowerModeBuff.buffColor = RoR2Content.Buffs.SmallArmorBoost.buffColor;
            PowerModeBuff.canStack = false;
            PowerModeBuff.isDebuff = false;
            PowerModeBuff.name = "RiskyMod_PowerModeBuff";
            PowerModeBuff.iconSprite = RoR2Content.Buffs.SmallArmorBoost.iconSprite;
            BuffAPI.Add(new CustomBuff(PowerModeBuff));
            GetStatsCoefficient.HandleStatsActions += HandlePowerMode;
        }

        private void HandlePowerMode(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(PowerModeBuff.buffIndex))
            {
                args.armorAdd += 75f;
            }
        }
    }
}
