using RoR2;
using R2API;
using UnityEngine;

namespace RiskyMod.Items.Legendary
{
    public class Clover
    {
        public static bool enabled = true;
        public Clover()
        {
            if (!enabled) return;

            On.RoR2.Util.CheckRoll_float_float_CharacterMaster += CapLuck;
            RecalculateStatsAPI.GetStatCoefficients += ConvertLuckToStatBonus;
        }

        private static bool CapLuck(On.RoR2.Util.orig_CheckRoll_float_float_CharacterMaster orig, float percentChance, float luck, RoR2.CharacterMaster effectOriginMaster)
        {
            return orig(percentChance, Mathf.Min(luck, 1f), effectOriginMaster);
        }

        private static void ConvertLuckToStatBonus(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.master && sender.master.luck > 1f)
            {
                float boost = sender.master.luck - 1f;
                float mult =  boost * 0.15f;
                args.healthMultAdd += mult;
                args.moveSpeedMultAdd += mult;
                args.regenMultAdd += mult;  //Irradiant Pearls add regen instead of multiplying according to the wiki
                args.damageMultAdd += mult;
                args.critAdd += 15f * boost;
                args.armorAdd += boost * 15f;   //Irradiant Pearls multiply armor instead of adding according to the wiki
            }
        }
    }
}
