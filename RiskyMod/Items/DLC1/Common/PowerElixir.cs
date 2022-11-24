using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace RiskyMod.Items.DLC1.Common
{
    public class PowerElixir
    {
        public static bool enabled = true;

        public PowerElixir()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //TODO: FIND GOLD POTION ICON
            //ItemDef healPotionDef = Addressables.LoadAssetAsync<ItemDef>("RoR2/DLC1/HealingPotion/HealingPotion.asset").WaitForCompletion();

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
            if (self.itemCounts.healingPotion > 0)
            {
                if (self.combinedHealthFraction <= 0.5f)
                {
                    self.AddBarrier(self.fullCombinedHealth * 0.5f);

                    self.body.inventory.RemoveItem(DLC1Content.Items.HealingPotion, 1);
                    self.body.inventory.GiveItem(DLC1Content.Items.HealingPotionConsumed, 1);
                    CharacterMasterNotificationQueue.SendTransformNotification(self.body.master, DLC1Content.Items.HealingPotion.itemIndex, DLC1Content.Items.HealingPotionConsumed.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);

                    EffectData effectData = new EffectData
                    {
                        origin = self.transform.position
                    };
                    effectData.SetNetworkedObjectReference(self.gameObject);
                    EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/HealingPotionEffect"), effectData, true);
                }
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
