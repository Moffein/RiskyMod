using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class Raincoat
    {
        public static bool enabled = true;
        public static BuffDef RaincoatReadyBuff;
        public static BuffDef RaincoatActiveBuff;
        public static BuffDef RaincoatCooldownBuff;
        public Raincoat()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            DisableVanillaBehavior();
            HookTimedDebuffs();
            SetupBuffs();
        }

        public static bool ProcRaincoat(CharacterBody body)
        {
            if (body.HasBuff(RaincoatActiveBuff))
            {
                return true;
            }
            else if (body.HasBuff(RaincoatReadyBuff))
            {
                int itemCount = body.inventory ? body.inventory.GetItemCount(DLC1Content.Items.ImmuneToDebuff) : 0;
                if (itemCount > 0)
                {
                    body.RemoveBuff(RaincoatReadyBuff);
                    body.AddTimedBuff(RaincoatActiveBuff, 4f);
                }
                return true;
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
                new Color(214f / 255f, 201f / 255f, 58f / 255f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/Nullified").iconSprite
                );

            RaincoatActiveBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_RaincoatActive",
                false,
                false,
                false,
                new Color(214f / 255f, 201f / 255f, 58f / 255f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite
                );

            RaincoatCooldownBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_RaincoatCooldown",
                false,
                true,
                false,
                new Color(88f / 255f, 91f / 255f, 98f / 255f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/CloakSpeed").iconSprite
                );

            //Raincoat Active
            RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender.HasBuff(RaincoatActiveBuff))
                {
                    args.moveSpeedMultAdd += 0.5f;
                }
            };

            //Cooldown Start
            On.RoR2.CharacterBody.RemoveBuff_BuffIndex += (orig, self, buffType) =>
            {
                orig(self, buffType);

                if (buffType == RaincoatActiveBuff.buffIndex)
                {
                    if (!self.HasBuff(RaincoatActiveBuff))
                    {
                        float cooldown = 20f;
                        int itemCount = self.inventory ? self.inventory.GetItemCount(DLC1Content.Items.ImmuneToDebuff) : 0;
                        if (itemCount > 1)
                        {
                            cooldown = Mathf.Max(cooldown * Mathf.Pow(0.8f, itemCount - 1), 1f);
                        }
                        int stacksToApply = Mathf.CeilToInt(cooldown);
                        for (int i = 0; i<stacksToApply;i++)
                        {
                            self.AddTimedBuff(RaincoatCooldownBuff, i == 0 ? cooldown : stacksToApply - i);
                        }
                    }
                }
            };

            //Cooldown Expired
            On.RoR2.CharacterBody.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (self.inventory && self.inventory.GetItemCount(DLC1Content.Items.ImmuneToDebuff) > 0)
                {
                    if (!(self.HasBuff(RaincoatActiveBuff) || self.HasBuff(RaincoatCooldownBuff) || self.HasBuff(RaincoatReadyBuff)))
                    {
                        self.AddBuff(RaincoatReadyBuff);
                    }
                }
            };
        }

        private void HookTimedDebuffs()
        {
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += (orig, self, buffDef, duration) =>
            {
                if (buffDef.isDebuff)
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
                if (buffDef.isDebuff)
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
                if (inflictDotInfo.victimObject)
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
