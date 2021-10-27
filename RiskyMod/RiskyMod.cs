using R2API;
using BepInEx;
using RoR2;
using R2API.Utils;
using RiskyMod.Items.Uncommon;
using RiskyMod.Items.Common;
using RiskyMod.SharedHooks;
using RiskyMod.Items.Boss;
using RiskyMod.Items.Lunar;
using RiskyMod.Items.Legendary;
using UnityEngine;
using RiskyMod.Tweaks;
using RiskyMod.Fixes;
using RiskyMod.Items;
using RiskyMod.Drones;

namespace RiskyMod
{
    [BepInDependency("com.PlasmaCore.StickyStunter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.MercExposeFix", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.FixPlayercount", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoLevelupHeal", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.RiskyLives.RiskyMod", "RiskyMod", "1.0.0")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(LanguageAPI), nameof(RecalculateStatsAPI), nameof(PrefabAPI),
        nameof(ProjectileAPI), nameof(EffectAPI), nameof(DamageAPI), nameof(BuffAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class RiskyMod : BaseUnityPlugin
    {
        public static bool disableProcChains = true;

        public static ItemDef emptyItemDef = null;
        public static BuffDef emptyBuffDef = null;

        public void Awake()
        {
            ReadConfig();
            RunFixes();
            RunTweaks();
            new ItemsCore();
            new DronesCore();

            AddHooks();
        }

        private void ReadConfig()
        {

        }

        private void RunTweaks()
        {
            new RunScaling();
            new ShieldGating();
            new SceneDirectorMonsterRewards();
        }

        private void RunFixes()
        {
            if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.MercExposeFix"))
            {
                new FixMercExpose();
            }
            if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.FixPlayercount"))
            {
                new FixPlayercount();
            }
            else
            {
                FixPlayercount.standalonePluginLoaded = true;
            }
            if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoLevelupHeal"))
            {
                new FixLevelupHeal();
            }
            new FixDamageTypeOverwrite();
        }
        
        private void AddHooks()
        {
            //A hook needs to be used at least once to be added
            if (LeechingSeed.enabled)
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
            if (Headhunter.enabled || Berzerker.enabled)
            {
                On.RoR2.GlobalEventManager.OnCharacterDeath += OnCharacterDeath.GlobalEventManager_OnCharacterDeath;
            }
            if (Guillotine.enabled || Headhunter.enabled)
            {
                ModifyFinalDamage.Modify();
            }
        }
    }
}
