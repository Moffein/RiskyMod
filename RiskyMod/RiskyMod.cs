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
            ConfigFiles.Init();
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
            SlowDownProjectilesModifyDamage.enabled = Config.Bind(charMechanicsString, "Slowed Projectiles Deal Less Damage", true, "Projectiles slowed by Railgunners Polar Field Device and similar skills deal less damage.").Value;

            ShrineSpawnRate.enabled = Config.Bind(shrineString, "Mountain/Combat Shrine Playercount Scaling", true, "Mountain/Combat Shrine Director Credit cost scales with playercount.").Value;
            ShrineCombatItems.enabled = Config.Bind(shrineString, "Shrine of Combat Drops Items", true, "Shrine of Combat drops items for the team on completion.").Value;
            BloodShrineMinReward.enabled = Config.Bind(shrineString, "Shrine of Blood Minimum Reward", true, "Shrine of Blood always gives at least enough money to buy a small chest.").Value;

            VengeancePercentHeal.enabled = Config.Bind(artifactString, "Reduce Vengeance Healing", true, "Vengeance Doppelgangers receive reduced healing from percent-based healing effects.").Value;
            EnigmaBlacklist.enabled = Config.Bind(artifactString, "Enigma Blacklist", true, "Blacklist Lunars and Recycler from the Artifact of Enigma.").Value;


            ConfigMoon();
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
    }
}
