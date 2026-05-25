using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.DLC3.Boss
{
    public class PrisonMatrix
    {
        public static bool enabled = true;

        public PrisonMatrix()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            GetStatCoefficients.HandleStatsInventoryActions += HandleStatsInventory;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool error = true;
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC3Content.Buffs), "PowerCubeBuff")))
            {
                c.EmitDelegate<Func<BuffDef, BuffDef>>(x => null);

                if (c.TryGotoNext(x => x.MatchRet()))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Action<CharacterBody>>(body =>
                    {
                        if (body.HasBuff(DLC3Content.Buffs.PowerCubeBuff))
                        {
                            body.armor *= 1.5f;
                        }
                    });
                    error = false;
                }
            }

            if (error)
            {
                Debug.LogError("RiskyMod: Prison Matrix IL Hook failed.");
            }
        }

        private void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            if (sender.inventory.GetItemCountEffective(DLC3Content.Items.PowerCube) > 0)
            {
                args.armorAdd += 10;
            }
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, DLC3Content.Items.PowerCube);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC3Content.Items.PowerCube);
        }
    }
}
