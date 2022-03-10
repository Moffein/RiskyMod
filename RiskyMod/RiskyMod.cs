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

namespace RiskyMod
{
    [BepInDependency("com.PlasmaCore.StickyStunter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.MobileTurretBuff", BepInDependency.DependencyFlags.SoftDependency)]
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
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TheTimeSweeper.AcridHitboxBuff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AcridBlightStack", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.LunarWispFalloff", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BeetleQueenPlus", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.NoVoidAllies", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.EliteReworks", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.RiskyLives.RiskyMod", "RiskyMod Beta", "0.6.0")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(RecalculateStatsAPI), nameof(PrefabAPI), nameof(DamageAPI), nameof(ContentAddition), nameof(LanguageAPI))]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class RiskyMod : BaseUnityPlugin
    {
        private const string generalString = "General";
        private const string scalingString = "General - Run Scaling";
        private const string tweakString = "General - Tweaks";
        private const string coreModuleString = "General - Core Modules";

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

        private const string fireSelectString = "Survivors - Firemode Selection (Client-Side)";

        public static bool disableProcChains = true;

        public static bool ScepterPluginLoaded = false;
        public static bool AIBlacklistLoaded = false;
        public static bool AIBlacklistUseVanillaBlacklist = true;

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
            RunFixes();
            RunTweaks();
            new ItemsCore();
            new DronesCore();
            new MoonCore();
            new SurvivorsCore();
            new FireSelect();
            new EnemiesCore();
            SetupAssists();
            AddHooks();
            FunnyLanguage();
        }

        private void CheckDependencies()
        {
            NoLevelupHeal.enabled = NoLevelupHeal.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoLevelupHeal");
            RemoveLevelCap.enabled = RemoveLevelCap.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.RaiseMonsterLevelCap");
            FixMercExpose.enabled = FixMercExpose.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.MercExposeFix");

            FixPlayercount.enabled = FixPlayercount.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.FixPlayercount");
            FixPlayercount.MultitudesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("dev.wildbook.multitudes");
            FixPlayercount.ZetArtifactsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TPDespair.ZetArtifacts");

            NoVoidAllies.enabled = NoVoidAllies.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.NoVoidAllies");

            AIBlacklistLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AI_Blacklist");
            FixVengeanceLeveling.enabled = FixVengeanceLeveling.enabled && !AIBlacklistLoaded;
            if (AIBlacklistLoaded)
            {
                HandleAIBlacklist();
            }

            PreventArtifactHeal.enabled = PreventArtifactHeal.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.ArtifactReliquaryHealingFix");
            CaptainOrbitalHiddenRealms.enabled = CaptainOrbitalHiddenRealms.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.PlasmaCore.AlwaysAllowSupplyDrops");
            CritHud.enabled = CritHud.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.Hypercrit");   //Effect is already a part of hypercrit

            RevertBurnDamage.enabled = RevertBurnDamage.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.EliteReworks");

            ScepterPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");

            //Croco
            BiggerMeleeHitbox.enabled = BiggerMeleeHitbox.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TheTimeSweeper.AcridHitboxBuff");
            BlightStack.enabled = BlightStack.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AcridBlightStack");

            //Enemies
            BeetleQueen.enabled = BeetleQueen.enabled && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.BeetleQueenPlus");
            LunarWisp.enableFalloff = LunarWisp.enableFalloff && !BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.LunarWispFalloff");
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void HandleAIBlacklist()
        {
            AIBlacklistUseVanillaBlacklist = AI_Blacklist.AIBlacklist.useVanillaAIBlacklist;
        }

        private void RunTweaks()
        {
            new RunScaling();
            new TrueOSP();
            new ShieldGating();
            new SceneDirectorMonsterRewards();
            new NoLevelupHeal();
            new RemoveLevelCap();
            new SmallHoldoutCharging();
            new TeleExpandOnBossKill();
            new DistantRoostCredits();
            new ShrineCombatItems();
            new VengeancePercentHeal();
            new CloakBuff();
            new SmallHoldoutRadius();
            new BloodShrineMinReward();
            new ShrineSpawnRate();
            new Shock();
            new FixSlayer();
            new BarrierDecay();
            new FreezeChampionExecute();
            new LoopBossArmor();
            new PlayerControlledMonsters();
            new NoVoidAllies();
            new RevertBurnDamage();
        }

        private void RunFixes()
        {
            new FixMercExpose();
            new FixPlayercount();
            new FixVengeanceLeveling();
            new FixFocusCrystalSelfDamage();
            new PreventArtifactHeal();
            new TreebotFruitingNullref();
            new FixLightningStrikeOrbProcCoefficient();
            new FixCrocoPoisonAchievement();
            new FixGhostMinonSpawns();
            new FixWormFallDeath();
            new FixHereticFreeze();
        }
        
        private void AddHooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += OnHitEnemy.GlobalEventManager_OnHitEnemy;
            RecalculateStatsAPI.GetStatCoefficients += GetStatsCoefficient.RecalculateStatsAPI_GetStatCoefficients;
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

        private void FunnyLanguage()
        {
            /*PhysicalFileSystem physicalFileSystem = new PhysicalFileSystem();
            RiskyMod.fileSystem = new SubFileSystem(physicalFileSystem, physicalFileSystem.ConvertPathFromInternal(Assets.assemblyDir), true);
            if (RiskyMod.fileSystem.DirectoryExists("/language/")) //Uh, it exists and we make sure to not shit up R2Api
            {
                Language.collectLanguageRootFolders += delegate (List<string> list)
                {
                    list.Add(RiskyMod.fileSystem.GetDirectoryEntry("/language/").ToString());   //doublecheck this, ToString was added in without testing
                };
            }*/
            RegisterTokens(@"\en");
        }

        private void RegisterTokens(string languageString)
        {
            string pathToLanguage = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\language";
            LanguageAPI.AddPath(System.IO.Path.Combine(pathToLanguage + languageString, "Survivors.txt"));
            LanguageAPI.AddPath(System.IO.Path.Combine(pathToLanguage + languageString, "Equipment.txt"));
            LanguageAPI.AddPath(System.IO.Path.Combine(pathToLanguage + languageString, "Items.txt"));
            LanguageAPI.AddPath(System.IO.Path.Combine(pathToLanguage + languageString, "Keywords.txt"));
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

            RunScaling.enabled = Config.Bind(scalingString, "Linear Difficulty Scaling", true, "Makes difficulty scaling linear.").Value;
            NoLevelupHeal.enabled = Config.Bind(scalingString, "No Levelup Heal", true, "Monsters don't gain HP when leveling up.").Value;
            RemoveLevelCap.enabled = Config.Bind(scalingString, "Increase Monster Level Cap", true, "Increases Monster Level Cap.").Value;
            RemoveLevelCap.maxLevel = Config.Bind(scalingString, "Increase Monster Level Cap - Max Level", 1000f, "Maximum monster level if Increase Monster Level Cap is enabled.").Value;
            LoopBossArmor.enabled = Config.Bind(scalingString, "Loop Boss Armor", true, "Teleporter bosses gain bonus armor when looping.").Value;

            DronesCore.enabled = Config.Bind(coreModuleString, "Drone Changes", true, "Enable drone and ally changes.").Value;
            ItemsCore.enabled = Config.Bind(coreModuleString, "Item Changes", true, "Enable item changes.").Value;
            SurvivorsCore.enabled = Config.Bind(coreModuleString, "Survivor Changes", true, "Enable survivor changes.").Value;
            EnemiesCore.modifyEnemies = Config.Bind(coreModuleString, "Monster Changes", true, "Enable enemy changes.").Value;
            MoonCore.enabled = Config.Bind(coreModuleString, "Moon Changes", true, "Enable Moon changes.").Value;

            PlayerControlledMonsters.enabled = Config.Bind(tweakString, "Player-Controlled Monster Regen", true, "Gives players health regen + armor when playing as monsters via mods.").Value;
            TeleExpandOnBossKill.enabled = Config.Bind(tweakString, "Tele Expand on Boss Kill", true, "Teleporter expands to cover the whole map when the boss is killed.").Value;
            SmallHoldoutCharging.enabled = Config.Bind(tweakString, "Small Holdout Charging", true, "Void/Moon Holdouts charge at max speed as long as 1 player is charging.").Value;
            SmallHoldoutRadius.enabled = Config.Bind(tweakString, "Small Holdout Radius", true, "Void/Moon Holdouts have increased radius.").Value;
            ShrineSpawnRate.enabled = Config.Bind(tweakString, "Mountain/Combat Shrine Playercount Scaling", true, "Mountain/Combat Shrine Director Credit cost scales with playercount.").Value;
            ShrineCombatItems.enabled = Config.Bind(tweakString, "Shrine of Combat Drops Items", true, "Shrine of Combat drops items for the team on completion.").Value;
            BloodShrineMinReward.enabled = Config.Bind(tweakString, "Shrine of Blood Minimum Reward", true, "Shrine of Blood always gives at least enough money to buy a small chest.").Value;
            DistantRoostCredits.enabled = Config.Bind(tweakString, "Distant Roost Credit Boost", true, "Makes Distant Roost have the same director credits as Titanic Plains.").Value;
            FixSlayer.enabled = Config.Bind(tweakString, "Fix Slayer Procs", true, "Bandit/Acrid bonus damage to low hp effect now applies to procs.").Value;
            SceneDirectorMonsterRewards.enabled = Config.Bind(tweakString, "SceneDirector Monster Rewards", true, "Monsters that spawn with the map now give the same rewards as teleporter monsters.").Value;
            VengeancePercentHeal.enabled = Config.Bind(tweakString, "Reduce Vengeance Healing", true, "Vengeance Doppelgangers receive reduced healing from percent-based healing effects.").Value;
            CloakBuff.enabled = Config.Bind(tweakString, "Cloak Buff", true, "Increases delay between position updates while cloaked.").Value;
            Shock.enabled = Config.Bind(tweakString, "No Shock Interrupt", true, "Shock is no longer interrupted by damage.").Value;
            BarrierDecay.enabled = Config.Bind(tweakString, "Barrier Decay", true, "Barrier decays slower at low barrier values.").Value;
            FreezeChampionExecute.enabled = Config.Bind(tweakString, "Freeze Executes Bosses", true, "Freeze counts as a debuff and can execute bosses at 15% HP.").Value;
            NoVoidAllies.enabled = Config.Bind(tweakString, "No Void Infestor Ally Possession", true, "Void Infestors can't possess allies.").Value;
            RevertBurnDamage.enabled = Config.Bind(tweakString, "Revert Blazing Damage", true, "Reverts Blazing burn duration to its pre-DLC1 Update behavior.").Value;

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

        private void ConfigCommonItems()
        {
            BisonSteak.enabled = Config.Bind(commonString, "Bison Steak", true, itemConfigDescString).Value;
            CritGlasses.enabled = Config.Bind(commonString, "Lensmakers Glasses", true, itemConfigDescString).Value;
            Crowbar.enabled = Config.Bind(commonString, "Crowbar", true, itemConfigDescString).Value;
            Fireworks.enabled = Config.Bind(commonString, "Fireworks", true, itemConfigDescString).Value;
            Fireworks.maxRockets = Config.Bind(commonString, "Fireworks - Max Rockets", 32, "Max rockets to spawn.").Value;
            Gasoline.enabled = Config.Bind(commonString, "Gasoline", true, itemConfigDescString).Value;
            MonsterTooth.enabled = Config.Bind(commonString, "Monster Tooth", true, itemConfigDescString).Value;
            StickyBomb.enabled = Config.Bind(commonString, "Stickybomb", true, itemConfigDescString).Value;
            TougherTimes.enabled = Config.Bind(commonString, "Tougher Times", true, itemConfigDescString).Value;
            Warbanner.enabled = Config.Bind(commonString, "Warbanner", true, itemConfigDescString).Value;
        }

        private void ConfigUncommonItems()
        {
            AtG.enabled = Config.Bind(uncommonString, "AtG Missile", true, itemConfigDescString).Value;
            Bandolier.enabled = Config.Bind(uncommonString, "Bandolier", true, itemConfigDescString).Value;
            Berzerker.enabled = Config.Bind(uncommonString, "Berzerkers Pauldron", true, itemConfigDescString).Value;
            Chronobauble.enabled = Config.Bind(uncommonString, "Chronobauble", true, itemConfigDescString).Value;
            ElementalBands.enabled = Config.Bind(uncommonString, "Runalds and Kjaros Bands", true, itemConfigDescString).Value;
            Guillotine.enabled = Config.Bind(uncommonString, "Old Guillotine", true, itemConfigDescString).Value;
            Guillotine.reduceVFX = Config.Bind(uncommonString, "Old Guillotine - Reduce VFX", true, "Reduce how often this item's VFX shows up.").Value;
            HarvesterScythe.enabled = Config.Bind(uncommonString, "Harvesters Scythe", true, itemConfigDescString).Value;
            Infusion.enabled = Config.Bind(uncommonString, "Infusion", true, itemConfigDescString).Value;
            LeechingSeed.enabled = Config.Bind(uncommonString, "Leeching Seed", true, itemConfigDescString).Value;
            Predatory.enabled = Config.Bind(uncommonString, "Predatory Instincts", true, itemConfigDescString).Value;
            Razorwire.enabled = Config.Bind(uncommonString, "Razorwire", true, itemConfigDescString).Value;
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
            FrostRelic.enabled = Config.Bind(legendaryString, "Frost Relic", true, itemConfigDescString).Value;
            FrostRelic.removeFOV = Config.Bind(legendaryString, "Frost Relic - Disable FOV Modifier", true, "Disables FOV modifier.").Value;
            FrostRelic.removeBubble = Config.Bind(legendaryString, "Frost Relic - Disable Bubble", true, "Disables bubble visuals.").Value;
            HeadHunter.enabled = Config.Bind(legendaryString, "Wake of Vultures", true, itemConfigDescString).Value;
            HeadHunter.perfectedTweak = Config.Bind(legendaryString, "Wake of Vultures - Perfected Tweak", true, "Perfected Affix gained via Wake of Vultures will not force your health pool to bec").Value;
            Headstompers.enabled = Config.Bind(legendaryString, "H3AD-ST", true, itemConfigDescString).Value;
            Tesla.enabled = Config.Bind(legendaryString, "Unstable Tesla Coil", true, itemConfigDescString).Value;
            CeremonialDagger.enabled = Config.Bind(legendaryString, "Ceremonial Dagger", true, itemConfigDescString).Value;
            MeatHook.enabled = Config.Bind(legendaryString, "Sentient Meat Hook", true, itemConfigDescString).Value;
            Behemoth.enabled = Config.Bind(legendaryString, "Brilliant Behemoth", true, itemConfigDescString).Value;
            LaserTurbine.enabled = Config.Bind(legendaryString, "Resonance Disc", true, itemConfigDescString).Value;
            HappiestMask.enabled = Config.Bind(legendaryString, "Happiest Mask", true, itemConfigDescString).Value;
            HappiestMask.scaleCount = Config.Bind(legendaryString, "Happiest Mask - Stacks Increase Max Ghosts", false, "Extra stacks allow for more ghosts to spawn. Will lag in MP.").Value;
            HappiestMask.noGhostLimit = Config.Bind(legendaryString, "Happiest Mask - Remove Ghost Limit", false, "Removes the ghost limit at all times. Definitely will lag.").Value;

            //Disabled for now, trying to get a feel for the balance
            //Turns out SS2's Gadget increased initial hit damage by 50%, which lead to a 3x total damage multiplier, which is what this item does already.
            //LaserScope.enabled = Config.Bind(legendaryString, "Laser Scope", true, itemConfigDescString).Value;
            LaserScope.enabled = false;

            BottledChaos.enabled = Config.Bind(legendaryString, "Bottled Chaos", true, itemConfigDescString).Value;
        }

        private void ConfigVoidItems()
        {
            SaferSpaces.enabled = Config.Bind(voidString, "Safer Spaces", true, itemConfigDescString).Value;
            PlasmaShrimp.enabled = Config.Bind(voidString, "Plasma Shrimp", true, itemConfigDescString).Value;
            VoidWisp.enabled = Config.Bind(voidString, "Voidsent Flame", true, itemConfigDescString).Value;
            Polylute.enabled = Config.Bind(voidString, "Polylute", true, itemConfigDescString).Value;
            VoidRing.enabled = Config.Bind(voidString, "Singularity Band", true, itemConfigDescString).Value;
        }

        private void ConfigBossItems()
        {
            MoltenPerf.enabled = Config.Bind(bossString, "Molten Perforator", true, itemConfigDescString).Value;
            ChargedPerf.enabled = Config.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            Disciple.enabled = Config.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            QueensGland.enabled = Config.Bind(bossString, "Queens Gland", true, itemConfigDescString).Value;
            Shatterspleen.enabled = Config.Bind(bossString, "Shatterspleen", true, itemConfigDescString).Value;
        }

        private void ConfigLunars()
        {
            ShapedGlass.enabled = Config.Bind(lunarString, "Shaped Glass", true, itemConfigDescString).Value;
            BrittleCrown.enabled = Config.Bind(lunarString, "Brittle Crown", true, itemConfigDescString).Value;
            Transcendence.enabled = Config.Bind(lunarString, "Transcendence", true, itemConfigDescString).Value;
            Meteorite.enabled = Config.Bind(lunarString, "Glowing Meteorite", true, itemConfigDescString).Value;
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
            VolcanicEgg.enabled = Config.Bind(equipmentString, "Volcanic Egg", true, itemConfigDescString).Value;
            SuperLeech.enabled = Config.Bind(equipmentString, "Super Massive Leech", true, itemConfigDescString).Value;
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
        }

        private void ConfigFireSelect()
        {
            FireSelect.scrollSelection = Config.Bind(fireSelectString, "Use Scrollwheel", true, "Mouse Scroll Wheel changes firemode").Value;
            FireSelect.swapButton = Config.Bind(fireSelectString, "Next Firemode", KeyCode.None, "Button to swap to the next firemode.").Value;
            FireSelect.prevButton = Config.Bind(fireSelectString, "Previous Firemode", KeyCode.None, "Button to swap to the previous firemode.").Value;
            EngiFireModes.enabled = Config.Bind(fireSelectString, "Engineer: Enable Fire Select", false, "Enable firemode selection for Engineer's Bouncing Grenades.").Value;
            EngiFireModes.defaultButton = Config.Bind(fireSelectString, "Engineer: Swap to Default", KeyCode.None, "Button to swap to the Default firemode.").Value;
            EngiFireModes.autoButton = Config.Bind(fireSelectString, "Engineer: Swap to Auto", KeyCode.None, "Button to swap to the Auto firemode.").Value;
            EngiFireModes.holdButton = Config.Bind(fireSelectString, "Engineer: Swap to Hold", KeyCode.None, "Button to swap to the Charged firemode.").Value;
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
            CommandoCore.phaseBlastChanges = Config.Bind(commandoString, "Phase Blast Changes", true, "Enable changes to this skill.").Value;
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
            HuntressCore.phaseBlinkChanges = Config.Bind(huntressString, "Phase Blink Changes", true, "Enable changes to this skill.").Value;
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
            M1Projectiles.increaseRange = Config.Bind(mageString, "Primary Range Increase", true, "Primary projectiles no longer disappear mid-flight.").Value;
            MageCore.m2Buffer = Config.Bind(mageString, "Secondary Buffer Time", true, "Adds a 0.6s duration between Secondary uses if you hold down the button.").Value;
            MageCore.m2RequiresKeypress = Config.Bind(mageString, "Secondary Requires Keypress", false, "Each Secondary use requires you to re-press the button.").Value;
            MageCore.flamethrowerAttackSpeed = Config.Bind(mageString, "Flamethrower - Attack Speed Scaling", true, "Flamethrower shot count and fire rate increases with attack speed.").Value;
            MageCore.flamethrowerSprintCancel = Config.Bind(mageString, "Flamethrower - Sprint Cancel", true, "Sprinting cancels Flamethrower.").Value;
            EntityStates.RiskyMod.Mage.FlamethrowerScepter.maxFlames = Config.Bind(mageString, "Flamethrower - Scepter Max Flames", 30, "Max napalm pools left behind by the Scepter Flamethrower if Flamethrower changes are enabled.").Value;
            MageCore.ionSurgeMovementScaling = Config.Bind(mageString, "Ion Surge - Movement Scaling", false, "Ion Surge jump height scales with movement speed.").Value;
            MageCore.ionSurgeShock = Config.Bind(mageString, "Ion Surge - Shock", true, "Ion Surge shocks enemies.").Value;

            MercCore.enabled = Config.Bind(mercString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;

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

            Bandit2Core.enabled = Config.Bind(banditString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            BanditSpecialGracePeriod.enabled = Config.Bind(banditString, "Special Grace Period", true, "Special On-kill effects can trigger if an enemy dies shortly after being hit.").Value;
            BanditSpecialGracePeriod.duration = Config.Bind(banditString, "Special Grace Period Duration", 1.2f, "Length in seconds of Special Grace Period.").Value;
            DesperadoRework.enabled = Config.Bind(banditString, "Persistent Desperado", true, "Desperado stacks are weaker but last between stages.").Value;
            Bandit2Core.backstabNerf = Config.Bind(banditString, "Backstab Nerf", true, "Backstabs Minicrit for 50% bonus damage (stacks with crit).").Value;
            Bandit2Core.burstChanges = Config.Bind(banditString, "Burst Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.blastChanges = Config.Bind(banditString, "Blast Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.knifeChanges = Config.Bind(banditString, "Serrated Dagger Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.knifeThrowChanges = Config.Bind(banditString, "Serrated Shiv Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.utilityFix = Config.Bind(banditString, "Smokebomb Fix", true, "Fixes various bugs with Smokebomb.").Value;
            Bandit2Core.specialRework = Config.Bind(banditString, "Special Rework", true, "Makes Resets/Desperado a selectable passive and adds a new Special skill.").Value;
        }
    }
}
