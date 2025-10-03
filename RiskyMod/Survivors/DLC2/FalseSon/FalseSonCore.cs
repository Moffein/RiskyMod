using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.DLC2.FalseSon
{
    public class FalseSonCore
    {
        public static bool enabled;
        public static bool modifyBaseStats;
        public static bool increaseGrowthCost;
        public static bool modifyPassive;
        public static bool buffLaser;

        public FalseSonCore()
        {
            if (!enabled) return;

            ModifyBody();
            ModifyPassive();
            BuffLaser();
        }

        private void ModifyBody()
        {
            GameObject bodyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/FalseSon/FalseSonBody.prefab").WaitForCompletion();
            CharacterBody body = bodyObject.GetComponent<CharacterBody>();

            if (modifyBaseStats)
            {
                body.baseMaxHealth = 160f;
                body.levelMaxHealth = 48f;
            }

            FalseSonController fsc = body.GetComponent<FalseSonController>();
            if (increaseGrowthCost)
            {
                fsc.growthStartCost = 24f;
                fsc.defaultGrowthLevelCostIncrease = 8f;
            }
        }

        private void ModifyPassive()
        {
            if (modifyPassive)
            {
                Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC2/FalseSon/FalseSonBodyTheTamperedHeart.asset").WaitForCompletion().skillDescriptionToken = "FALSESON_PASSIVE_DESCRIPTION_RISKYMOD";
                IL.RoR2.CharacterBody.ComputeTamperedHeartBonus += IncreaseArmor;
                IL.RoR2.CharacterBody.ComputeTamperedHeartBonus += DisableVanillaRegen;
                RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffCount = sender.GetBuffCount(DLC2Content.Buffs.TamperingMSAndHRBonus);
            if (buffCount > 0)
            {
                args.baseRegenAdd += 0.8f * buffCount;
                args.levelRegenAdd += 0.16f * buffCount;
            }
        }

        private void DisableVanillaRegen(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(FalseSonController), "tamperedHeartHealthRegen")))
            {
                c.EmitDelegate<Func<float, float>>(x => 0);
            }
            else
            {
                Debug.LogError("RiskyMod: FalseSon ReduceRegen IL hook failed.");
            }
        }

        private void IncreaseArmor(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(FalseSonController), "tamperedHeartArmor")))
            {
                c.EmitDelegate<Func<float, float>>(x => 5f);
            }
            else
            {
                Debug.LogError("RiskyMod: FalseSon ReduceRegen IL hook failed.");
            }
        }

        private void BuffLaser()
        {
            if (!buffLaser) return;

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/FalseSon/EntityStates.FalseSon.LaserFather.asset", "baseChargeDuration", "1.5");
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/FalseSon/EntityStates.FalseSon.LaserFather.asset", "baseDuration", "2.25");

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/FalseSon/EntityStates.FalseSon.LaserFatherCharged.asset", "minimumDuration", "2.25");
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/FalseSon/EntityStates.FalseSon.LaserFatherCharged.asset", "maximumDuration", "3");
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/FalseSon/EntityStates.FalseSon.LaserFatherCharged.asset", "baseFireFrequency", "10.666666667");//8/0.75
        }
    }
}
