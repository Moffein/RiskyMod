using RoR2;
using System;
using UnityEngine;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace RiskyMod.Items.DLC1.Common
{
    public class PowerElixir
    {
        public static bool enabled = true;
        public static BuffDef regenBuff;

        public PowerElixir()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            BuffDef bd = LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CrocoRegen");
            regenBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_HealingPotionBuff",
                false,
                false,
                false,
                new Color(1f, 100f / 255f, 100f / 255f),
                bd.iconSprite
                );

            IL.RoR2.HealthComponent.UpdateLastHitTime += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "healingPotion")
                    ))
                {
                    c.EmitDelegate<Func<int, int>>(val => 0);
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: PowerElixir UpdateLastHitTime IL Hook failed");
                }
            };

            SharedHooks.HealthComponent_UpdateLastHitTime.UpdateLastHitTimeActions += TriggerElixir;
            On.RoR2.Stage.Start += StageStartResetElixir;

            IL.RoR2.HealthComponent.ServerFixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(HealthComponent), "regenAccumulator")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, HealthComponent, float>>((regenAccumulator, self) =>
                    {
                        if (self.body.HasBuff(PowerElixir.regenBuff))
                        {
                            regenAccumulator += Time.fixedDeltaTime * 0.0625f * self.fullHealth;
                        }
                        return regenAccumulator;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: PowerElixir ModifyRegenAccumulator IL Hook failed");
                }
            };
        }

        private static void StageStartResetElixir(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            if (CharacterMaster.instancesList != null)
            {
                foreach (CharacterMaster cm in CharacterMaster.instancesList)
                {
                    if (cm.inventory)
                    {
                        int brokenElixirCount = cm.inventory.GetItemCount(DLC1Content.Items.HealingPotionConsumed);
                        if (brokenElixirCount > 0)
                        {
                            cm.inventory.RemoveItem(DLC1Content.Items.HealingPotionConsumed, brokenElixirCount);
                            cm.inventory.GiveItem(DLC1Content.Items.HealingPotion, brokenElixirCount);
                            CharacterMasterNotificationQueue.SendTransformNotification(cm, DLC1Content.Items.HealingPotionConsumed.itemIndex, DLC1Content.Items.HealingPotion.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                        }
                    }
                }
            }
        }

        private static void TriggerElixir (HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
        {
            if (self.itemCounts.healingPotion > 0 && self.health/self.fullHealth <= 0.5f && !self.body.HasBuff(PowerElixir.regenBuff))
            {
                self.body.inventory.RemoveItem(DLC1Content.Items.HealingPotion, 1);
                self.body.inventory.GiveItem(DLC1Content.Items.HealingPotionConsumed, 1);
                CharacterMasterNotificationQueue.SendTransformNotification(self.body.master, DLC1Content.Items.HealingPotion.itemIndex, DLC1Content.Items.HealingPotionConsumed.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                self.body.AddTimedBuff(PowerElixir.regenBuff, 8f);
                EffectData effectData = new EffectData
                {
                    origin = self.transform.position
                };
                effectData.SetNetworkedObjectReference(self.gameObject);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/HealingPotionEffect"), effectData, true);
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, DLC1Content.Items.HealingPotion);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.HealingPotion);
            SneedUtils.SneedUtils.RemoveItemTag(DLC1Content.Items.HealingPotion, ItemTag.LowHealth);
        }
    }
}
