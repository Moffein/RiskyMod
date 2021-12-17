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
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Infusion);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Infusion);

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

            AssistManager.HandleAssistActions += OnKillEffect;

            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdloc(50),
                     x => x.MatchLdloc(40),
                     x => x.MatchConvRUn(),
                     x => x.MatchConvR4(),
                     x => x.MatchAdd(),
                     x => x.MatchStloc(50)
                    );
                c.Index += 4;
                c.Emit(OpCodes.Ldarg_0);//self
                c.EmitDelegate<Func<float, CharacterBody, float>>((infusionBonus, self) =>
                {
                    float newHP = 0;
                    int hundredsFulfilled = 0;
                    while (infusionBonus > 0f)
                    {
                        float killRequirement = 100f + 150f * hundredsFulfilled;
                        if (infusionBonus <= killRequirement)
                        {
                            newHP += 100f * infusionBonus / killRequirement;
                            infusionBonus = 0f;
                        }
                        else
                        {
                            infusionBonus -= killRequirement;
                            newHP += 100f;
                            hundredsFulfilled++;
                        }
                    }
                    if (NetworkServer.active)
                    {
                        while (self.HasBuff(InfusionBuff.buffIndex))
                        {
                            self.RemoveBuff(InfusionBuff.buffIndex);
                        }
                        if (self.inventory.GetItemCount(RoR2Content.Items.Infusion) > 0)
                        {
                            int hpGained = Mathf.FloorToInt(newHP);
                            for (int i = 0; i < hpGained; i++)
                            {
                                self.AddBuff(InfusionBuff.buffIndex);
                            }
                        }
                    }
                    return newHP;
                });

                InfusionBuff = ScriptableObject.CreateInstance<BuffDef>();
                InfusionBuff.buffColor = Color.white;
                InfusionBuff.canStack = true;
                InfusionBuff.isDebuff = false;
                InfusionBuff.name = "RiskyMod_InfusionBuff";
                InfusionBuff.iconSprite = BuffIcons.Infusion;
                BuffAPI.Add(new CustomBuff(InfusionBuff));
            };
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
