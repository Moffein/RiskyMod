using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using RoR2.Orbs;
using System;
using UnityEngine;

namespace RiskyMod.Items.Uncommon
{
    public class Infusion
    {
        public static bool enabled = true;
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
                c.EmitDelegate<Func<float, float>>(infusionBonus =>
                {
                    float newHP = 0;
                    int hundredsFulfilled = 0;
                    while (infusionBonus > 0f)
                    {
                        float killRequirement = 100f + 150f * hundredsFulfilled;
                        if (infusionBonus <= killRequirement)
                        {
                            newHP += infusionBonus * 100f / killRequirement;
                            infusionBonus = 0f;
                        }
                        else
                        {
                            infusionBonus -= killRequirement;
                            newHP += 100f;
                            hundredsFulfilled++;
                        }
                    }
                    return newHP;
                });
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
