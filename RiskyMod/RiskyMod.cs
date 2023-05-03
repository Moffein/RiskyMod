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

namespace RiskyMod
{
    [BepInDependency(".AVFX_Options..", BepInDependency.DependencyFlags.SoftDependency)]    //Does this softdependency actually work? (since RiskyMod clones the projectiles)
    [BepInDependency("com.PlasmaCore.StickyStunter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.MobileTurretBuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.FixPlayercount", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoLevelupHeal", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AI_Blacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dev.wildbook.multitudes", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TPDespair.ZetArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.SacrificeTweaks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rob.ArtifactReliquaryHealingFix", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RaiseMonsterLevelCap", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.PlasmaCore.AlwaysAllowSupplyDrops", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.Hypercrit", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TheTimeSweeper.AcridHitboxBuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AcridBlightStack", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.LunarWispFalloff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoVoidAllies", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.EliteReworks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Charzard4261.CaptainAbilitiesOffworld", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.EnigmaBlacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.GestureEnigma", BepInDependency.DependencyFlags.SoftDependency)]
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
    [BepInDependency("zombieseatflesh7.ArtifactOfPotential", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RuneFoxMods.TeleporterTurrets", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.RiskyLives.RiskyMod", "RiskyMod", "1.1.3")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(RecalculateStatsAPI), nameof(PrefabAPI), nameof(DamageAPI), nameof(SoundAPI), nameof(ItemAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class RiskyMod : BaseUnityPlugin
    {
        public static bool inBazaar = false;

        public static bool disableProcChains = true;

        public static ItemDef emptyItemDef = null;
        public static BuffDef emptyBuffDef = null;

        public static AssistManager assistManager = null;

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
            DefenseMatrixManager.Initialize();
        }

        public void Awake()
        {
            //Check this first since config relies on it
            InitDependencies();

            On.RoR2.Stage.Start += (orig, self) =>
            {
                orig(self);
                inBazaar = false;
                if (RoR2.SceneCatalog.GetSceneDefForCurrentScene() == bazaarScene)
                {
                    inBazaar = true;
                }
            };

            On.RoR2.GameModeCatalog.LoadGameModes += (orig) =>
            {
                orig();
                simulacrumIndex = GameModeCatalog.FindGameModeIndex("InfiniteTowerRun");
                classicRunIndex = GameModeCatalog.FindGameModeIndex("ClassicRun");
            };

            pluginInfo = Info;
            Assets.Init();
            ConfigFiles.Init();
            CheckDependencies();

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
            new FireSelect();
            new EnemiesCore();
            SetupAssists();
            AddHooks();
        }

        //This isn't all the softdependency checks. Some still are in CheckDependencies, this is just a quick fix.
        private void InitDependencies()
        {
            SoftDependencies.RiskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            SoftDependencies.ArtifactOfPotentialLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("zombieseatflesh7.ArtifactOfPotential");
            SoftDependencies.TeleporterTurretsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RuneFoxMods.TeleporterTurrets");
            SoftDependencies.KingKombatArenaLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Kingpinush.KingKombatArena");
            SoftDependencies.ZetTweaksLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetTweaks");
            SoftDependencies.AIBlacklistLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AI_Blacklist");
            SoftDependencies.QueensGlandBuffLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.kking117.QueenGlandBuff");
            FixPlayercount.MultitudesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.wildbook.multitudes");
            FixPlayercount.ZetArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetArtifacts");
            SoftDependencies.ScepterPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            SoftDependencies.ClassicItemsScepterLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");
            SoftDependencies.ShareSuiteLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.funkfrog_sipondo.sharesuite");
            SoftDependencies.RtAutoSprintLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.johnedwa.RTAutoSprintEx");
            SoftDependencies.SpikestripGrooveSalad = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.groovesalad.GrooveSaladSpikestripContent");
            SoftDependencies.SpikestripHeyImNoob = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.heyimnoob.NoopSpikestripContent");
            SoftDependencies.SpikestripPlasmaCore = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.plasmacore.PlasmaCoreSpikestripContent");
        }

        private void CheckDependencies()
        {
            if (SoftDependencies.KingKombatArenaLoaded)
            {
                On.RoR2.Stage.Start += CheckArenaLoaded;
            }

            if (SoftDependencies.ZetTweaksLoaded) ZetTweaksCompat();

            bool noLevelUpHealPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoLevelupHeal");
            if (noLevelUpHealPluginLoaded && NoLevelupHeal.enabled) Debug.Log("RiskyMod: Disabling NoLevelupHeal because standalone plugin is loaded.");
            NoLevelupHeal.enabled = NoLevelupHeal.enabled && !noLevelUpHealPluginLoaded;

            bool removeLevelCapPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RaiseMonsterLevelCap");
            if (removeLevelCapPluginLoaded && RemoveLevelCap.enabled) Debug.Log("RiskyMod: Disabling RemoveLevelCap because standalone plugin is loaded.");
            RemoveLevelCap.enabled = RemoveLevelCap.enabled && !removeLevelCapPluginLoaded;

            bool fpcPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.FixPlayercount");
            if (fpcPluginLoaded && FixPlayercount.enabled) Debug.Log("RiskyMod: Disabling FixPlayercount because standalone plugin is loaded.");
            FixPlayercount.enabled = FixPlayercount.enabled && !fpcPluginLoaded;

            if (SoftDependencies.QueensGlandBuffLoaded && QueensGland.enabled) Debug.Log("RiskyMod: Disabling Queens Gland changes because QueensGlandBuff is loaded.");

            bool nvaPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoVoidAllies");
            if (nvaPluginLoaded && VoidInfestor.noVoidAllies) Debug.Log("RiskyMod: Disabling NoVoidAllies because standalone plugin is loaded.");
            VoidInfestor.noVoidAllies = VoidInfestor.noVoidAllies && !nvaPluginLoaded;

            if (SoftDependencies.AIBlacklistLoaded)
            {
                HandleAIBlacklist();
                if (FixVengeanceLeveling.enabled) Debug.Log("RiskyMod: Disabling FixVengeanceLeveling because AI_Blacklist plugin is loaded.");
            }
            FixVengeanceLeveling.enabled = FixVengeanceLeveling.enabled && !SoftDependencies.AIBlacklistLoaded;

            bool pahPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.ArtifactReliquaryHealingFix");
            if (pahPluginLoaded && PreventArtifactHeal.enabled) Debug.Log("RiskyMod: Disabling PreventArtifactHeal because standalone plugin is loaded.");
            PreventArtifactHeal.enabled = PreventArtifactHeal.enabled && !pahPluginLoaded;

            bool captainHiddenRealmsPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.PlasmaCore.AlwaysAllowSupplyDrops") || BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Charzard4261.CaptainAbilitiesOffworld");
            if (captainHiddenRealmsPluginLoaded && CaptainOrbitalHiddenRealms.enabled) Debug.Log("RiskyMod: Disabling CaptainOrbitalHiddenRealms because standalone plugin is loaded.");
            CaptainOrbitalHiddenRealms.enabled = CaptainOrbitalHiddenRealms.enabled
                && !captainHiddenRealmsPluginLoaded;

            bool hypercritPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.Hypercrit");
            if (hypercritPluginLoaded && CritHud.enabled) Debug.Log("RiskyMod: Disabling Ocular HUD changes because Hypercrit is loaded.");
            CritHud.enabled = CritHud.enabled && !hypercritPluginLoaded;   //Effect is already a part of hypercrit

            bool moffHereticPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.Heretic");
            if (moffHereticPluginLoaded && Visions.enabled) Debug.Log("RiskyMod: Disabling Visions of Heresy changes because Moffein's Heretic is loaded.");
            Visions.enabled = Visions.enabled && !moffHereticPluginLoaded;

            //Croco
            bool ahbPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TheTimeSweeper.AcridHitboxBuff");
            if (ahbPluginLoaded && BiggerMeleeHitbox.enabled) Debug.Log("RiskyMod: Disabling Acrid BiggerMeleeHitbox because standalone plugin is loaded.");
            BiggerMeleeHitbox.enabled = BiggerMeleeHitbox.enabled && !ahbPluginLoaded;

            bool blightStackPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AcridBlightStack");
            if (blightStackPluginLoaded && BlightStack.enabled) Debug.Log("RiskyMod: Disabling Acrid BlightStack because standalone plugin is loaded.");
            BlightStack.enabled = BlightStack.enabled && !blightStackPluginLoaded;

            //Bandit2
            bool backstabReworkPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.BackstabRework");
            if (backstabReworkPluginLoaded && BackstabRework.enabled) Debug.Log("RiskyMod: Disabling Bandit2 BackstabRework because standalone plugin is loaded.");
            BackstabRework.enabled = BackstabRework.enabled && !backstabReworkPluginLoaded;

            //Enemies
            bool lunarWispFalloffPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.LunarWispFalloff");
            if (lunarWispFalloffPluginLoaded && LunarWisp.enableFalloff) Debug.Log("RiskyMod: Disabling LunarWispFalloff because standalone plugin is loaded.");
            LunarWisp.enableFalloff = LunarWisp.enableFalloff && !lunarWispFalloffPluginLoaded;

            bool enigmaBlacklistPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EnigmaBlacklist");
            if (enigmaBlacklistPluginLoaded && FixEnigmaBlacklist.enabled) Debug.Log("RiskyMod: Disabling EnigmaBlacklist because standalone plugin is loaded.");
            FixEnigmaBlacklist.enabled = FixEnigmaBlacklist.enabled && !enigmaBlacklistPluginLoaded;

            //Gesture is always disabled by default so this shouldn't matter.
            bool gestureEnigmaPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.GestureEnigma");
            if (gestureEnigmaPluginLoaded && Gesture.enabled) Debug.Log("RiskyMod: Disabling Gesture of the Drowned changes because GestureEnigma is loaded.");
            Gesture.enabled = Gesture.enabled && !gestureEnigmaPluginLoaded;

            bool eliteReworksPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EliteReworks");
            if (eliteReworksPluginLoaded && NerfVoidtouched.enabled) Debug.Log("RiskyMod: Disabling NerfVoidtouched because EliteReworks is loaded.");
            NerfVoidtouched.enabled = NerfVoidtouched.enabled && !eliteReworksPluginLoaded;

            if (eliteReworksPluginLoaded && NullifyDebuff.enabled) Debug.Log("RiskyMod: Disabling Nullify Changes because EliteReworks is loaded.");
            NullifyDebuff.enabled = NullifyDebuff.enabled && !eliteReworksPluginLoaded;

            if (SoftDependencies.ShareSuiteLoaded) HandleShareSuite();

            bool interactableLimitPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.InteractableLimit");
            if (interactableLimitPluginLoaded && SpawnLimits.enabled) Debug.Log("RiskyMod: Disabling Interactable SpawnLimits changes because InteractableLimit is loaded.");
            SpawnLimits.enabled = SpawnLimits.enabled && !interactableLimitPluginLoaded;

            //No need for this to be static since this doesn't get referenced anywhere else
            bool teleExpansionLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.TeleExpansion");

            if (teleExpansionLoaded && SmallHoldoutCharging.enabled) Debug.Log("RiskyMod: Disabling SmallHoldoutCharging because TeleExpansion is loaded.");
            SmallHoldoutCharging.enabled = SmallHoldoutCharging.enabled && !teleExpansionLoaded;

            if (teleExpansionLoaded && TeleExpandOnBossKill.enabled) Debug.Log("RiskyMod: Disabling TeleExpandOnBossKill because TeleExpansion is loaded.");
            TeleExpandOnBossKill.enabled = TeleExpandOnBossKill.enabled && !teleExpansionLoaded;

            if (teleExpansionLoaded && VoidLocus.ModifyHoldout.enabled) Debug.Log("RiskyMod: Disabling VoidLocus.ModifyHoldout because TeleExpansion is loaded.");
            VoidLocus.ModifyHoldout.enabled = VoidLocus.ModifyHoldout.enabled && !teleExpansionLoaded;

            if (teleExpansionLoaded && Moon.ModifyHoldout.enabled) Debug.Log("RiskyMod: Disabling Moon.ModifyHoldout because TeleExpansion is loaded.");
            Moon.ModifyHoldout.enabled = Moon.ModifyHoldout.enabled && !teleExpansionLoaded;

            Sacrifice.enabled = Sacrifice.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.SacrificeTweaks");
        }

        private void CheckArenaLoaded(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
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
                if (Allies.DroneChanges.MegaDrone.allowRepair) Debug.LogError("RiskyMod: MegaDrone.allowRepair is enabled while ZetTweaks is loaded. Disable this feature in the config of ZetTweaks or RiskyMod, or else you will see duplicated MegaDrone corpses.");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private bool CheckClassicItemsScepter()
        {
            return ThinkInvisible.ClassicItems.Scepter.instance.enabled;
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
            new RemoveLevelCap();
            new NoLevelupHeal();
            new SceneDirectorMonsterRewards();
            new MonsterGoldRewards();
            new CombatDirectorMultiplier();
            new LoopBossArmor();
            new LoopTeleMountainShrine();
            new NoBossRepeat();

            //Holdouts
            new SmallHoldoutCharging();
            new TeleExpandOnBossKill();
            new TeleChargeDuration();

            //Interactables
            new BloodShrineMinReward();
            new ShrineCombatItems();
            new SpawnLimits();
            new ScaleCostWithPlayerCount();

            //Character Mechanics
            new TrueOSP();
            new ShieldGating();
            new CloakBuff();
            new Shock();
            new FixSlayer();
            new BarrierDecay();
            new FreezeChampionExecute();
            new NerfVoidtouched();
            new PlayerControlledMonsters();
            new NullifyDebuff();

            //Artifacts
            new FixVengeanceLeveling();
            new VengeancePercentHeal();
            new VengeanceVoidTeam();
            new EnigmaBlacklist();
            new BulwarksAmbry();
            new Sacrifice();

            //Misc
            new AIBlacklistItems();
            new BetterProjectileTracking();
            new ItemOutOfBounds();
        }

        private void RunFixes()
        {
            new FixPlayercount();
            new FixFocusCrystalSelfDamage();
            new PreventArtifactHeal();
            new TreebotFruitingNullref();
            new FixLightningStrikeOrbProcCoefficient();
            new FixCrocoPoisonAchievement();
            new FixWormFallDeath();
            new GhostDunestriderFriendlyFire();
            new GhostGrandparentFriendlyFire();
        }
        
        private void AddHooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemy.GlobalEventManager_OnHitEnemy;
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients.RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += RecalculateStats.CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += TakeDamage.HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnCharacterDeath += OnCharacterDeath.GlobalEventManager_OnCharacterDeath;
            On.RoR2.GlobalEventManager.OnHitAll += OnHitAll.GlobalEventManager_OnHitAll;
            On.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime.UpdateLastHitTime;
            SharedHooks.TakeDamage.OnDamageTakenAttackerActions += TakeDamage.DistractOnHit;

            new ModifyFinalDamage();
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
        }

        public delegate void FireMode();
        public static FireMode FireModeActions;

        private void Update()
        {
            if (FireModeActions != null) FireModeActions.Invoke();
        }
    }
}
