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
            RoR2.Stage.onStageStartGlobal += StageStartResetElixir;
        }

        private void StageStartResetElixir(Stage obj)
        {
            if (CharacterMaster.instancesList != null)
            {
                foreach (CharacterMaster cm in CharacterMaster.instancesList)
                {
                    if (cm.inventory)
                    {
                        int brokenElixirCount = cm.inventory.GetItemCountPermanent(DLC1Content.Items.HealingPotionConsumed);
                        if (brokenElixirCount > 0)
                        {
                            cm.inventory.RemoveItemPermanent(DLC1Content.Items.HealingPotionConsumed, brokenElixirCount);
                            cm.inventory.GiveItemPermanent(DLC1Content.Items.HealingPotion, brokenElixirCount);
                            CharacterMasterNotificationQueue.SendTransformNotification(cm, DLC1Content.Items.HealingPotionConsumed.itemIndex, DLC1Content.Items.HealingPotion.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                        }
                    }
                }
            }
        }

        private static GameObject healEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/HealingPotion/HealingPotionEffect.prefab").WaitForCompletion();
        private static void TriggerElixir (HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
        {
            if (self.itemCounts.healingPotion > 0)
            {
                if (self.combinedHealthFraction <= 0.5f)
                {
                    bool removed = false;

                    //Check temp first
                    int itemCountTemp = self.body.inventory.GetItemCountTemp(DLC1Content.Items.HealingPotion);
                    if (itemCountTemp > 0)
                    {
                        removed = true;
                        self.body.inventory.RemoveItemTemp(DLC1Content.Items.HealingPotion.itemIndex);
                        self.body.inventory.GiveItemTemp(DLC1Content.Items.HealingPotionConsumed.itemIndex);
                    }

                    int itemCountPermanent = self.body.inventory.GetItemCountPermanent(DLC1Content.Items.HealingPotion);
                    if (!removed &&  itemCountPermanent > 0)
                    {
                        removed = true;
                        self.body.inventory.RemoveItemPermanent(DLC1Content.Items.HealingPotion.itemIndex);
                        self.body.inventory.GiveItemPermanent(DLC1Content.Items.HealingPotionConsumed.itemIndex);
                    }

                    if (removed)
                    {
                        self.AddBarrier(self.fullCombinedHealth * 0.5f);
                        CharacterMasterNotificationQueue.SendTransformNotification(self.body.master, DLC1Content.Items.HealingPotion.itemIndex, DLC1Content.Items.HealingPotionConsumed.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                        EffectData effectData = new EffectData
                        {
                            origin = self.transform.position
                        };
                        effectData.SetNetworkedObjectReference(self.gameObject);
                        EffectManager.SpawnEffect(healEffect, effectData, true);
                    }
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
