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
using RiskyMod.Allies;
using RiskyMod.Items.Equipment;
using UnityEngine.Networking;
using RiskyMod.Moon;
using Zio;
using Zio.FileSystems;
using System.Collections.Generic;
using RiskyMod.Survivors;
using RiskyMod.Enemies;
using RiskyMod.Survivors.Captain;
using RiskyMod.Enemies.Bosses;
using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Croco;
using RiskyMod.Enemies.Mobs.Lunar;
using System.Runtime.CompilerServices;
using RiskyMod.Content;
using RiskyMod.Enemies.DLC1;
using RiskyMod.VoidLocus;
using RiskyMod.Tweaks.RunScaling;
using RiskyMod.Tweaks.Holdouts;
using RiskyMod.Tweaks.CharacterMechanics;
using RiskyMod.Tweaks.Artifact;
using RiskyMod.MonoBehaviours;
using RiskyMod.VoidFields;
using UnityEngine.AddressableAssets;
using RiskyMod.Enemies.DLC1.Voidling;
using static Generics.Dynamics.RigReader;
using RiskyMod.Enemies.Mithrix;
using RiskyMod.Survivors.Commando;
using RiskyMod.Content.Enemies;
using RiskyMod.Survivors.Croco.Tweaks;

namespace RiskyMod
{
    #region softdependencies
    [BepInDependency("com.Moffein.BlightReturnsRework", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AdditiveExecutes", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AssistManager", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(".AVFX_Options..", BepInDependency.DependencyFlags.SoftDependency)]    //Does this softdependency actually work? (since RiskyMod clones the projectiles)
    [BepInDependency("com.PlasmaCore.StickyStunter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.MobileTurretBuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.SacrificeTweaks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.Hypercrit", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.LunarWispFalloff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoVoidAllies", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.EliteReworks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Charzard4261.CaptainAbilitiesOffworld", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.EnigmaBlacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BackstabRework", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.funkfrog_sipondo.sharesuite", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.TeleExpansion", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TPDespair.ZetTweaks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.InteractableLimit", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.johnedwa.RTAutoSprintEx", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.Admiral", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.kking117.QueenGlandBuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.groovesalad.GrooveSaladSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.heyimnoob.NoopSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.plasmacore.PlasmaCoreSpikestripContent", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.Heretic", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Kingpinush.KingKombatArena", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RuneFoxMods.TeleporterTurrets", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.score.DirectorReworkPlus", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskOfBrainrot.MoreStats", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.TrueOSP", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.ArtificerPrimaryRework", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Nuxlar.EclipseRevamped", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskyLives.LinearDamage", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskyLives.RiskyMithrix", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RiskySkills", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskySleeps.MithrixWank", BepInDependency.DependencyFlags.SoftDependency)]
    #endregion

    [BepInDependency("com.RiskyLives.SneedHooks")]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2API.DamageAPI.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [BepInDependency(R2API.ItemAPI.PluginGUID)]
    [BepInDependency(R2API.ProcTypeAPI.PluginGUID)]
    [BepInDependency("com.Moffein.RiskyTweaks")]
    [BepInDependency("com.Moffein.RiskyFixes")]
    [BepInDependency("com.Moffein.AssistManager")]
    [BepInDependency("com.Moffein.DefenseMatrixManager")]
    [BepInPlugin("com.RiskyLives.RiskyMod", "RiskyMod", "2.9.7")]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class RiskyMod : BaseUnityPlugin
    {
        public static bool inBazaar = false;

        public static bool disableProcChains = true;

        public static ItemDef emptyItemDef = null;
        public static BuffDef emptyBuffDef = null;

        public static PluginInfo pluginInfo;
        public static FileSystem fileSystem { get; private set; }

        public static GameModeIndex classicRunIndex;
        public static GameModeIndex simulacrumIndex;

        private static SceneDef bazaarScene = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/bazaar/bazaar.asset").WaitForCompletion();

        public static PickupDropTable tier1Drops = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/Common/dtTier1Item.asset").WaitForCompletion();
        public static PickupDropTable tier2Drops = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/Common/dtTier2Item.asset").WaitForCompletion();
        public static PickupDropTable tier3Drops = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/Common/dtTier3Item.asset").WaitForCompletion();
        public static PickupDropTable lunarDrops = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/LunarChest/dtLunarChest.asset").WaitForCompletion();
        public static PickupDropTable tierVoidDrops = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/Common/dtVoidChest.asset").WaitForCompletion();
        public static GameObject potentialPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/OptionPickup/OptionPickup.prefab").WaitForCompletion();

        public void Start()
        {
            //Check for Inferno here since it has a RiskyMod softdependency.
            SoftDependencies.InfernoPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            SoftDependencies.SS2OLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2");
        }

        public void Awake()
        {
            //Check this first since config relies on it
            InitDependencies();

            RoR2.Stage.onStageStartGlobal += Stage_onStageStartGlobal;

            On.RoR2.GameModeCatalog.LoadGameModes += (orig) =>
            {
                orig();
                simulacrumIndex = GameModeCatalog.FindGameModeIndex("InfiniteTowerRun");
                classicRunIndex = GameModeCatalog.FindGameModeIndex("ClassicRun");
            };

            pluginInfo = Info;
            Content.Assets.Init();
            ConfigFiles.Init();
            CheckDependencies();    //More dependency checking

            ProjectileZapChainOnExplosion.Init();

            ContentCore.Init();
            new SharedDamageTypes();
            RunFixes();
            RunTweaks();
            new ItemsCore();
            new AlliesCore();
            new VoidFieldsCore();
            new MoonCore();
            new VoidLocusCore();
            new SurvivorsCore();
            new EnemiesCore();
            AddHooks();

            RoR2Application.onLoad += OnLoad;
        }

        private void OnLoad()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskySleeps.ClassicItemsReturns"))
            {
                Sacrifice.dropChance = 8f;
            }
        }

        private void Stage_onStageStartGlobal(Stage obj)
        {
            inBazaar = false;
            if (RoR2.SceneCatalog.GetSceneDefForCurrentScene() == bazaarScene)
            {
                inBazaar = true;
            }
        }

        //This isn't all the softdependency checks. Some still are in CheckDependencies, this is just a quick fix.
        private void InitDependencies()
        {
            SoftDependencies.MithrixWankLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskySleeps.MithrixWank");
            SoftDependencies.LinearDamageLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskyLives.LinearDamage");
            SoftDependencies.EclipseRevampedLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Nuxlar.EclipseRevamped");
            SoftDependencies.RiskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            SoftDependencies.ArtifactOfPotentialLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("zombieseatflesh7.ArtifactOfPotential");
            SoftDependencies.TeleporterTurretsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RuneFoxMods.TeleporterTurrets");
            SoftDependencies.KingKombatArenaLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Kingpinush.KingKombatArena");
            SoftDependencies.ZetTweaksLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetTweaks");
            SoftDependencies.AIBlacklistLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AI_Blacklist");
            SoftDependencies.QueensGlandBuffLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.kking117.QueenGlandBuff");
            SoftDependencies.ScepterPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            SoftDependencies.ShareSuiteLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.funkfrog_sipondo.sharesuite");
            SoftDependencies.RtAutoSprintLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.johnedwa.RTAutoSprintEx");
            SoftDependencies.SpikestripGrooveSalad = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.groovesalad.GrooveSaladSpikestripContent");
            SoftDependencies.SpikestripHeyImNoob = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.heyimnoob.NoopSpikestripContent");
            SoftDependencies.SpikestripPlasmaCore = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.plasmacore.PlasmaCoreSpikestripContent");
            SoftDependencies.MoreStatsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskOfBrainrot.MoreStats");
            SoftDependencies.RiskyMithrixLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskyLives.RiskyMithrix");
            SoftDependencies.RiskySkillsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RiskySkills");
            SoftDependencies.AdditiveExecutesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AdditiveExecutes");
            SoftDependencies.BlightReturnsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.BlightReturnsRework");
        }

        private void CheckDependencies()
        {
            if (SoftDependencies.KingKombatArenaLoaded)
            {
                RoR2.Stage.onStageStartGlobal += CheckArenaLoaded;
            }

            if (BlightReturns.enabled && SoftDependencies.BlightReturnsLoaded)
            {
                Debug.Log("RiskyMod: Disabling BlightReturns because standalone plugin is loaded.");
                BlightReturns.enabled = false;
            }

            if (SoftDependencies.ZetTweaksLoaded) ZetTweaksCompat();

            if (SoftDependencies.QueensGlandBuffLoaded && QueensGland.enabled) Debug.Log("RiskyMod: Disabling Queens Gland changes because QueensGlandBuff is loaded.");

            bool nvaPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoVoidAllies");
            if (nvaPluginLoaded && VoidInfestor.noVoidAllies) Debug.Log("RiskyMod: Disabling NoVoidAllies because standalone plugin is loaded.");
            VoidInfestor.noVoidAllies = VoidInfestor.noVoidAllies && !nvaPluginLoaded;

            bool hypercritPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.Hypercrit");
            if (hypercritPluginLoaded && CritHud.enabled) Debug.Log("RiskyMod: Disabling Ocular HUD changes because Hypercrit is loaded.");
            CritHud.enabled = CritHud.enabled && !hypercritPluginLoaded;   //Effect is already a part of hypercrit

            //Bandit2
            bool backstabReworkPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.BackstabRework");
            if (backstabReworkPluginLoaded && BackstabRework.enabled) Debug.Log("RiskyMod: Disabling Bandit2 BackstabRework because standalone plugin is loaded.");
            BackstabRework.enabled = BackstabRework.enabled && !backstabReworkPluginLoaded;

            //Should be fine if this doublestacks?
            //bool eliteReworksPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EliteReworks");
            //if (eliteReworksPluginLoaded && NullifyDebuff.enabled) Debug.Log("RiskyMod: Disabling Nullify Changes because EliteReworks is loaded.");
            //NullifyDebuff.enabled = NullifyDebuff.enabled && !eliteReworksPluginLoaded;

            if (SoftDependencies.ShareSuiteLoaded) HandleShareSuite();

            bool interactableLimitPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.InteractableLimit");
            if (interactableLimitPluginLoaded && SpawnLimits.enabled) Debug.Log("RiskyMod: Disabling Interactable SpawnLimits changes because InteractableLimit is loaded.");
            SpawnLimits.enabled = SpawnLimits.enabled && !interactableLimitPluginLoaded;

            bool teleExpansionLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.TeleExpansion");

            if (teleExpansionLoaded && TeleExpandOnBossKill.enabled) Debug.Log("RiskyMod: Disabling TeleExpandOnBossKill because TeleExpansion is loaded.");
            TeleExpandOnBossKill.enabled = TeleExpandOnBossKill.enabled && !teleExpansionLoaded;

            if (teleExpansionLoaded && VoidLocus.LargerHoldout.enabled) Debug.Log("RiskyMod: Disabling VoidLocus.ModifyHoldout because TeleExpansion is loaded.");
            VoidLocus.LargerHoldout.enabled = VoidLocus.LargerHoldout.enabled && !teleExpansionLoaded;

            if (teleExpansionLoaded && Moon.LargerHoldouts.enabled) Debug.Log("RiskyMod: Disabling Moon.ModifyHoldout because TeleExpansion is loaded.");
            Moon.LargerHoldouts.enabled = Moon.LargerHoldouts.enabled && !teleExpansionLoaded;

            Sacrifice.enabled = Sacrifice.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.SacrificeTweaks");

            bool directorReworkLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.score.DirectorReworkPlus");
            if (directorReworkLoaded) Debug.Log("RiskyMod: Disabling CombatDirectorMultiplier because DirectorReworkPlus is loaded.");
            CombatDirectorMultiplier.enabled = CombatDirectorMultiplier.enabled && !directorReworkLoaded;

            bool wolfoSpawnpoolFixerLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Wolfo.DLCSpawnPoolFixer");
            if (wolfoSpawnpoolFixerLoaded) Debug.Log("RiskyMod: Disabling DLC Enemy Replacement fix because DLCSpawnPoolFixer is loaded.");
            EnemiesCore.spawnpoolDLCReplacementFix = EnemiesCore.spawnpoolDLCReplacementFix && !wolfoSpawnpoolFixerLoaded;

            bool trueOSPLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.TrueOSP");
            if (trueOSPLoaded) Debug.Log("RiskyMod: Disabling True OSP because standalone plugin is loaded.");
            TrueOSP.enabled = TrueOSP.enabled && !trueOSPLoaded;

            bool artificerPrimaryReworkLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.ArtificerPrimaryRework");
            if (artificerPrimaryReworkLoaded) Debug.Log("RiskyMod: Disabling Artificer Primary changes because ArtificerPrimaryRework is loaded.");
            Survivors.Mage.MageCore.modifyFireBolt = Survivors.Mage.MageCore.modifyFireBolt && !artificerPrimaryReworkLoaded;
            Survivors.Mage.MageCore.modifyPlasmaBolt = Survivors.Mage.MageCore.modifyPlasmaBolt && !artificerPrimaryReworkLoaded;
            Survivors.Mage.SkillTweaks.PrimaryRework.enabled = Survivors.Mage.MageCore.modifyFireBolt || Survivors.Mage.MageCore.modifyPlasmaBolt;

            TeleExpandOnBossKill.enableDuringEclipse = TeleExpandOnBossKill.enableDuringEclipse || SoftDependencies.EclipseRevamped_CheckEclipse2Config();

            if (SoftDependencies.RiskyMithrixLoaded)
            {
                Debug.Log("RiskyMod: Disabling Mithrix changes because RiskyMithrix is loaded.");
                MithrixCore.enabled = false;
                MithrixDebuffResist.enabled = false;
                MithrixFreezeImmune.enabled = false;
                MithrixHealthBuff.enabled = false;
                MithrixTargetPrioritization.enabled = false;
            }

            if (SoftDependencies.RiskySkillsLoaded)
            {
                Debug.Log("RiskyMod: Disabling new skills because RiskyMithrix is loaded.");
                if (!CommandoCore.replacePhaseRound)
                {
                    CommandoCore.skillLightningRound = false;
                }

                if (!CommandoCore.replaceSuppressive)
                {
                    CommandoCore.skillShrapnelBarrage = false;
                }
            }

            if (SoftDependencies.MithrixWankLoaded)
            {
                Debug.Log("RiskyMod: Disabling Stage 5 Lunar Golems because MithrixWank is loaded.");
                LunarGolemSkyMeadow.enabled = false;
            }
        }

        private void CheckArenaLoaded(Stage obj)
        {
            if (SoftDependencies.KingKombatArenaLoaded) SetArena();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetArena()
        {
            SoftDependencies.KingKombatArenaActive = NS_KingKombatArena.KingKombatArenaMainPlugin.s_GAME_MODE_ACTIVE;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ZetTweaksCompat()
        {
            if (SoftDependencies.ZetTweaksLoaded)
            {
                //Allies.DroneChanges.MegaDrone.allowRepair = false;
                if (Allies.DroneChanges.MegaDrone.allowRepair) Debug.LogWarning("RiskyMod: MegaDrone.allowRepair is enabled while ZetTweaks is loaded. Disable this feature in the config of ZetTweaks or RiskyMod, or else you will see duplicated MegaDrone corpses.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void HandleAIBlacklist()
        {
            SoftDependencies.AIBlacklistUseVanillaBlacklist = AI_Blacklist.AIBlacklist.useVanillaAIBlacklist;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void HandleShareSuite()
        {
            SoftDependencies.ShareSuiteCommon = ShareSuite.ShareSuite.WhiteItemsShared.Value;
            SoftDependencies.ShareSuiteUncommon = ShareSuite.ShareSuite.GreenItemsShared.Value;
            SoftDependencies.ShareSuiteLegendary = ShareSuite.ShareSuite.RedItemsShared.Value;
            SoftDependencies.ShareSuiteBoss = ShareSuite.ShareSuite.BossItemsShared.Value;
            SoftDependencies.ShareSuiteLunar = ShareSuite.ShareSuite.LunarItemsShared.Value;
            SoftDependencies.ShareSuiteVoid = ShareSuite.ShareSuite.VoidItemsShared.Value;
        }

        private void RunTweaks()
        {
            //RunScaling
            new ModdedScaling();
            new MonsterGoldRewards();
            new CombatDirectorMultiplier();
            new LoopTeleMountainShrine();

            //Holdouts
            new TeleExpandOnBossKill();
            new TeleChargeDuration();
            new LargeLobbyScaling();

            //Interactables
            new SpawnLimits();

            //Character Mechanics
            new TrueOSP();
            new ShieldGating();
            new FreezeChampionExecute();
            new PlayerControlledMonsters();
            new NullifyDebuff();

            //Artifacts
            new Sacrifice();

            //Misc
            new AIBlacklistItems();
            new BetterProjectileTracking();
        }

        private void RunFixes()
        {
            new FixCrocoPoisonAchievement();
        }
        
        private void AddHooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients.RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += RecalculateStats.CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamageProcess += TakeDamage.HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnCharacterDeath += OnCharacterDeath.GlobalEventManager_OnCharacterDeath;
            On.RoR2.GlobalEventManager.OnHitAllProcess += OnHitAll.GlobalEventManager_OnHitAll;
            On.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime.UpdateLastHitTime;
            SharedHooks.TakeDamage.OnDamageTakenAttackerActions += TakeDamage.DistractOnHit;
        }
    }
}
