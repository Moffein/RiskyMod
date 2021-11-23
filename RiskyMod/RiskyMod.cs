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
using RiskyMod.Moon;
using Zio;
using Zio.FileSystems;
using System.Collections.Generic;
using RiskyMod.Survivors;
using RiskyMod.Enemies;
using RiskyMod.Survivors.Captain;
using RiskyMod.Enemies.Bosses;
using RiskyMod.Survivors.Bandit2;

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
    [BepInPlugin("com.RiskyLives.RiskyMod", "RiskyMod Beta", "0.3.0")]
    [R2API.Utils.R2APISubmoduleDependency(nameof(RecalculateStatsAPI), nameof(PrefabAPI),
        nameof(ProjectileAPI), nameof(EffectAPI), nameof(DamageAPI), nameof(BuffAPI)
        , nameof(LoadoutAPI))]
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
        private const string bossString = "Items - Boss";
        private const string lunarString = "Items - Lunar";
        private const string equipmentString = "Items - Equipment";
        private const string itemConfigDescString = "Enable changes to this item.";

        private const string monsterString = "Monsters";

        private const string commandoString = "Survivors: Commando";
        private const string banditString = "Survivors: Bandit";
        private const string captainString = "Survivors: Captain";

        public static bool disableProcChains = true;

        public static ItemDef emptyItemDef = null;
        public static BuffDef emptyBuffDef = null;

        public static AssistManager assistManager = null;

        public static PluginInfo pluginInfo;
        public static FileSystem fileSystem { get; private set; }

        public void Awake()
        {
            pluginInfo = Info;

            ReadConfig();
            CheckDependencies();

            RunFixes();
            RunTweaks();
            new ItemsCore();
            new DronesCore();
            new MoonCore();
            new SurvivorsCore();
            new EnemiesCore();
            SetupAssists();
            AddHooks();
            FunnyLanguage();
        }

        private void ReadConfig()
        {
            disableProcChains = Config.Bind(generalString, "Disable Proc Chains", true, "Remove the proc coefficient on most item effects.").Value;
            ShieldGating.enabled = Config.Bind(generalString, "Shield Gating", true, "Shields gate against HP damage.").Value;
            TrueOSP.enabled = Config.Bind(generalString, "True OSP", true, "Makes OSP work against multihits.").Value;

            RunScaling.enabled = Config.Bind(scalingString, "Linear Difficulty Scaling", true, "Makes difficulty scaling linear.").Value;
            NoLevelupHeal.enabled = Config.Bind(scalingString, "No Levelup Heal", true, "Monsters don't gain HP when leveling up.").Value;
            NoLevelupHeal.enabled = Config.Bind(scalingString, "No Partial Levels", true, "Monsters can't gain partial levels.").Value;
            RemoveLevelCap.enabled = Config.Bind(scalingString, "Increase Monster Level Cap", true, "Increases Monster Level Cap.").Value;
            RemoveLevelCap.maxLevel = Config.Bind(scalingString, "Increase Monster Level Cap - Max Level", 1000f, "Maximum monster level if Increase Monster Level Cap is enabled.").Value;

            DronesCore.enabled = Config.Bind(coreModuleString, "Drone Changes", true, "Enable drone and ally changes.").Value;
            ItemsCore.enabled = Config.Bind(coreModuleString, "Item Changes", true, "Enable item changes.").Value;
            SurvivorsCore.enabled = Config.Bind(coreModuleString, "Survivor Changes", true, "Enable survivor changes.").Value;
            EnemiesCore.enabled = Config.Bind(coreModuleString, "Monster Changes", true, "Enable enemy changes.").Value;
            MoonCore.enabled = Config.Bind(coreModuleString, "Moon Changes", true, "Enable Moon changes.").Value;

            TeleExpandOnBossKill.enabled = Config.Bind(tweakString, "Tele Expand on Boss Kill", true, "Teleporter expands to cover the whole map when the boss is killed.").Value;
            SmallHoldoutCharging.enabled = Config.Bind(tweakString, "Small Holdout Charging", true, "Void/Moon Holdouts charge at max speed as long as 1 player is charging.").Value;
            ShrineCombatItems.enabled = Config.Bind(tweakString, "Shrine of Combat Drops Items", true, "Shrine of Combat drops items for the team on completion.").Value;
            DistantRoostCredits.enabled = Config.Bind(tweakString, "Distant Roost Credit Boost", true, "Makes Distant Roost have the same director credits as Titanic Plains.").Value;
            FixSlayer.enabled = Config.Bind(tweakString, "Fix Slayer Procs", true, "Bandit/Acrid bonus damage to low hp effect now applies to procs.").Value;
            SceneDirectorMonsterRewards.enabled = Config.Bind(tweakString, "SceneDirector Monster Rewards", true, "Monsters that spawn with the map now give the same rewards as teleporter monsters.").Value;
            VengeancePercentHeal.enabled = Config.Bind(tweakString, "Reduce Vengeance Healing", true, "Vengeance Doppelgangers receive reduced healing from percent-based healing effects.").Value;

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

            AtG.enabled = Config.Bind(uncommonString, "AtG Missile", true, itemConfigDescString).Value;
            Bandolier.enabled = Config.Bind(uncommonString, "Bandolier", true, itemConfigDescString).Value;
            Berzerker.enabled = Config.Bind(uncommonString, "Berzerkers Pauldron", true, itemConfigDescString).Value;
            Chronobauble.enabled = Config.Bind(uncommonString, "Chronobauble", true, itemConfigDescString).Value;
            ElementalBands.enabled = Config.Bind(uncommonString, "Runalds and Kjaros Bands", true, itemConfigDescString).Value;
            Guillotine.enabled = Config.Bind(uncommonString, "Old Guillotine", true, itemConfigDescString).Value;
            HarvesterScythe.enabled = Config.Bind(uncommonString, "Harvesters Scythe", true, itemConfigDescString).Value;
            Infusion.enabled = Config.Bind(uncommonString, "Infusion", true, itemConfigDescString).Value;
            LeechingSeed.enabled = Config.Bind(uncommonString, "Leeching Seed", true, itemConfigDescString).Value;
            Predatory.enabled = Config.Bind(uncommonString, "Predatory Instincts", true, itemConfigDescString).Value;
            Razorwire.enabled = Config.Bind(uncommonString, "Razorwire", true, itemConfigDescString).Value;
            RoseBuckler.enabled = Config.Bind(uncommonString, "Rose Buckler", true, itemConfigDescString).Value;
            SquidPolyp.enabled = Config.Bind(uncommonString, "Squid Polyp", true, itemConfigDescString).Value;
            Stealthkit.enabled = Config.Bind(uncommonString, "Old War Stealthkit", true, itemConfigDescString).Value;
            Ukulele.enabled = Config.Bind(uncommonString, "Ukulele", true, itemConfigDescString).Value;
            WarHorn.enabled = Config.Bind(uncommonString, "War Horn", true, itemConfigDescString).Value;
            WillOWisp.enabled = Config.Bind(uncommonString, "Will-o-the-Wisp", true, itemConfigDescString).Value;

            FrostRelic.enabled = Config.Bind(legendaryString, "Frost Relic", true, itemConfigDescString).Value;
            FrostRelic.removeFOV = Config.Bind(legendaryString, "Frost Relic - Disable FOV Modifier", true, "Disables FOV modifier.").Value;
            FrostRelic.removeBubble = Config.Bind(legendaryString, "Frost Relic - Disable Bubble", true, "Disables bubble visuals.").Value;
            HeadHunter.enabled = Config.Bind(legendaryString, "Wake of Vultures", true, itemConfigDescString).Value;
            Headstompers.enabled = Config.Bind(legendaryString, "H3AD-ST", true, itemConfigDescString).Value;
            Tesla.enabled = Config.Bind(legendaryString, "Unstable Tesla Coil", true, itemConfigDescString).Value;
            CeremonialDagger.enabled = Config.Bind(legendaryString, "Ceremonial Dagger", true, itemConfigDescString).Value;
            MeatHook.enabled = Config.Bind(legendaryString, "Sentient Meat Hook", true, itemConfigDescString).Value;

            MoltenPerf.enabled = Config.Bind(bossString, "Molten Perforator", true, itemConfigDescString).Value;
            ChargedPerf.enabled = Config.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            Disciple.enabled = Config.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            QueensGland.enabled = Config.Bind(bossString, "Queens Gland", true, itemConfigDescString).Value;
            Shatterspleen.enabled = Config.Bind(bossString, "Shatterspleen", true, itemConfigDescString).Value;

            ShapedGlass.enabled = Config.Bind(lunarString, "Shaped Glass", true, itemConfigDescString).Value;
            BrittleCrown.enabled = Config.Bind(lunarString, "Brittle Crown", true, itemConfigDescString).Value;
            Transcendence.enabled = Config.Bind(lunarString, "Transcendence", true, itemConfigDescString).Value;
            Meteorite.enabled = Config.Bind(lunarString, "Glowing Meteorite", true, itemConfigDescString).Value;

            Backup.enabled = Config.Bind(equipmentString, "The Back-Up", true, itemConfigDescString).Value;
            BFG.enabled = Config.Bind(equipmentString, "Preon Accumulator", true, itemConfigDescString).Value;
            Capacitor.enabled = Config.Bind(equipmentString, "Royal Capacitor", true, itemConfigDescString).Value;
            Chrysalis.enabled = Config.Bind(equipmentString, "Milky Chrysalis", true, itemConfigDescString).Value;
            CritHud.enabled = Config.Bind(equipmentString, "Ocular HUD", true, itemConfigDescString).Value;
            VolcanicEgg.enabled = Config.Bind(equipmentString, "Volcanic Egg", true, itemConfigDescString).Value;

            Vagrant.enabled = Config.Bind(monsterString, "Wandering Vagrant", true, "Enable changes to this monster.").Value;

            CaptainCore.enabled = Config.Bind(captainString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            CaptainOrbitalHiddenRealms.enabled = Config.Bind(captainString, "Hidden Realm Orbital Skills", true, "Allow Orbital skills in Hiden Realms.").Value;
            Microbots.enabled = Config.Bind(captainString, "Defensive Microbots Nerf", true, "Defensive Microbots no longer deletes stationary projectiles like gas clouds and Void Reaver mortars.").Value;
            Shock.enabled = Config.Bind(captainString, "No Shock Interrupt", true, "Shock is no longer interrupted by damage.").Value;
            CaptainCore.enablePrimarySkillChanges = Config.Bind(captainString, "Enable Primary Skill Changes", true, "Enable primary skill changes for this survivor.").Value;

            Bandit2Core.enabled = Config.Bind(banditString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            BanditSpecialGracePeriod.enabled = Config.Bind(banditString, "Special Grace Period", true, "Special On-kill effects can trigger if an enemy dies shortly after being hit.").Value;
            BanditSpecialGracePeriod.duration = Config.Bind(banditString, "Special Grace Period Duration", 1.2f, "Length in seconds of Special Grace Period.").Value;
            Bandit2Core.enablePassiveSkillChanges = Config.Bind(banditString, "Enable Passive Skill Changes", true, "Enable passive skill changes for this survivor.").Value;
            Bandit2Core.enablePrimarySkillChanges = Config.Bind(banditString, "Enable Primary Skill Changes", true, "Enable primary skill changes for this survivor.").Value;
            Bandit2Core.enableSecondarySkillChanges = Config.Bind(banditString, "Enable Secondary Skill Changes", true, "Enable secondary skill changes for this survivor.").Value;
            Bandit2Core.enableUtilitySkillChanges = Config.Bind(banditString, "Enable Utility Skill Changes", true, "Enable utility skill changes for this survivor.").Value;
            Bandit2Core.enableSpecialSkillChanges = Config.Bind(banditString, "Enable Special Skill Changes", true, "Enable special skill changes for this survivor.").Value;
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
            new NoLevelupHeal();
            new RemoveLevelCap();
            new SmallHoldoutCharging();
            new TeleExpandOnBossKill();
            new DistantRoostCredits();
            new ShrineCombatItems();
            new VengeancePercentHeal();
            new NoPartialLevels();
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
