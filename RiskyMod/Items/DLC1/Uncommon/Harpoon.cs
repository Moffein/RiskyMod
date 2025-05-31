﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;

namespace RiskyMod.Items.DLC1.Uncommon
{
    public class Harpoon
    {
        public static bool enabled = true;
        private static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MoveSpeedOnKill/MoveSpeedOnKillActivate.prefab").WaitForCompletion();
        public static BuffDef ReturnsHarpoonBuff;
        public Harpoon()
        {
            if (!enabled) return;
            AssistManager.VanillaTweaks.Harpoon.Instance.SetEnabled(false);
            BuffDef originalHarpoonBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/DLC1/MoveSpeedOnKill/bdKillMoveSpeed.asset").WaitForCompletion();

            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "MoveSpeedOnKill")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Harpoon IL Hook failed");
                }
            };

            ItemsCore.ModifyItemDefActions += ModifyItem;
            AssistManager.AssistManager.HandleAssistInventoryCompatibleActions += HandleAssist;
            SharedHooks.OnCharacterDeath.OnCharacterDeathInventoryActions += ProcItem;

            ReturnsHarpoonBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_ReturnsHarpoonBuff",
                true,
                false,
                false,
                originalHarpoonBuff.buffColor,
                originalHarpoonBuff.iconSprite
                );

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void ProcItem(GlobalEventManager self, DamageReport damageReport, CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody)
        {
            OnKillEffect(attackerBody, attackerInventory);
        }

        private void HandleAssist(CharacterBody attackerBody, CharacterBody victimBody, DamageType? assistDamageType, DamageTypeExtended? assistDamageTypeExtended, DamageSource? assistDamageSource, System.Collections.Generic.HashSet<DamageAPI.ModdedDamageType> assistModdedDamageTypes, Inventory attackerInventory, CharacterBody killerBody, DamageInfo damageInfo)
        {
            if (attackerBody == killerBody) return;
            OnKillEffect(attackerBody, attackerInventory);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(Harpoon.ReturnsHarpoonBuff))
            {
                args.moveSpeedMultAdd += 1.25f;
            }
        }

        private static void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory)
        {
            int itemCount = attackerInventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
            if (itemCount > 0)
            {
                int buffsToGive = itemCount;
                int currentBuffs = attackerBody.GetBuffCount(Harpoon.ReturnsHarpoonBuff);

                SneedUtils.SneedUtils.AddCooldownBuff(attackerBody, Harpoon.ReturnsHarpoonBuff, Mathf.Min(25, buffsToGive + currentBuffs), 0.5f);

                EffectData effectData = new EffectData();
                effectData.origin = attackerBody.corePosition;
                CharacterMotor characterMotor = attackerBody.characterMotor;
                bool flag = false;
                if (characterMotor)
                {
                    Vector3 moveDirection = characterMotor.moveDirection;
                    if (moveDirection != Vector3.zero)
                    {
                        effectData.rotation = Util.QuaternionSafeLookRotation(moveDirection);
                        flag = true;
                    }
                }
                if (!flag)
                {
                    effectData.rotation = attackerBody.transform.rotation;
                }
                EffectManager.SpawnEffect(Harpoon.effectPrefab, effectData, true);
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.MoveSpeedOnKill);
        }
    }
}
