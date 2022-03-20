using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Uncommon
{
    public class Harpoon
    {
        public static bool enabled = true;
        public Harpoon()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "MoveSpeedOnKill")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };


            AssistManager.HandleAssistInventoryActions += OnKillEffect;
        }

        private static void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            int itemCount = attackerInventory.GetItemCount(DLC1Content.Items.MoveSpeedOnKill);
            if (itemCount > 0)
            {
                int num5 = itemCount - 1;
                int num6 = 5;
                float num7 = 3f + (float)num5 * 1.5f;
                attackerBody.ClearTimedBuffs(DLC1Content.Buffs.KillMoveSpeed);
                for (int l = 0; l < num6; l++)
                {
                    attackerBody.AddTimedBuff(DLC1Content.Buffs.KillMoveSpeed, num7 * (float)(l + 1) / (float)num6);
                }
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
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MoveSpeedOnKillActivate"), effectData, true);
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.MoveSpeedOnKill);
        }
    }
}
