using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using RoR2.Orbs;
using System;
using UnityEngine;
using UnityEngine.Networking;
using static RiskyMod.Assets;

namespace RiskyMod.Items.Uncommon
{
    public class Infusion
    {
        public static bool enabled = true;
        public static BuffDef InfusionBuff;
        public Infusion()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Infusion")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            AssistManager.HandleAssistInventoryActions += OnKillEffect;

            InfusionBuff = ScriptableObject.CreateInstance<BuffDef>();
            InfusionBuff.buffColor = Color.white;
            InfusionBuff.canStack = true;
            InfusionBuff.isDebuff = false;
            InfusionBuff.name = "RiskyMod_InfusionBuff";
            InfusionBuff.iconSprite = BuffIcons.Infusion;
            R2API.ContentAddition.AddBuffDef((InfusionBuff));

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                     x => x.MatchCallvirt<Inventory>("get_infusionBonus")
                    );
                c.Emit(OpCodes.Ldarg_0);//self
                c.EmitDelegate<Func<int, CharacterBody, int>>((infusionBonus, self) =>
                {
                    float newHP = 0;
                    float infusionCount = (float)infusionBonus;
                    int hundredsFulfilled = 0;
                    while (infusionCount > 0f)
                    {
                        float killRequirement = 100f + 150f * hundredsFulfilled;
                        if (infusionBonus <= killRequirement)
                        {
                            newHP += 100f * infusionBonus / killRequirement;
                            infusionCount = 0f;
                        }
                        else
                        {
                            infusionCount-= killRequirement;
                            newHP += 100f;
                            hundredsFulfilled++;
                        }
                    }
                    int hpGained = Mathf.FloorToInt(newHP);
                    if (NetworkServer.active)
                    {
                        int infusionBuffCount = self.GetBuffCount(InfusionBuff.buffIndex);
                        if (hpGained != infusionBuffCount)
                        {
                            if (hpGained > infusionBuffCount)
                            {
                                for (int i = 0; i < hpGained - infusionBuffCount; i++)
                                {
                                    self.AddBuff(InfusionBuff.buffIndex);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < infusionBuffCount - hpGained; i++)
                                {
                                    self.RemoveBuff(InfusionBuff.buffIndex);
                                }
                            }
                        }
                    }
                    return hpGained;
                });
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Infusion);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Infusion);
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.Infusion);
            if (itemCount > 0)
            {
                if ((victimBody.bodyFlags & CharacterBody.BodyFlags.Masterless) != CharacterBody.BodyFlags.Masterless)
                {
                    InfusionOrb infusionOrb = new InfusionOrb();
                    infusionOrb.origin = victimBody.corePosition;
                    infusionOrb.target = Util.FindBodyMainHurtBox(attackerBody);
                    infusionOrb.maxHpValue = itemCount;
                    OrbManager.instance.AddOrb(infusionOrb);
                }
            }
        }
    }
}
