using System;
using RoR2;
using R2API;
using UnityEngine;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using RiskyMod.Content;
using UnityEngine.Networking;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class FreezeChampionExecute
    {
        public static bool enabled = true;
        public static BuffDef FreezeDebuff;
        public static float bossExecuteFractionMultiplier = 1f/3f;

        public static bool nerfFreeze = false;

        public FreezeChampionExecute()
        {
            if (!enabled) return;
            BuildDebuff();
            ModifyExecuteThreshold();
            ApplyFreezeDebuff();
        }

        private void BuildDebuff()
        {
            FreezeDebuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_FreezeDebuff",
                false,
                false,
                true,
                Color.white,
                Content.Assets.BuffIcons.Freeze,
                true
                );

            RecalculateStatsAPI.GetStatCoefficients += ApplySlow;

            IL.RoR2.CharacterModel.UpdateOverlays += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Slow80")
                    ))
                {
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterModel, bool>>((hasBuff, self) =>
                    {
                        return hasBuff || (self.body.HasBuff(FreezeDebuff));
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: FreezeChampionExecute UpdateOverlays IL Hook failed");
                }
            };
        }

        private static void ApplySlow(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(FreezeDebuff))
            {
                args.moveSpeedReductionMultAdd += 0.8f;
            }
        }

        private float GetExecuteThreshold(float executeTreshold, HealthComponent self)
        {
            float multiplier = 1f;
            if (self.body.isPlayerControlled)
            {
                multiplier = 0f;
            }
            else if ((self.body.isChampion || self.body.bodyIndex == DLC3Content.BodyPrefabs.ExhaustPortWeakpointBody.bodyIndex) || nerfFreeze)
            {
                multiplier = bossExecuteFractionMultiplier;
            }
            return executeTreshold * multiplier;
        }

        //This doesn't fix Guillotines not stacking with Freeze, but Guillotines are reworked anyways.
        private void ModifyExecuteThreshold()
        {
            IL.RoR2.HealthComponent.TakeDamageProcess += TakeDamageExecute;

            IL.RoR2.HealthComponent.GetHealthBarValues += DisplayExecuteOnHealthBar;
        }

        private void TakeDamageExecute(ILContext il)
        {
            bool error = true;
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchCall<HealthComponent>("get_isInFrozenState")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, HealthComponent, bool>>((isFrozen, self) =>
                {
                    return isFrozen || (self.body.HasBuff(FreezeDebuff) && !Allies.AlliesCore.IsDrone(self));
                });

                if (c.TryGotoNext(MoveType.After,
                    x => x.MatchLdcR4(0.3f)
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, HealthComponent, float>>((executeThreshold, self) =>
                    {
                        return GetExecuteThreshold(executeThreshold, self);
                    });

                    if (c.TryGotoNext(MoveType.After,
                        x => x.MatchLdcR4(0.3f)
                        ))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<float, HealthComponent, float>>((executeThreshold, self) =>
                        {
                            return GetExecuteThreshold(executeThreshold, self);
                        });
                        error = false;
                    }
                }
            }

            if (error)
            {
                UnityEngine.Debug.LogError("RiskyMod: FreezeChampionExecute TakeDamage IL Hook failed");
            }
        }

        private void DisplayExecuteOnHealthBar(ILContext il)
        {
            bool error = true;

            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchCall<HealthComponent>("get_isInFrozenState")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, HealthComponent, bool>>((isFrozen, self) =>
                {
                    return isFrozen || self.body.HasBuff(FreezeDebuff);
                });

                if (c.TryGotoNext(MoveType.After,
                    x => x.MatchLdcR4(0.3f)
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, HealthComponent, float>>((executeThreshold, self) =>
                    {
                        return GetExecuteThreshold(executeThreshold, self);
                    });
                    error = false;
                }
            }

            if (error)
            {
                UnityEngine.Debug.LogError("RiskyMod: FreezeChampionExecute GetHealthBarValues IL Hook failed");
            }
        }

        private void ApplyFreezeDebuff()
        {
            On.EntityStates.FrozenState.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.characterBody)
                {
                    self.characterBody.AddBuff(FreezeDebuff);
                }
            };

            On.EntityStates.FrozenState.OnExit += (orig, self) =>
            {
                orig(self);
                if (self.characterBody && self.characterBody.HasBuff(FreezeDebuff))
                {
                    self.characterBody.RemoveBuff(FreezeDebuff);
                }
            };

            SharedHooks.TakeDamage.OnDamageTakenActions += ApplyDebuff;
            IL.RoR2.CharacterBody.HandleCascadingBuffs += CharacterBody_HandleCascadingBuffs;
        }

        private void CharacterBody_HandleCascadingBuffs(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "Frost"))
                && c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC2Content.Buffs), "FreezeImmune")))
            {
                c.Emit(OpCodes.Ldarg_0);//Characterbody
                c.EmitDelegate<Func<BuffDef, CharacterBody, BuffDef>>((buff, body) =>
                {
                    bool notFreezeImmune = !body.HasBuff(DLC2Content.Buffs.FreezeImmune);
                    bool notFrozen = !(body.healthComponent && body.healthComponent.isInFrozenState);
                    if (notFreezeImmune && notFrozen && !body.IsDrone)
                    {
                        body.AddTimedBuff(FreezeDebuff, 3f);
                    }
                    return buff;
                });
            }
        }

        private static void ApplyDebuff(DamageInfo damageInfo, HealthComponent self)
        {
            if ((damageInfo.damageType & DamageType.Freeze2s) == DamageType.Freeze2s)
            {
                var body = self.body;

                if (body)
                {
                    bool notFreezeImmune = !body.HasBuff(DLC2Content.Buffs.FreezeImmune);
                    bool notFrozen = !(body.healthComponent && body.healthComponent.isInFrozenState);
                    if (notFreezeImmune && notFrozen && !body.IsDrone)
                    {
                        self.body.AddTimedBuff(FreezeDebuff.buffIndex, 2f * damageInfo.procCoefficient);    //this is how freeze is handled in SetStateOnHurt
                    }
                }
            }
        }
    }
}
