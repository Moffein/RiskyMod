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
using RiskyMod.Survivors.Commando;
using RiskyMod.Survivors.Huntress;
using RiskyMod.Survivors.Engi;
using RiskyMod.Survivors.Toolbot;
using RiskyMod.Survivors.Treebot;
using RiskyMod.Survivors.Croco;
using RiskyMod.Enemies.Mobs.Lunar;
using RiskyMod.Survivors.Loader;
using RiskyMod.Survivors.Mage;
using RiskyMod.Enemies.Mobs;
using RiskyMod.Survivors.Merc;
using System.Runtime.CompilerServices;
using RiskyMod.Content;
using System.Reflection;
using RiskyMod.Items.DLC1.Legendary;
using RiskyMod.Items.DLC1.Void;
using RiskyMod.Enemies.DLC1;
using RiskyMod.Survivors.DLC1.VoidFiend;
using System.IO;
using RiskyMod.VoidLocus;
using RiskyMod.Survivors.DLC1.Railgunner;
using RiskyMod.Items.DLC1.Uncommon;
using RiskyMod.Tweaks.RunScaling;
using RiskyMod.Tweaks.Holdouts;
using RiskyMod.Tweaks.CharacterMechanics;
using RiskyMod.Tweaks.Artifacts;

namespace RiskyMod
{
    [BepInDependency("com.PlasmaCore.StickyStunter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.MobileTurretBuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.FixPlayercount", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoLevelupHeal", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AI_Blacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dev.wildbook.multitudes", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TPDespair.ZetArtifacts", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rob.ArtifactReliquaryHealingFix", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.RaiseMonsterLevelCap", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.PlasmaCore.AlwaysAllowSupplyDrops", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.Hypercrit", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TheTimeSweeper.AcridHitboxBuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AcridBlightStack", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.LunarWispFalloff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BeetleQueenPlus", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoVoidAllies", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.EliteReworks", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("Charzard4261.CaptainAbilitiesOffworld", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.EnigmaBlacklist", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.GestureEnigma", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BackstabRework", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.funkfrog_sipondo.sharesuite", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.TeleExpansion", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.RiskyLives.RiskyMod", "RiskyMod Beta", "0.7.0")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(RecalculateStatsAPI), nameof(PrefabAPI), nameof(DamageAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class RiskyMod : BaseUnityPlugin
    {
        private const string generalString = "General";
        private const string scalingString = "General - Run Scaling";
        private const string holdoutString = "General - Holdouts";
        private const string shrineString = "General - Shrines";
        private const string charMechanicsString = "General - Character Mechanics";
        private const string artifactString = "General - Artifacts";
        private const string coreModuleString = "0 - Core Modules";

        private const string moonString = "Moon";

        private const string allyString = "Allies";

        private const string uncommonString = "Items - Uncommon";
        private const string commonString = "Items - Common";
        private const string legendaryString = "Items - Legendary";
        private const string voidString = "Items - Void";
        private const string bossString = "Items - Boss";
        private const string lunarString = "Items - Lunar";
        private const string equipmentString = "Items - Equipment";
        private const string itemConfigDescString = "Enable changes to this item.";

        private const string monsterString = "Monsters";

        private const string commandoString = "Survivors: Commando";
        private const string huntressString = "Survivors: Huntress";
        private const string toolbotString = "Survivors: MUL-T";
        private const string engiString = "Survivors: Engineer";
        private const string treebotString = "Survivors: REX";
        private const string crocoString = "Survivors: Acrid";
        private const string banditString = "Survivors: Bandit";
        private const string captainString = "Survivors: Captain";
        private const string mageString = "Survivors: Artificer";
        private const string loaderString = "Survivors: Loader";
        private const string mercString = "Survivors: Mercenary";
        private const string railgunnerString = "Survivors: Railgunner";
        private const string voidFiendString = "Survivors: Void Fiend";

        private const string fireSelectString = "Survivors - Firemode Selection (Client-Side)";

        public static bool disableProcChains = true;

        public static bool ClassicItemsScepterLoaded = false;
        public static bool ScepterPluginLoaded = false;
        public static bool AIBlacklistLoaded = false;
        public static bool AIBlacklistUseVanillaBlacklist = true;

        public static bool ShareSuiteLoaded = false;
        public static bool ShareSuiteCommon = false;
        public static bool ShareSuiteUncommon = false;
        public static bool ShareSuiteLegendary = false;
        public static bool ShareSuiteBoss = false;
        public static bool ShareSuiteLunar = false;

        public static bool EnableProjectileCloning = true;

        public static ItemDef emptyItemDef = null;
        public static BuffDef emptyBuffDef = null;

        public static AssistManager assistManager = null;

        public static PluginInfo pluginInfo;
        public static FileSystem fileSystem { get; private set; }

        public void Awake()
        {
            pluginInfo = Info;
            Assets.Init();
            ReadConfig();
            CheckDependencies();

            ContentCore.Init();
            new SharedDamageTypes();
            RunFixes();
            RunTweaks();
            new ItemsCore();
            new AlliesCore();
            new MoonCore();
            new VoidLocusCore();
            new SurvivorsCore();
            new FireSelect();
            new EnemiesCore();
            SetupAssists();
            AddHooks();
        }

        private void CheckDependencies()
        {
            NoLevelupHeal.enabled = NoLevelupHeal.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoLevelupHeal");
            RemoveLevelCap.enabled = RemoveLevelCap.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RaiseMonsterLevelCap");

            FixPlayercount.enabled = FixPlayercount.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.FixPlayercount");
            FixPlayercount.MultitudesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.wildbook.multitudes");
            FixPlayercount.ZetArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetArtifacts");

            VoidInfestor.noVoidAllies = VoidInfestor.noVoidAllies && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoVoidAllies");

            AIBlacklistLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AI_Blacklist");
            if (AIBlacklistLoaded) HandleAIBlacklist();
            FixVengeanceLeveling.enabled = FixVengeanceLeveling.enabled && !AIBlacklistLoaded;

            PreventArtifactHeal.enabled = PreventArtifactHeal.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.ArtifactReliquaryHealingFix");
            CaptainOrbitalHiddenRealms.enabled = CaptainOrbitalHiddenRealms.enabled
                && !(BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.PlasmaCore.AlwaysAllowSupplyDrops")|| BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Charzard4261.CaptainAbilitiesOffworld"));
            CritHud.enabled = CritHud.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.Hypercrit");   //Effect is already a part of hypercrit

            ScepterPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            ClassicItemsScepterLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");

            //Croco
            BiggerMeleeHitbox.enabled = BiggerMeleeHitbox.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TheTimeSweeper.AcridHitboxBuff");
            BlightStack.enabled = BlightStack.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AcridBlightStack");

            //Bandit2
            BackstabRework.enabled = BackstabRework.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.BackstabRework");

            //Enemies
            BeetleQueen.enabled = BeetleQueen.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.BeetleQueenPlus");
            LunarWisp.enableFalloff = LunarWisp.enableFalloff && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.LunarWispFalloff");

            FixEnigmaBlacklist.enabled = FixEnigmaBlacklist.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EnigmaBlacklist");
            Gesture.enabled = Gesture.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.GestureEnigma");
            NerfVoidtouched.enabled = NerfVoidtouched.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EliteReworks");

            ShareSuiteLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.funkfrog_sipondo.sharesuite");
            if (ShareSuiteLoaded) HandleShareSuite();

            //No need for this to be static since this doesn't get referenced anywhere else
            bool teleExpansionLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.TeleExpansion");
            SmallHoldoutCharging.enabled = SmallHoldoutCharging.enabled && !teleExpansionLoaded;
            SmallHoldoutRadius.enabled = SmallHoldoutRadius.enabled && !teleExpansionLoaded;
            TeleExpandOnBossKill.enabled = TeleExpandOnBossKill.enabled && !teleExpansionLoaded;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private bool CheckClassicItemsScepter()
        {
            return ThinkInvisible.ClassicItems.Scepter.instance.enabled;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void HandleAIBlacklist()
        {
            AIBlacklistUseVanillaBlacklist = AI_Blacklist.AIBlacklist.useVanillaAIBlacklist;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void HandleShareSuite()
        {
            ShareSuiteCommon = ShareSuite.ShareSuite.WhiteItemsShared.Value;
            ShareSuiteUncommon = ShareSuite.ShareSuite.GreenItemsShared.Value;
            ShareSuiteLegendary = ShareSuite.ShareSuite.RedItemsShared.Value;
            ShareSuiteBoss = ShareSuite.ShareSuite.BossItemsShared.Value;
            ShareSuiteLunar = ShareSuite.ShareSuite.LunarItemsShared.Value;
        }

        private void RunTweaks()
        {
            //RunScaling
            new Scaling();
            new LoopBossArmor();
            new RemoveLevelCap();
            new NoLevelupHeal();
            new SceneDirectorMonsterRewards();

            //Holdouts
            new SmallHoldoutCharging();
            new SmallHoldoutRadius();
            new TeleExpandOnBossKill();

            //Shrines
            new BloodShrineMinReward();
            new ShrineCombatItems();
            new ShrineSpawnRate();

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
            new SlowDownProjectilesModifyDamage();

            //Artifacts
            new VengeancePercentHeal();
            new EnigmaBlacklist();

            //Misc
            new AIBlacklistItems();
        }

        private void RunFixes()
        {
            new FixPlayercount();
            new FixVengeanceLeveling();
            new FixFocusCrystalSelfDamage();
            new PreventArtifactHeal();
            new TreebotFruitingNullref();
            new FixLightningStrikeOrbProcCoefficient();
            new FixCrocoPoisonAchievement();
            new FixWormFallDeath();
            new GhostDunestriderFriendlyFire();
            new FixExtraLifeVoidBlacklist();
        }
        
        private void AddHooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemy.GlobalEventManager_OnHitEnemy;
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients.RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.RecalculateStats += RecalculateStats.CharacterBody_RecalculateStats;
            On.RoR2.HealthComponent.TakeDamage += TakeDamage.HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnCharacterDeath += OnCharacterDeath.GlobalEventManager_OnCharacterDeath;
            On.RoR2.GlobalEventManager.OnHitAll += OnHitAll.GlobalEventManager_OnHitAll;

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

        public delegate void FireMode();
        public static FireMode FireModeActions = FireModeMethod;
        private static void FireModeMethod() { }
        private void Update()
        {
            FireModeActions.Invoke();
        }



        private void ReadConfig()
        {
            disableProcChains = Config.Bind(generalString, "Disable Proc Chains", true, "Remove the proc coefficient on most item effects.").Value;
            ShieldGating.enabled = Config.Bind(generalString, "Shield Gating", true, "Shields gate against HP damage.").Value;
            TrueOSP.enabled = Config.Bind(generalString, "True OSP", true, "Makes OSP work against multihits.").Value;
            AIBlacklistItems.enabled = Config.Bind(generalString, "Expanded AI Blacklist", true, "Adds extra items to the AI Blacklist by default.").Value;

            Scaling.enabled = Config.Bind(scalingString, "Linear Difficulty Scaling", true, "Makes difficulty scaling linear.").Value;
            NoLevelupHeal.enabled = Config.Bind(scalingString, "No Levelup Heal", true, "Monsters don't gain HP when leveling up.").Value;
            RemoveLevelCap.enabled = Config.Bind(scalingString, "Increase Monster Level Cap", true, "Increases Monster Level Cap.").Value;
            RemoveLevelCap.maxLevel = Config.Bind(scalingString, "Increase Monster Level Cap - Max Level", 9999f, "Maximum monster level if Increase Monster Level Cap is enabled.").Value;
            LoopBossArmor.enabled = Config.Bind(scalingString, "Loop Boss Armor", true, "Teleporter bosses gain bonus armor when looping.").Value;
            SceneDirectorMonsterRewards.enabled = Config.Bind(scalingString, "SceneDirector Monster Rewards", true, "Monsters that spawn with the map now give the same rewards as teleporter monsters.").Value;

            AlliesCore.enabled = Config.Bind(coreModuleString, "Ally Changes", true, "Enable drone and ally changes.").Value;
            ItemsCore.enabled = Config.Bind(coreModuleString, "Item Changes", true, "Enable item changes.").Value;
            SurvivorsCore.enabled = Config.Bind(coreModuleString, "Survivor Changes", true, "Enable survivor changes.").Value;
            EnemiesCore.modifyEnemies = Config.Bind(coreModuleString, "Monster Changes", true, "Enable enemy changes.").Value;
            MoonCore.enabled = Config.Bind(coreModuleString, "Moon Changes", true, "Enable Moon changes.").Value;
            VoidLocusCore.enabled = Config.Bind(coreModuleString, "Void Locus Changes", true, "Enable Void Locus changes.").Value;

            AllyScaling.normalizeDroneDamage = Config.Bind(allyString, "Normalize Drone Damage", true, "Normalize drone damage stats so that they perform the same when using Spare Drone Parts.").Value;
            AlliesCore.nerfDroneParts = Config.Bind(allyString, "Spare Drone Parts Changes", true, "Reduce the damage of Spare Drone Parts.").Value;
            AllyScaling.noVoidDeath = Config.Bind(allyString, "No Void Death", true, "Allies are immune to Void implosions.").Value;
            NoVoidDamage.enabled = Config.Bind(allyString, "No Void Damage", true, "Allies take no damage from Void fog.").Value;
            AllyScaling.noOverheat = Config.Bind(allyString, "No Overheat", true, "Allies are immune to Grandparent Overheat.").Value;
            SuperAttackResist.enabled = Config.Bind(allyString, "Superattack Resistance", true, "Allies take less damage from superattacks like Vagrant Novas.").Value;

            TeleExpandOnBossKill.enabled = Config.Bind(holdoutString, "Tele Expand on Boss Kill", true, "Teleporter expands to cover the whole map when the boss is killed.").Value;
            SmallHoldoutCharging.enabled = Config.Bind(holdoutString, "Small Holdout Charging", true, "Void/Moon Holdouts charge at max speed as long as 1 player is charging.").Value;
            SmallHoldoutRadius.enabled = Config.Bind(holdoutString, "Small Holdout Radius", true, "Void/Moon Holdouts have increased radius.").Value;

            FixSlayer.enabled = Config.Bind(charMechanicsString, "Fix Slayer Procs", true, "Bandit/Acrid bonus damage to low hp effect now applies to procs.").Value;
            CloakBuff.enabled = Config.Bind(charMechanicsString, "Cloak Buff", true, "Increases delay between position updates while cloaked.").Value;
            Shock.enabled = Config.Bind(charMechanicsString, "No Shock Interrupt", true, "Shock is no longer interrupted by damage.").Value;
            BarrierDecay.enabled = Config.Bind(charMechanicsString, "Barrier Decay", true, "Barrier decays slower at low barrier values.").Value;
            FreezeChampionExecute.enabled = Config.Bind(charMechanicsString, "Freeze Executes Bosses", true, "Freeze counts as a debuff and can execute bosses at 15% HP.").Value;
            NerfVoidtouched.enabled = Config.Bind(charMechanicsString, "Nerf Voidtouched", true, "Replaces Voidtouched Collapse with Nullify.").Value;
            PlayerControlledMonsters.enabled = Config.Bind(charMechanicsString, "Player-Controlled Monster Regen", true, "Gives players health regen + armor when playing as monsters via mods.").Value;

            Clover.enabled = Config.Bind(charMechanicsString, "Cap Max Luck", false, "Caps how high the Luck stat can go.").Value;
            Clover.luckCap = Config.Bind(charMechanicsString, "Cap Max Luck - Maximum Value", 1f, "Maximum Luck value playeres can reach. Extra Luck is converted to stat boosts.").Value;

            ShrineSpawnRate.enabled = Config.Bind(shrineString, "Mountain/Combat Shrine Playercount Scaling", true, "Mountain/Combat Shrine Director Credit cost scales with playercount.").Value;
            ShrineCombatItems.enabled = Config.Bind(shrineString, "Shrine of Combat Drops Items", true, "Shrine of Combat drops items for the team on completion.").Value;
            BloodShrineMinReward.enabled = Config.Bind(shrineString, "Shrine of Blood Minimum Reward", true, "Shrine of Blood always gives at least enough money to buy a small chest.").Value;

            VengeancePercentHeal.enabled = Config.Bind(artifactString, "Reduce Vengeance Healing", true, "Vengeance Doppelgangers receive reduced healing from percent-based healing effects.").Value;
            EnigmaBlacklist.enabled = Config.Bind(artifactString, "Enigma Blacklist", true, "Blacklist Lunars and Recycler from the Artifact of Enigma.").Value;


            ConfigMoon();
            ConfigCommonItems();
            ConfigUncommonItems();
            ConfigLegendaryItems();
            ConfigVoidItems();
            ConfigBossItems();
            ConfigLunars();
            ConfigEquipment();

            ConfigMonsters();

            ConfigFireSelect();
            ConfigSurvivors();
        }

        private void ConfigMoon()
        {
            LessPillars.enabled = Config.Bind(moonString, "Reduce Pillar Count", true, "Reduce the amount of pillars required to activate the jump pads.").Value;
            PillarsDropItems.enabled = Config.Bind(moonString, "Pillars Drop Items", true, "Pillars drop items for the team when completed.").Value;
            PillarsDropItems.whiteChance = Config.Bind(moonString, "Pillars Drop Items - Common Chance", 50f, "Chance for Pillars to drop Common Items.").Value;
            PillarsDropItems.greenChance = Config.Bind(moonString, "Pillars Drop Items - Uncommon Chance", 40f, "Chance for Pillars to drop Common Items.").Value;
            PillarsDropItems.redChance = Config.Bind(moonString, "Pillars Drop Items - Legendary Chance", 10f, "Chance for Pillars to drop Common Items.").Value;
            PillarsDropItems.pearlOverwriteChance = Config.Bind(moonString, "Pillars Drop Items - Pearl Overwrite Chance", 15f, "Chance for nonlegendary Pillar drops to be overwritten with a Pearl.").Value;
            PillarsDropItems.lunarOverwriteChance = Config.Bind(moonString, "Pillars Drop Items - Lunar Overwrite Chance", 0f, "Chance for nonlegendary Pillar drops to be overwritten with a random Lunar item.").Value;
            PillarsDropItems.noOverwriteChance = Config.Bind(moonString, "Pillars Drop Items - No Overwrite Chance", 85f, "Chance for nonlegendary Pillar drops to not be overwritten.").Value;
        }

        private void ConfigCommonItems()
        {
            BisonSteak.enabled = Config.Bind(commonString, "Bison Steak", true, itemConfigDescString).Value;
            Bungus.enabled = Config.Bind(commonString, "Bustling Fungus", true, itemConfigDescString).Value;
            CautiousSlug.enabled = Config.Bind(commonString, "Cautious Slug", true, itemConfigDescString).Value;
            Crowbar.enabled = Config.Bind(commonString, "Crowbar", true, itemConfigDescString).Value;
            Fireworks.enabled = Config.Bind(commonString, "Fireworks", true, itemConfigDescString).Value;
            Fireworks.maxRockets = Config.Bind(commonString, "Fireworks - Max Rockets", 32, "Max rockets to spawn.").Value;
            Gasoline.enabled = Config.Bind(commonString, "Gasoline", true, itemConfigDescString).Value;
            CritGlasses.enabled = Config.Bind(commonString, "Lensmakers Glasses", true, itemConfigDescString).Value;
            MonsterTooth.enabled = Config.Bind(commonString, "Monster Tooth", true, itemConfigDescString).Value;
            StickyBomb.enabled = Config.Bind(commonString, "Stickybomb", true, itemConfigDescString).Value;
            TougherTimes.enabled = Config.Bind(commonString, "Tougher Times", true, itemConfigDescString).Value;
            Warbanner.enabled = Config.Bind(commonString, "Warbanner", true, itemConfigDescString).Value;
        }

        private void ConfigUncommonItems()
        {
            AtG.enabled = Config.Bind(uncommonString, "AtG Missile", true, itemConfigDescString).Value;
            AtG.useOrb = Config.Bind(uncommonString, "AtG Missile - Use OrbAttack", true, "Makes AtG missiles behave like Plasma Shrimp when fired off of weak attacks to improve performance.").Value;
            AtG.alwaysOrb = Config.Bind(uncommonString, "AtG Missile - Always Use OrbAttack", false, "AtG Missiles always fire orbattacks regardless of the hit's strength.").Value;
            Bandolier.enabled = Config.Bind(uncommonString, "Bandolier", true, itemConfigDescString).Value;
            Berzerker.enabled = Config.Bind(uncommonString, "Berzerkers Pauldron", true, itemConfigDescString).Value;
            Chronobauble.enabled = Config.Bind(uncommonString, "Chronobauble", true, itemConfigDescString).Value;
            ElementalBands.enabled = Config.Bind(uncommonString, "Runalds and Kjaros Bands", true, itemConfigDescString).Value;
            Harpoon.enabled = Config.Bind(uncommonString, "Hunters Harpoon", true, itemConfigDescString).Value;
            HarvesterScythe.enabled = Config.Bind(uncommonString, "Harvesters Scythe", true, itemConfigDescString).Value;
            Infusion.enabled = Config.Bind(uncommonString, "Infusion", true, itemConfigDescString).Value;
            LeechingSeed.enabled = Config.Bind(uncommonString, "Leeching Seed", true, itemConfigDescString).Value;
            Daisy.enabled = Config.Bind(uncommonString, "Lepton Daisy", true, itemConfigDescString).Value;
            Guillotine.enabled = Config.Bind(uncommonString, "Old Guillotine", true, itemConfigDescString).Value;
            Guillotine.reduceVFX = Config.Bind(uncommonString, "Old Guillotine - Reduce VFX", true, "Reduce how often this item's VFX shows up.").Value;
            Razorwire.enabled = Config.Bind(uncommonString, "Razorwire", true, itemConfigDescString).Value;
            RedWhip.enabled = Config.Bind(uncommonString, "Red Whip", true, itemConfigDescString).Value;
            RoseBuckler.enabled = Config.Bind(uncommonString, "Rose Buckler", true, itemConfigDescString).Value;
            SquidPolyp.enabled = Config.Bind(uncommonString, "Squid Polyp", true, itemConfigDescString).Value;
            SquidPolyp.scaleCount = Config.Bind(uncommonString, "Squid Polyp - Stacks Increase Max Squids", false, "Extra stacks allow for more squids to spawn. Will lag in MP.").Value;
            Stealthkit.enabled = Config.Bind(uncommonString, "Old War Stealthkit", true, itemConfigDescString).Value;
            Ukulele.enabled = Config.Bind(uncommonString, "Ukulele", true, itemConfigDescString).Value;
            WarHorn.enabled = Config.Bind(uncommonString, "War Horn", true, itemConfigDescString).Value;
            WillOWisp.enabled = Config.Bind(uncommonString, "Will-o-the-Wisp", true, itemConfigDescString).Value;
        }

        private void ConfigLegendaryItems()
        {
            Behemoth.enabled = Config.Bind(legendaryString, "Brilliant Behemoth", true, itemConfigDescString).Value;
            BottledChaos.enabled = Config.Bind(legendaryString, "Bottled Chaos", true, itemConfigDescString).Value;
            Brainstalks.enabled = Config.Bind(legendaryString, "Brainstalks", true, itemConfigDescString).Value;
            CeremonialDagger.enabled = Config.Bind(legendaryString, "Ceremonial Dagger", true, itemConfigDescString).Value;
            FrostRelic.enabled = Config.Bind(legendaryString, "Frost Relic", true, itemConfigDescString).Value;
            FrostRelic.removeFOV = Config.Bind(legendaryString, "Frost Relic - Disable FOV Modifier", true, "Disables FOV modifier.").Value;
            FrostRelic.removeBubble = Config.Bind(legendaryString, "Frost Relic - Disable Bubble", true, "Disables bubble visuals.").Value;
            HappiestMask.enabled = Config.Bind(legendaryString, "Happiest Mask", true, itemConfigDescString).Value;
            HappiestMask.scaleCount = Config.Bind(legendaryString, "Happiest Mask - Stacks Increase Max Ghosts", false, "Extra stacks allow for more ghosts to spawn. Will lag in MP.").Value;
            HappiestMask.noGhostLimit = Config.Bind(legendaryString, "Happiest Mask - Remove Ghost Limit", false, "Removes the ghost limit at all times. Definitely will lag.").Value;
            HeadHunter.enabled = Config.Bind(legendaryString, "Wake of Vultures", true, itemConfigDescString).Value;
            HeadHunter.perfectedTweak = Config.Bind(legendaryString, "Wake of Vultures - Perfected Tweak", true, "Perfected Affix gained via Wake of Vultures will not force your health pool to bec").Value;
            Headstompers.enabled = Config.Bind(legendaryString, "H3AD-ST", true, itemConfigDescString).Value;
            LaserTurbine.enabled = Config.Bind(legendaryString, "Resonance Disc", true, itemConfigDescString).Value;
            MeatHook.enabled = Config.Bind(legendaryString, "Sentient Meat Hook", true, itemConfigDescString).Value;
            Raincoat.enabled = Config.Bind(legendaryString, "Bens Raincoat", true, itemConfigDescString).Value;
            Tesla.enabled = Config.Bind(legendaryString, "Unstable Tesla Coil", true, itemConfigDescString).Value;

            //Turns out SS2's Gadget increased initial hit damage by 50%, which lead to a 3x total damage multiplier, which is what this item does already.
            //LaserScope.enabled = Config.Bind(legendaryString, "Laser Scope", true, itemConfigDescString).Value;
            LaserScope.enabled = false;

        }

        private void ConfigVoidItems()
        {
            Dungus.enabled = Config.Bind(voidString, "Weeping Fungus", true, itemConfigDescString).Value;

            //Get feedback first.
            //SaferSpaces.enabled = Config.Bind(voidString, "Safer Spaces", true, itemConfigDescString).Value;
            SaferSpaces.enabled = false;

            PlasmaShrimp.enabled = Config.Bind(voidString, "Plasma Shrimp", true, itemConfigDescString).Value;
            VoidWisp.enabled = Config.Bind(voidString, "Voidsent Flame", true, itemConfigDescString).Value;
            Polylute.enabled = Config.Bind(voidString, "Polylute", true, itemConfigDescString).Value;
            VoidRing.enabled = Config.Bind(voidString, "Singularity Band", true, itemConfigDescString).Value;
        }

        private void ConfigBossItems()
        {
            ChargedPerf.enabled = Config.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            Disciple.enabled = Config.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            Knurl.enabled = Config.Bind(bossString, "Titanic Knurl", true, itemConfigDescString).Value;
            MoltenPerf.enabled = Config.Bind(bossString, "Molten Perforator", true, itemConfigDescString).Value;
            QueensGland.enabled = Config.Bind(bossString, "Queens Gland", true, itemConfigDescString).Value;
            Shatterspleen.enabled = Config.Bind(bossString, "Shatterspleen", true, itemConfigDescString).Value;
        }

        private void ConfigLunars()
        {
            //Currently split into a separate mod to see how it plays.
            //Gesture.enabled = Config.Bind(lunarString, "Gesture of the Drowned", true, itemConfigDescString).Value;
            Gesture.enabled = false;

            BrittleCrown.enabled = Config.Bind(lunarString, "Brittle Crown", true, itemConfigDescString).Value;
            Meteorite.enabled = Config.Bind(lunarString, "Glowing Meteorite", true, itemConfigDescString).Value;
            ShapedGlass.enabled = Config.Bind(lunarString, "Shaped Glass", true, itemConfigDescString).Value;
            Transcendence.enabled = Config.Bind(lunarString, "Transcendence", true, itemConfigDescString).Value;
        }

        private void ConfigEquipment()
        {
            Backup.enabled = Config.Bind(equipmentString, "The Back-Up", true, itemConfigDescString).Value;
            Backup.ignoreTeamLimit = !Config.Bind(equipmentString, "The Back-Up: Limit Drones", false, "Back-Up drones count towards the ally cap.").Value;
            BackupTracker.maxCount = Config.Bind(equipmentString, "The Back-Up: Max Drones", 4, "Max active Backup Drones.").Value;
            BFG.enabled = Config.Bind(equipmentString, "Preon Accumulator", true, itemConfigDescString).Value;
            Capacitor.enabled = Config.Bind(equipmentString, "Royal Capacitor", true, itemConfigDescString).Value;
            Chrysalis.enabled = Config.Bind(equipmentString, "Milky Chrysalis", true, itemConfigDescString).Value;
            CritHud.enabled = Config.Bind(equipmentString, "Ocular HUD", true, itemConfigDescString).Value;
            Fruit.enabled = Config.Bind(equipmentString, "Foreign Fruit", true, itemConfigDescString).Value;
            SuperLeech.enabled = Config.Bind(equipmentString, "Super Massive Leech", true, itemConfigDescString).Value;
            VolcanicEgg.enabled = Config.Bind(equipmentString, "Volcanic Egg", true, itemConfigDescString).Value;
        }

        private void ConfigMonsters()
        {
            FixVengeanceLeveling.enabled = Config.Bind(monsterString, "Fix Vengeance Doppelganger Levels", true, "Fix Vengeance Doppelgangers not leveling up.").Value;

            Beetle.enabled = Config.Bind(monsterString, "Beetle", true, "Enable changes to this monster.").Value;
            Jellyfish.enabled = Config.Bind(monsterString, "Jellyfish", true, "Enable changes to this monster.").Value;
            Imp.enabled = Config.Bind(monsterString, "Imp", true, "Enable changes to this monster.").Value;
            HermitCrab.enabled = Config.Bind(monsterString, "Hermit Crab", true, "Enable changes to this monster.").Value;

            Golem.enabled = Config.Bind(monsterString, "Stone Golem", true, "Enable changes to this monster.").Value;
            Mushrum.enabled = Config.Bind(monsterString, "Mini Mushrum", true, "Enable changes to this monster.").Value;

            Bronzong.enabled = Config.Bind(monsterString, "Brass Contraption", true, "Enable changes to this monster.").Value;
            GreaterWisp.enabled = Config.Bind(monsterString, "Greater Wisp", true, "Enable changes to this monster.").Value;

            Parent.enabled = Config.Bind(monsterString, "Parent", true, "Enable changes to this monster.").Value;

            LunarGolem.enabled = Config.Bind(monsterString, "Lunar Golem", true, "Enable changes to this monster.").Value;
            LunarWisp.enabled = Config.Bind(monsterString, "Lunar Wisp", true, "Enable changes to this monster.").Value;

            BeetleQueen.enabled = Config.Bind(monsterString, "Beetle Queen", true, "Enable changes to this monster.").Value;
            Vagrant.enabled = Config.Bind(monsterString, "Wandering Vagrant", true, "Enable changes to this monster.").Value;
            Gravekeeper.enabled = Config.Bind(monsterString, "Grovetender", true, "Enable changes to this monster.").Value;
            SCU.enabled = Config.Bind(monsterString, "Solus Control Unit", true, "Enable changes to this monster.").Value;

            AWU.enabled = Config.Bind(monsterString, "Alloy Worship Unit", true, "Enable changes to this monster.").Value;

            BlindPest.enabled = Config.Bind(monsterString, "Blind Pest", true, "Enable changes to this monster.").Value;

            VoidInfestor.enabled = Config.Bind(monsterString, "Void Infestor", true, "Enable changes to this monster.").Value;
            VoidInfestor.noVoidAllies = Config.Bind(monsterString, "Void Infestor - No Ally Infestation", true, "Void Infestors can't possess allies.").Value;
        }

        private void ConfigFireSelect()
        {
            FireSelect.scrollSelection = Config.Bind(fireSelectString, "Use Scrollwheel", true, "Mouse Scroll Wheel changes firemode").Value;
            FireSelect.swapButton = Config.Bind(fireSelectString, "Next Firemode", KeyCode.None, "Button to swap to the next firemode.").Value;
            FireSelect.prevButton = Config.Bind(fireSelectString, "Previous Firemode", KeyCode.None, "Button to swap to the previous firemode.").Value;
            CaptainFireModes.enabled = Config.Bind(fireSelectString, "Captain: Enable Fire Select", false, "Enable firemode selection for Captain's shotgun (requires primary changes to be enabled).").Value;
            CaptainFireModes.defaultButton = Config.Bind(fireSelectString, "Captain: Swap to Default", KeyCode.None, "Button to swap to the Default firemode.").Value;
            CaptainFireModes.autoButton = Config.Bind(fireSelectString, "Captain: Swap to Auto", KeyCode.None, "Button to swap to the Auto firemode.").Value;
            CaptainFireModes.chargeButton = Config.Bind(fireSelectString, "Captain: Swap to Charged", KeyCode.None, "Button to swap to the Charged firemode.").Value;
        }

        private void ConfigSurvivors()
        {
            CommandoCore.enabled = Config.Bind(commandoString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            CommandoCore.fixPrimaryFireRate = Config.Bind(commandoString, "Double Tap - Fix Scaling", true, "Fixes Double Tap having a low attack speed cap.").Value;
            CommandoCore.phaseRoundChanges = Config.Bind(commandoString, "Phase Round Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.rollChanges = Config.Bind(commandoString, "Tactical Dive Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.suppressiveChanges = Config.Bind(commandoString, "Suppressive Fire Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.grenadeChanges = Config.Bind(commandoString, "Frag Grenade Changes", true, "Enable changes to this skill.").Value;

            HuntressCore.enabled = Config.Bind(huntressString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            HuntressCore.HuntressTargetingMode = Config.Bind(huntressString, "Targeting Mode (Client-Side)", BullseyeSearch.SortMode.Angle, "How Huntress's target prioritization works.").Value;
            HuntressCore.increaseAngle = Config.Bind(huntressString, "Increase Targeting Angle", true, "Increase max targeting angle.").Value;
            HuntressCore.strafeChanges = Config.Bind(huntressString, "Strafe Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.flurryChanges = Config.Bind(huntressString, "Flurry Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.laserGlaiveChanges = Config.Bind(huntressString, "Laser Glaive Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.blinkChanges = Config.Bind(huntressString, "Blink Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.arrowRainChanges = Config.Bind(huntressString, "Arrow Rain Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.ballistaChanges = Config.Bind(huntressString, "Ballista Changes", true, "Enable changes to this skill.").Value;

            ToolbotCore.enabled = Config.Bind(toolbotString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            ToolbotCore.enableNailgunChanges = Config.Bind(toolbotString, "Nailgun Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableRebarChanges = Config.Bind(toolbotString, "Rebar Puncher Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableScrapChanges = Config.Bind(toolbotString, "Scrap Launcher Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableSawChanges = Config.Bind(toolbotString, "Power Saw Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableSecondarySkillChanges = Config.Bind(toolbotString, "Blast Canister Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableRetoolChanges = Config.Bind(toolbotString, "Retool Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enablePowerModeChanges = Config.Bind(toolbotString, "Power Mode Changes", true, "Enable changes to this skill.").Value;

            EngiCore.enabled = Config.Bind(engiString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            PressureMines.enabled = Config.Bind(engiString, "Pressure Mine Changes", true, "Pressure Mines only detonate when fully armed.").Value;
            TurretChanges.turretChanges = Config.Bind(engiString, "Stationary Turret Changes", true, "Enable changes to Stationary Turrets.").Value;
            TurretChanges.mobileTurretChanges = Config.Bind(engiString, "Mobile Turret Changes", true, "Enable changes to Mobile Turrets.").Value;

            MageCore.enabled = Config.Bind(mageString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            MageCore.modifyFireBolt = Config.Bind(mageString, "Fire Bolt Changes", true, "Enable changes to this skill.").Value;
            MageCore.modifyPlasmaBolt = Config.Bind(mageString, "Plasma Bolt Changes", true, "Enable changes to this skill.").Value;
            MageCore.m2RemoveNanobombGravity = Config.Bind(mageString, "Nanobomb - Remove Gravity", true, "Removes projectile drop from Nanobomb so it behaves like it did pre-1.0 update.").Value;
            MageCore.flamethrowerSprintCancel = Config.Bind(mageString, "Flamethrower - Sprint Cancel", true, "Sprinting cancels Flamethrower.").Value;
            MageCore.ionSurgeMovementScaling = Config.Bind(mageString, "Vanilla Ion Surge - Movement Scaling", false, "Ion Surge jump height scales with movement speed.").Value;
            MageCore.ionSurgeShock = Config.Bind(mageString, "Vanilla Ion Surge - Shock", true, "Ion Surge shocks enemies.").Value;
            MageCore.iceWallRework = Config.Bind(mageString, "Snapfreeze Rework", true, "Adds blast jumping to Snapfreeze.").Value;
            MageCore.ionSurgeRework = Config.Bind(mageString, "Ion Surge Rework", true, "Moves Ion Surge to the Utility slot and changes it into a blast jumping skill.").Value;

            MercCore.enabled = Config.Bind(mercString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            MercCore.modifyStats = Config.Bind(mercString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;
            MercCore.m1ComboFinishTweak = Config.Bind(mercString, "M1 Attack Speed Tweak (Client-Side)", true, "Makes the 3rd hit of Merc's M1 be unaffected by attack speed for use with combo tech.").Value;

            TreebotCore.enabled = Config.Bind(treebotString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            TreebotCore.drillChanges = Config.Bind(treebotString, "DIRECTIVE Drill Changes", true, "Enable changes to this skill.").Value;
            TreebotCore.swapUtilityEffects = Config.Bind(treebotString, "Utility - Swap Effects", true, "Swaps the effects of REXs Utilities").Value;
            ModifyUtilityForce.enabled = Config.Bind(treebotString, "Utility - Modify Force", true, "Modifies the force of REXs Utilities.").Value;
            TreebotCore.fruitChanges = Config.Bind(treebotString, "DIRECTIVE Harvest Changes", true, "Enable changes to this skill.").Value;

            LoaderCore.enabled = Config.Bind(loaderString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            LoaderCore.grappleCancelsSprint = Config.Bind(loaderString, "Secondaries Cancel Sprint", false, "Loader's Grapple cancels sprinting.").Value;
            LoaderCore.shiftCancelsSprint = Config.Bind(loaderString, "Utilities Cancel Sprint", false, "Loader's Big Punches cancel sprinting.").Value;
            LoaderCore.modifyStats = Config.Bind(loaderString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;
            LoaderCore.zapFistChanges = Config.Bind(loaderString, "Thunder Gauntlet Changes", true, "Enable changes to this skill.").Value;
            LoaderCore.slamChanges = Config.Bind(loaderString, "Thunderslam Changes", true, "Enable changes to this skill.").Value;
            LoaderCore.pylonChanges = Config.Bind(loaderString, "M551 Pylon Changes", true, "Enable changes to this skill.").Value;

            CrocoCore.enabled = Config.Bind(crocoString, "Enable Changes", true, "Enable changes to this survivor. Skill options unavailable due to all the changes being too interlinked.").Value;
            CrocoCore.gameplayRework = Config.Bind(crocoString, "Gameplay Rework", true, "A full rework of Acrid's skills.").Value;
            BiggerMeleeHitbox.enabled = Config.Bind(crocoString, "Extend Melee Hitbox", true, "Extends Acrid's melee hitbox so he can hit Vagrants while standing on top of them.").Value;
            BlightStack.enabled = Config.Bind(crocoString, "Blight Duration Reset", true, "Blight stacks like Bleed.").Value;
            RemovePoisonDamageCap.enabled = Config.Bind(crocoString, "Remove Poison Damage Cap", true, "Poison no longer has a hidden damage cap.").Value;
            BiggerLeapHitbox.enabled = Config.Bind(crocoString, "Extend Leap Collision Box", true, "Acrid's Shift skills have a larger collision hitbox. Damage radius remains the same.").Value;
            ShiftAirControl.enabled = Config.Bind(crocoString, "Leap Air Control", true, "Acrid's Shift skills gain increased air control at high move speeds.").Value;

            CaptainCore.enabled = Config.Bind(captainString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            CaptainOrbitalHiddenRealms.enabled = Config.Bind(captainString, "Hidden Realm Orbital Skills", true, "Allow Orbital skills in Hiden Realms.").Value;
            Microbots.enabled = Config.Bind(captainString, "Defensive Microbots Nerf", true, "Defensive Microbots no longer deletes stationary projectiles like gas clouds and Void Reaver mortars.").Value;
            CaptainCore.enablePrimarySkillChanges = Config.Bind(captainString, "Enable Primary Skill Changes", true, "Enable primary skill changes for this survivor.").Value;
            CaptainCore.modifyTaser = Config.Bind(captainString, "Power Taser Changes", true, "Enable changes to this skill.").Value;
            CaptainCore.nukeBuff = Config.Bind(captainString, "Diablo Strike Changes", true, "Enable changes to this skill.").Value;
            CaptainCore.beaconRework = Config.Bind(captainString, "Beacon Changes", true, "Beacons can be replaced on a cooldown and reworks Supply and Hack beacons.").Value;

            Bandit2Core.enabled = Config.Bind(banditString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            BanditSpecialGracePeriod.enabled = Config.Bind(banditString, "Special Grace Period", true, "Special On-kill effects can trigger if an enemy dies shortly after being hit.").Value;
            BanditSpecialGracePeriod.duration = Config.Bind(banditString, "Special Grace Period Duration", 1.2f, "Length in seconds of Special Grace Period.").Value;
            DesperadoRework.enabled = Config.Bind(banditString, "Persistent Desperado", true, "Desperado stacks are weaker but last between stages.").Value;
            BackstabRework.enabled = Config.Bind(banditString, "Backstab Changes", true, "Backstabs minicrit for 50% bonus damage and crit chance is converted to crit damage..").Value;
            Bandit2Core.burstChanges = Config.Bind(banditString, "Burst Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.blastChanges = Config.Bind(banditString, "Blast Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.knifeChanges = Config.Bind(banditString, "Serrated Dagger Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.knifeThrowChanges = Config.Bind(banditString, "Serrated Shiv Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.utilityFix = Config.Bind(banditString, "Smokebomb Fix", true, "Fixes various bugs with Smokebomb.").Value;
            Bandit2Core.specialRework = Config.Bind(banditString, "Special Rework", true, "Makes Resets/Desperado a selectable passive and adds a new Special skill.").Value;

            RailgunnerCore.enabled = Config.Bind(railgunnerString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            Survivors.DLC1.Railgunner.FixBungus.enabled = Config.Bind(railgunnerString, "Fix Bungus", true, "Removes self knockback force when on the ground.").Value;

            VoidFiendCore.enabled = Config.Bind(voidFiendString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            VoidFiendCore.fasterCorruptTransition = Config.Bind(voidFiendString, "Faster Corrupt Transition", true, "Speed up the corruption transform animation.").Value;
            VoidFiendCore.corruptOnKill = Config.Bind(voidFiendString, "Corruption on Kill", true, "Gain Corruption on kill. Lowers passive Corruption gain and Corrupted form duration.").Value;
            VoidFiendCore.corruptMeterTweaks = Config.Bind(voidFiendString, "Corruption Meter Tweaks", true, "Faster decay, slower passive buildup. Corrupted Suppress can be used as long as you have the HP for it. Meant to be used with Corrupt on Kill.").Value;
            VoidFiendCore.noCorruptCrit = Config.Bind(voidFiendString, "No Corruption on Crit", true, "Disables Corruption gain on crit.").Value;
            VoidFiendCore.noCorruptHeal = Config.Bind(voidFiendString, "No Corruption loss on Heal", true, "Disables Corruption loss on heal.").Value;
            VoidFiendCore.secondaryMultitask = Config.Bind(voidFiendString, "Secondary Multitasking", true, "Drown and Suppress can be fired while charging Flood.").Value;
        }
    }
}
