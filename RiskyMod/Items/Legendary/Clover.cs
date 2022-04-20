using RoR2;
using R2API;
using UnityEngine;

namespace RiskyMod.Items.Legendary
{
    public class Clover
    {
        public static bool enabled = true;

        public static float luckCap = 1f;

        public Clover()
        {
            if (!enabled) return;

            On.RoR2.Util.CheckRoll_float_float_CharacterMaster += CapLuck;
            RecalculateStatsAPI.GetStatCoefficients += ConvertLuckToStatBonus;
        }

        private static bool CapLuck(On.RoR2.Util.orig_CheckRoll_float_float_CharacterMaster orig, float percentChance, float luck, RoR2.CharacterMaster effectOriginMaster)
        {
            return orig(percentChance, Mathf.Min(luck, luckCap), effectOriginMaster);
        }

        private static void ConvertLuckToStatBonus(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.master && sender.master.luck > luckCap)
            {
                float boost = sender.master.luck - luckCap;
                float mult =  boost * 0.3f;
                float add = boost * 30f;
                args.healthMultAdd += mult;
                args.moveSpeedMultAdd += mult;
                args.regenMultAdd += mult;  //Irradiant Pearls add regen instead of multiplying according to the wiki
                args.damageMultAdd += mult;
                args.critAdd += add;
                args.armorAdd += add;   //Irradiant Pearls multiply armor instead of adding according to the wiki
            }
        }
    }
}
