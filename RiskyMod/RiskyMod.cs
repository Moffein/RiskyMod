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
using RiskyMod.Items.Equipment;
using UnityEngine.Networking;
using RiskyMod.MonoBehaviours;
using RiskyMod.MoonRework;
using Zio;
using Zio.FileSystems;
using System.Collections.Generic;

namespace RiskyMod
{
    [BepInDependency("com.PlasmaCore.StickyStunter", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.Moffein.MercExposeFix", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.FixPlayercount", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoLevelupHeal", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AI_Blacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dev.wildbook.multitudes", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TPDespair.ZetArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rob.ArtifactReliquaryHealingFix", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RaiseMonsterLevelCap", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.PlasmaCore.AlwaysAllowSupplyDrops", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.Hypercrit", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.RiskyLives.RiskyMod", "RiskyMod Beta", "0.1.3")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(RecalculateStatsAPI), nameof(PrefabAPI),
        nameof(ProjectileAPI), nameof(EffectAPI), nameof(DamageAPI), nameof(BuffAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class RiskyMod : BaseUnityPlugin
    {
        public static bool disableProcChains = true;

        public static ItemDef emptyItemDef = null;
        public static BuffDef emptyBuffDef = null;

        public static AssistManager assistManager = null;

        public static PluginInfo pluginInfo;
        public static FileSystem fileSystem { get; private set; }

        public void Awake()
        {
            pluginInfo = Info;

            CheckDependencies();
            ReadConfig();
            RunFixes();
            RunTweaks();
            new ItemsCore();
            new DronesCore();
            new MoonReworkCore();
            SetupAssists();
            AddHooks();
            FunnyLanguage();
        }

        private void ReadConfig()
        {

        }

        private void CheckDependencies()
        {
            NoLevelupHeal.enabled = !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoLevelupHeal");
            RemoveLevelCap.enabled = !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RaiseMonsterLevelCap");
            FixMercExpose.enabled = !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.MercExposeFix");

            FixPlayercount.enabled = !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.FixPlayercount");
            FixPlayercount.MultitudesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.wildbook.multitudes");
            FixPlayercount.ZetArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetArtifacts");
            FixVengeanceLeveling.enabled = !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AI_Blacklist");
            PreventArtifactHeal.enabled = !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.ArtifactReliquaryHealingFix");
            CaptainOrbitalHiddenRealms.enabled = !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.PlasmaCore.AlwaysAllowSupplyDrops");
            CritHud.enabled = !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.Hypercrit");   //Effect is already a part of hypercrit
        }

        private void RunTweaks()
        {
            new RunScaling();
            new TrueOSP();
            new ShieldGating();
            new SceneDirectorMonsterRewards();
            new BanditSpecialGracePeriod();
            new NoLevelupHeal();
            new RemoveLevelCap();
            new SmallHoldoutCharging();
            new TeleExpandOnBossKill();
            new CaptainOrbitalHiddenRealms();
            new Shock();
            new DistantRoostCredits();
            new ShrineCombatItems();
        }

        private void RunFixes()
        {
            new FixMercExpose();
            new FixPlayercount();
            new FixVengeanceLeveling();
            new FixDamageTypeOverwrite();
            new FixFocusCrystalSelfDamage();
            new PreventArtifactHeal();
        }
        
        private void AddHooks()
        {
            //A hook needs to be used at least once to be added
            On.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemy.GlobalEventManager_OnHitEnemy;
            RecalculateStatsAPI.GetStatCoefficients += GetStatsCoefficient.RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += RecalculateStats.CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += TakeDamage.HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnCharacterDeath += OnCharacterDeath.GlobalEventManager_OnCharacterDeath;

            //GlobalEventManager.onCharacterDeathGlobal += OnCharacterDeath.GlobalEventManager_onCharacterDeathGlobal; //Event subscription instead of On. Hook
            //I am unable to test anything right now, so its commented
            new ModifyFinalDamage();
            new StealBuffVFX();
        }

        private void SetupAssists()
        {
            AssistManager.initialized = true;
            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    RiskyMod.assistManager = self.gameObject.GetComponent<AssistManager>();
                    if (!RiskyMod.assistManager)
                    {
                        RiskyMod.assistManager = self.gameObject.AddComponent<AssistManager>();
                    }
                }
            };

            //RoR2.Run.onRunStartGlobal += Run_onRunStartGlobal; Same as with onCharacterDeathGlobal, gotta test this code remplacement whenever I get home
        }

        private void Run_onRunStartGlobal(Run obj)
        {
            if (NetworkServer.active)
            {
                RiskyMod.assistManager = obj.gameObject.GetComponent<AssistManager>();
                if (!RiskyMod.assistManager)
                {
                    RiskyMod.assistManager = obj.gameObject.AddComponent<AssistManager>();
                }
            }
        }

        private void FunnyLanguage()
        {
            PhysicalFileSystem physicalFileSystem = new PhysicalFileSystem();
            RiskyMod.fileSystem = new SubFileSystem(physicalFileSystem, physicalFileSystem.ConvertPathFromInternal(Assets.assemblyDir), true);
            if (RiskyMod.fileSystem.DirectoryExists("/language/")) //Uh, it exists and we make sure to not shit up R2Api
            {
                Language.collectLanguageRootFolders += delegate (List<DirectoryEntry> list)
                {
                    list.Add(RiskyMod.fileSystem.GetDirectoryEntry("/language/"));
                };
            }
        }
    }
}
