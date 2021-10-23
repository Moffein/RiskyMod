using RoR2;
using R2API;
using Risky_Mod.Items.Uncommon;
using Risky_Mod.Items.Common;
using Risky_Mod.Items.Boss;
using UnityEngine;
using Risky_Mod.Items.Lunar;
using Risky_Mod.Items.Legendary;

namespace Risky_Mod.SharedHooks
{
    public class GetStatsCoefficient
    {
        public static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (Chronobauble.enabled && sender.HasBuff(RoR2Content.Buffs.Slow60))
            {
                //Slow -60% already present in vanilla
                args.damageMultAdd -= 0.3f; //Might need to check for negative damage mult, come back to this later if there's problems.
                args.armorAdd -= 30f;
            }
            if (Warbanner.enabled && sender.HasBuff(RoR2Content.Buffs.Warbanner))
            {
                //+30% AtkSpd and MoveSpd already present in vanilla
                args.armorAdd += 15f;
                args.damageMultAdd += 0.15f;
            }
            if (Headhunter.enabled && sender.HasBuff(Headhunter.headhunterBuff))
            {
                args.moveSpeedMultAdd += 0.3f;
                args.damageMultAdd += 0.3f;
            }
            if (Berzerker.enabled)
            {
                int berzerkCount = sender.GetBuffCount(Berzerker.berzerkBuff);
                if (berzerkCount > 0)
                {
                    args.moveSpeedMultAdd += 0.1f * berzerkCount;
                    args.attackSpeedMultAdd += 0.1f * berzerkCount;
                }
            }

            if (sender.inventory)
            {
                if (CritGlasses.enabled)
                {
                    int glassesCount = sender.inventory.GetItemCount(RoR2Content.Items.CritGlasses);
                    if (glassesCount > 0)
                    {
                        args.critAdd += 7f * glassesCount;
                    }
                }
                if (BisonSteak.enabled)
                {
                    int steakCount = sender.inventory.GetItemCount(RoR2Content.Items.FlatHealth);
                    if (steakCount > 0)
                    {
                        args.baseHealthAdd += 33f * steakCount;
                    }
                }
                if (ShapedGlass.enabled)
                {
                    int glassCount = sender.inventory.GetItemCount(RoR2Content.Items.LunarDagger);
                    if (glassCount > 0)
                    {
                        args.damageMultAdd += glassCount;
                    }
                }
                if (Knurl.enabled)
                {
                    int knurlCount = sender.inventory.GetItemCount(RoR2Content.Items.Knurl);
                    if (knurlCount > 0)
                    {
                        args.healthMultAdd += 0.1f * knurlCount;
                        args.armorAdd += 10f * knurlCount;
                        args.baseRegenAdd += (1.6f + 0.32f*(sender.level - 1f)) * knurlCount;
                    }
                }
                if (RoseBuckler.enabled)
                {
                    int bucklerCount = sender.inventory.GetItemCount(RoR2Content.Items.SprintArmor);
                    if (bucklerCount > 0)
                    {
                        args.armorAdd += 12f * bucklerCount;
                    }
                }
                if (RepArmor.enabled)
                {
                    int rapCount = sender.inventory.GetItemCount(RoR2Content.Items.ArmorPlate);
                    if (rapCount > 0)
                    {
                        args.armorAdd += 5f * rapCount;
                    }
                }
            }
        }
    }
}
