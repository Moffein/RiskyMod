using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class Raincoat
    {
        public static bool enabled = true;
        public static bool replaceIcons = false;

        public static BuffDef DebuffImmune;

        public Raincoat()
        {
            if (replaceIcons)
            {
                On.RoR2.BuffCatalog.Init += (orig) =>
                {
                    orig();

                    DLC1Content.Buffs.ImmuneToDebuffCooldown.iconSprite = Content.Assets.BuffIcons.RaincoatCooldown;
                    DLC1Content.Buffs.ImmuneToDebuffReady.iconSprite = Content.Assets.BuffIcons.RaincoatReady;
                };
            }

            if (enabled)
            {
                ItemsCore.ModifyItemDefActions += ModifyItem;
                BuffDef powerDef = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/Common/bdSmallArmorBoost.asset").WaitForCompletion();
                Raincoat.DebuffImmune = SneedUtils.SneedUtils.CreateBuffDef(
                    "RiskyMod_RaincoatDebuffImmuneBuff",
                    false,
                    false,
                    false,
                    replaceIcons ? Color.white : new Color(0.839f, 0.788f, 0.227f),
                    replaceIcons ? Content.Assets.BuffIcons.RaincoatActive : powerDef.iconSprite
                    );

                On.RoR2.Items.ImmuneToDebuffBehavior.FixedUpdate += (orig, self) =>
                {
                    orig(self);

                    if (self.body && self.body.HasBuff(Raincoat.DebuffImmune))
                    {
                        self.isProtected = true;
                    }
                };

                IL.RoR2.Items.ImmuneToDebuffBehavior.TryApplyOverride += (il) =>
                {
                    bool error = true;
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(MoveType.After,
                         x => x.MatchLdcR4(0.1f)
                         ))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<float, CharacterBody, float>>((origBarrier, body) =>
                        {
                            if (!body.HasBuff(Raincoat.DebuffImmune))
                            {
                                body.AddTimedBuff(Raincoat.DebuffImmune, 0.5f);
                            }
                            return 0.25f;
                        });

                        if (c.TryGotoNext(
                         x => x.MatchLdcR4(5f)
                         ))
                        {
                            c.Next.Operand = 5.5f;
                            error = false;
                        }
                    }

                    if (error)
                    {
                        Debug.LogError("RiskyMod: Raincoat IL Hook failed.");
                    }
                };
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.ImmuneToDebuff);
        }
    }
}
