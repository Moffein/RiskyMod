using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class Raincoat
    {
        public static bool enabled = true;
        public static BuffDef RaincoatReadyBuff;
        public static BuffDef RaincoatActiveBuff;
        public static BuffDef RaincoatCooldownBuff;

        public static GameObject triggerEffectPrefab;
        public static GameObject debuffNegateEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/ImmuneToDebuff/ImmuneToDebuffEffect.prefab").WaitForCompletion();
        public static GameObject endEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/ImmuneToDebuff/ImmuneToDebuffEffect.prefab").WaitForCompletion();

        public Raincoat()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            triggerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/muzzleflashes/Bandit2SmokeBomb").InstantiateClone("RiskyMod_ProcRaincoat", false);
            EffectComponent ec = triggerEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_bandit2_shift_exit";
            Content.Content.effectDefs.Add(new EffectDef(triggerEffectPrefab));

            endEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/CleanseEffect").InstantiateClone("RiskyMod_ProcRaincoatEnd", false);
            ec = endEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_item_lunar_use_utilityReplacement_end";
            Content.Content.effectDefs.Add(new EffectDef(endEffectPrefab));

            DisableVanillaBehavior();
            HookTimedDebuffs();
            SetupBuffs();
        }

        public static bool ProcRaincoat(CharacterBody body)
        {
            if (body)
            {
                if (body.HasBuff(RaincoatActiveBuff))
                {
                    EffectManager.SimpleImpactEffect(debuffNegateEffectPrefab, body.corePosition, Vector3.up, true);
                    return true;
                }
                else if (body.HasBuff(RaincoatReadyBuff))
                {
                    body.RemoveBuff(RaincoatReadyBuff);
                    int itemCount = body.inventory ? body.inventory.GetItemCount(DLC1Content.Items.ImmuneToDebuff) : 0;
                    if (itemCount > 0)
                    {
                        body.AddTimedBuff(RaincoatActiveBuff, 4f);

                        SneedUtils.SneedUtils.StunEnemiesInSphere(body, 12f);
                        EffectManager.SimpleImpactEffect(debuffNegateEffectPrefab, body.corePosition, Vector3.up, true);
                        EffectManager.SpawnEffect(triggerEffectPrefab,
                            new EffectData
                            {
                                origin = body.corePosition
                            }, true);
                    }
                    return true;
                }
            }
            return false;
        }

        private void SetupBuffs()
        {
            RaincoatReadyBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_RaincoatReady",
                false,
                false,
                false,
                new Color(1f,1f,1f),
                Content.Assets.BuffIcons.RaincoatReady
                );

            RaincoatActiveBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_RaincoatActive",
                false,
                false,
                false,
                new Color(1f, 1f, 1f),
                Content.Assets.BuffIcons.RaincoatActive
                );

            RaincoatCooldownBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_RaincoatCooldown",
                true,
                true,
                false,
                new Color(1f, 1f, 1f),
                Content.Assets.BuffIcons.RaincoatCooldown
                );


            //Raincoat Active
            SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += (sender, args, inventory) =>
            {
                if (sender.HasBuff(RaincoatActiveBuff))
                {
                    int itemCount = inventory.GetItemCount(DLC1Content.Items.ImmuneToDebuff);
                    args.armorAdd += 50f * Mathf.Max(1f, itemCount);
                }
            };

            //Cooldown Start
            On.RoR2.CharacterBody.RemoveBuff_BuffIndex += (orig, self, buffType) =>
            {
                orig(self, buffType);
                if (NetworkServer.active)
                {
                    if (buffType == RaincoatActiveBuff.buffIndex)
                    {
                        if (!self.HasBuff(RaincoatActiveBuff))
                        {
                            float cooldown = 15f;
                            int itemCount = self.inventory ? self.inventory.GetItemCount(DLC1Content.Items.ImmuneToDebuff) : 0;
                            if (itemCount > 1)
                            {
                                cooldown = Mathf.Max(cooldown * Mathf.Pow(0.9f, itemCount - 1), 1f);
                            }
                            int stacksToApply = Mathf.CeilToInt(cooldown);
                            for (int i = 0; i < stacksToApply; i++)
                            {
                                self.AddTimedBuff(RaincoatCooldownBuff, i == 0 ? cooldown : stacksToApply - i);
                            }
                            EffectManager.SpawnEffect(endEffectPrefab,
                               new EffectData
                               {
                                   origin = self.corePosition
                               }, true);
                        }
                    }
                }
            };

            //Cooldown Expired
            On.RoR2.CharacterBody.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    if (self.inventory && self.inventory.GetItemCount(DLC1Content.Items.ImmuneToDebuff) > 0)
                    {
                        if (!(self.HasBuff(RaincoatActiveBuff) || self.HasBuff(RaincoatCooldownBuff) || self.HasBuff(RaincoatReadyBuff)))
                        {
                            self.AddBuff(RaincoatReadyBuff);
                        }
                    }
                }
            };
        }

        private void HookTimedDebuffs()
        {
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += (orig, self, buffDef, duration) =>
            {
                if (NetworkServer.active && buffDef.isDebuff)
                {
                    if (ProcRaincoat(self))
                    {
                        return;
                    }
                }
                orig(self, buffDef, duration);
            };

            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float_int += (orig, self, buffDef, duration, maxStacks) =>
            {
                if (NetworkServer.active && buffDef.isDebuff)
                {
                    if (ProcRaincoat(self))
                    {
                        return;
                    }
                }
                orig(self, buffDef, duration, maxStacks);
            };

            On.RoR2.DotController.InflictDot_refInflictDotInfo += (On.RoR2.DotController.orig_InflictDot_refInflictDotInfo orig, ref InflictDotInfo inflictDotInfo) =>
            {
                if (NetworkServer.active && inflictDotInfo.victimObject)
                {
                    CharacterBody characterBody = inflictDotInfo.victimObject.GetComponent<CharacterBody>();
                    if (ProcRaincoat(characterBody))
                    {
                        return;
                    }
                }
                orig(ref inflictDotInfo);
            };
        }

        private void DisableVanillaBehavior()
        {
            //Remove Vanilla behavior
            On.RoR2.ImmuneToDebuffBehavior.OverrideDebuff_BuffDef_CharacterBody += (orig, buff, body) =>
            {
                return false;
            };
            On.RoR2.ImmuneToDebuffBehavior.OverrideDebuff_BuffIndex_CharacterBody += (orig, buff, body) =>
            {
                return false;
            };
            On.RoR2.ImmuneToDebuffBehavior.OverrideDot += (orig, dot) =>
            {
                return false;
            };

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "ImmuneToDebuff")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.ImmuneToDebuff);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, DLC1Content.Items.ImmuneToDebuff);
        }
    }
}
