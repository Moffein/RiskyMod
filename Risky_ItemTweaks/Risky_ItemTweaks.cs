using R2API;
using BepInEx;
using RoR2;
using R2API.Utils;
using Risky_ItemTweaks.Items.Uncommon;
using Risky_ItemTweaks.Items.Common;
using Risky_ItemTweaks.SharedHooks;
using Risky_ItemTweaks.Items.Boss;
using Risky_ItemTweaks.Items.Lunar;
using Risky_ItemTweaks.Items.Legendary;
using UnityEngine;
using Risky_ItemTweaks.Tweaks;

namespace Risky_ItemTweaks
{
    [BepInDependency("com.bepis.r2api")]
    [BepInDependency("com.PlasmaCore.StickyStunter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("com.Moffein.Risky_ItemTweaks", "Risky ItemTweaks", "1.0.0")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(LanguageAPI), nameof(RecalculateStatsAPI), nameof(PrefabAPI),
        nameof(ProjectileAPI), nameof(EffectAPI), nameof(DamageAPI), nameof(BuffAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class Risky_ItemTweaks : BaseUnityPlugin
    {
        bool uncommonEnabled = true;
        bool commonEnabled = true;
        bool legendaryEnabled = true;
        bool bossEnabled = true;
        bool lunarEnabled = true;

        public static bool disableProcChains = true;

        public static ItemDef emptyItemDef = null;
        public static BuffDef emptyBuffDef = null;

        public void Awake()
        {
            ReadConfig();

            FixDamageTypeOverwrite.Modify();
            ShieldGating.Modify();

            Planula.enabled = Stealthkit.enabled || Razorwire.enabled || SquidPolyp.enabled;
            ModifyCommon();
            ModifyUncommon();
            ModifyLegendary();
            ModifyBoss();
            ModifyLunar();
            AddHooks();
        }

        private void ReadConfig()
        {

        }
        
        private void AddHooks()
        {
            //A hook needs to be used at least once to be added
            if (LeechingSeed.enabled || ElementalBands.enabled || Shatterspleen.enabled || 
                (Risky_ItemTweaks.disableProcChains && (MeatHook.enabled || ChargedPerf.enabled)))
            {
                On.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemy.GlobalEventManager_OnHitEnemy;
            }
            if (Chronobauble.enabled || CritGlasses.enabled || BisonSteak.enabled || ShapedGlass.enabled || Knurl.enabled || Warbanner.enabled
                || RoseBuckler.enabled || RepArmor.enabled || Headhunter.enabled || Berzerker.enabled || FixDamageTypeOverwrite.enabled)
            {
                RecalculateStatsAPI.GetStatCoefficients += GetStatsCoefficient.RecalculateStatsAPI_GetStatCoefficients;
            }
            if (Bandolier.enabled || RoseBuckler.enabled || ShieldGating.enabled)
            {
                On.RoR2.CharacterBody.RecalculateStats += RecalculateStats.CharacterBody_RecalculateStats;
            }
            if (Stealthkit.enabled || SquidPolyp.enabled || Crowbar.enabled || Razorwire.enabled || Planula.enabled)
            {
                On.RoR2.HealthComponent.TakeDamage += TakeDamage.HealthComponent_TakeDamage;
            }
            if (WillOWisp.enabled || Shatterspleen.enabled || Headhunter.enabled || Berzerker.enabled)
            {
                On.RoR2.GlobalEventManager.OnCharacterDeath += OnCharacterDeath.GlobalEventManager_OnCharacterDeath;
            }
            if (Guillotine.enabled || Headhunter.enabled)
            {
                ModifyFinalDamage.Modify();
            }
        }

        private void ModifyCommon()
        {
            if (!commonEnabled) return;
            BisonSteak.Modify();
            MonsterTooth.Modify();
            CritGlasses.Modify();
            Fireworks.Modify();
            StickyBomb.Modify();
            Crowbar.Modify();
            Warbanner.Modify();
            Gasoline.Modify();
            RepArmor.Modify();
        }

        private void ModifyUncommon()
        {
            if (!uncommonEnabled) return;
            Predatory.Modify();
            Chronobauble.Modify();
            LeechingSeed.Modify();
            AtG.Modify();
            ElementalBands.Modify();
            Bandolier.Modify();
            Stealthkit.Modify();
            WillOWisp.Modify();
            SquidPolyp.Modify();
            Ukulele.Modify();
            Razorwire.Modify();
            RoseBuckler.Modify();
            Guillotine.Modify();
            Berzerker.Modify();
        }

        private void ModifyLegendary()
        {
            if (!legendaryEnabled) return;
            Tesla.Modify();
            FrostRelic.Modify();
            CeremonialDagger.Modify();
            MeatHook.Modify();
            LaserTurbine.Modify();
            Headhunter.Modify();
            Headstompers.Modify();
            NovaOnHeal.Modify();
        }

        private void ModifyBoss()
        {
            if (!bossEnabled) return;
            QueensGland.Modify();
            MoltenPerf.Modify();
            ChargedPerf.Modify();
            Shatterspleen.Modify();
            Knurl.Modify();
            Disciple.Modify();
            Planula.Modify();
            GenesisLoop.Modify();
        }

        private void ModifyLunar()
        {
            if (!lunarEnabled) return;
            ShapedGlass.Modify();
        }

        public void AddToAIBlacklist(string itemName)
        {
            ItemIndex i = ItemCatalog.FindItemIndex(itemName);
            if (i != ItemIndex.None)
            {
                AddToAIBlacklist(i);
            }
        }

        public static void AddToAIBlacklist(ItemIndex index)
        {
            //Debug.Log("Adding BrotherBlacklist tag to ItemIndex " + index);
            ItemDef itemDef = ItemCatalog.GetItemDef(index);
            if (itemDef.DoesNotContainTag(ItemTag.BrotherBlacklist))
            {
                System.Array.Resize(ref itemDef.tags, itemDef.tags.Length + 1);
                itemDef.tags[itemDef.tags.Length - 1] = ItemTag.BrotherBlacklist;
            }
        }
    }
}
