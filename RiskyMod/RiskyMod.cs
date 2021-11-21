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
            new MoonCore();
            new SurvivorsCore();
            new EnemiesCore();
            SetupAssists();
            AddHooks();
            FunnyLanguage();
        }

        private void ReadConfig()
        {
            disableProcChains = Config.Bind("00 - General", "Disable Proc Chains", true, "Remove the proc coefficient on most item effects.").Value;
            ShieldGating.enabled = Config.Bind("00 - General", "Shield Gating", true, "Shields gate against HP damage.").Value;
            TrueOSP.enabled = Config.Bind("00 - General", "True OSP", true, "Makes OSP work against multihits.").Value;

            RunScaling.enabled = Config.Bind("01 - Run Scaling", "Linear Difficulty Scaling", true, "Makes difficulty scaling linear.").Value;
            NoLevelupHeal.enabled = Config.Bind("01 - Run Scaling", "No Levelup Heal", true, "Monsters don't gain HP when leveling up.").Value;
            RemoveLevelCap.enabled = Config.Bind("01 - Run Scaling", "Increase Monster Level Cap", true, "Increases Monster Level Cap.").Value;
            RemoveLevelCap.maxLevel = Config.Bind("01 - Run Scaling", "Max Monster Level", 1000f, "Maximum monster level if Increase Monster Level Cap is enabled.").Value;

            DronesCore.enabled = Config.Bind("02 - Core Modules", "Drone Changes", true, "Enable drone and ally changes.").Value;
            ItemsCore.enabled = Config.Bind("02 - Core Modules", "Item Changes", true, "Enable item changes.").Value;
            SurvivorsCore.enabled = Config.Bind("02 - Core Modules", "Survivor Changes", true, "Enable survivor changes.").Value;
            EnemiesCore.enabled = Config.Bind("02 - Core Modules", "Monster Changes", true, "Enable enemy changes.").Value;
            MoonCore.enabled = Config.Bind("02 - Core Modules", "Moon Changes", true, "Enable Moon changes.").Value;

            TeleExpandOnBossKill.enabled = Config.Bind("03 - Tweaks", "Tele Expand on Boss Kill", true, "Teleporter expands to cover the whole map when the boss is killed.").Value;
            SmallHoldoutCharging.enabled = Config.Bind("03 - Tweaks", "Small Holdout Charging", true, "Void/Moon Holdouts charge at max speed as long as 1 player is charging.").Value;
            ShrineCombatItems.enabled = Config.Bind("03 - Tweaks", "Shrine of Combat Drops Items", true, "Shrine of Combat drops items for the team on completion.").Value;
            DistantRoostCredits.enabled = Config.Bind("03 - Tweaks", "Distant Roost Credit Boost", true, "Makes Distant Roost have the same director credits as Titanic Plains.").Value;
            FixSlayer.enabled = Config.Bind("03 - Tweaks", "Fix Slayer Procs", true, "Bandit/Acrid bonus damage to low hp effect now applies to procs.").Value;
            SceneDirectorMonsterRewards.enabled = Config.Bind("03 - Tweaks", "SceneDirector Monster Rewards", true, "Monsters that spawn with the map now give the same rewards as teleporter monsters.").Value;
            VengeancePercentHeal.enabled = Config.Bind("03 - Tweaks", "Reduce Vengeance Healing", true, "Vengeance Doppelgangers receive reduced healing from percent-based healing effects.").Value;

            BisonSteak.enabled = Config.Bind("04 - Common Items", "Bison Steak", true, "Enable changes to this item.").Value;
            CritGlasses.enabled = Config.Bind("04 - Common Items", "Lensmakers Glasses", true, "Enable changes to this item.").Value;
            Crowbar.enabled = Config.Bind("04 - Common Items", "Crowbar", true, "Enable changes to this item.").Value;
            Fireworks.enabled = Config.Bind("04 - Common Items", "Fireworks", true, "Enable changes to this item.").Value;
            Fireworks.maxRockets = Config.Bind("04 - Common Items", "Fireworks - Max Rockets", 32, "Max rockets to spawn.").Value;
            Gasoline.enabled = Config.Bind("04 - Common Items", "Gasoline", true, "Enable changes to this item.").Value;
            MonsterTooth.enabled = Config.Bind("04 - Common Items", "Monster Tooth", true, "Enable changes to this item.").Value;
            StickyBomb.enabled = Config.Bind("04 - Common Items", "Stickybomb", true, "Enable changes to this item.").Value;
            //TopazBrooch.enabled = Config.Bind("04 - Common Items", "Topaz Brooch", true, "Allow this item to trigger on assist.").Value;
            TougherTimes.enabled = Config.Bind("04 - Common Items", "Tougher Times", true, "Enable changes to this item.").Value;
            Warbanner.enabled = Config.Bind("04 - Common Items", "Warbanner", true, "Enable changes to this item.").Value;

            AtG.enabled = Config.Bind("05 - Uncommon Items", "AtG Missile", true, "Enable changes to this item.").Value;
            Bandolier.enabled = Config.Bind("05 - Uncommon Items", "Bandolier", true, "Enable changes to this item.").Value;
            Berzerker.enabled = Config.Bind("05 - Uncommon Items", "Berzerkers Pauldron", true, "Enable changes to this item.").Value;
            Chronobauble.enabled = Config.Bind("05 - Uncommon Items", "Chronobauble", true, "Enable changes to this item.").Value;
            ElementalBands.enabled = Config.Bind("05 - Uncommon Items", "Runalds and Kjaros Bands", true, "Enable changes to this item.").Value;
            Guillotine.enabled = Config.Bind("05 - Uncommon Items", "Old Guillotine", true, "Enable changes to this item.").Value;
            HarvesterScythe.enabled = Config.Bind("05 - Uncommon Items", "Harvesters Scythe", true, "Enable changes to this item.").Value;
            Infusion.enabled = Config.Bind("05 - Uncommon Items", "Infusion", true, "Enable changes to this item.").Value;
            LeechingSeed.enabled = Config.Bind("05 - Uncommon Items", "Leeching Seed", true, "Enable changes to this item.").Value;
            Predatory.enabled = Config.Bind("05 - Uncommon Items", "Predatory Instincts", true, "Enable changes to this item.").Value;
            Razorwire.enabled = Config.Bind("05 - Uncommon Items", "Razorwire", true, "Enable changes to this item.").Value;
            RoseBuckler.enabled = Config.Bind("05 - Uncommon Items", "Rose Buckler", true, "Enable changes to this item.").Value;
            SquidPolyp.enabled = Config.Bind("05 - Uncommon Items", "Squid Polyp", true, "Enable changes to this item.").Value;
            Stealthkit.enabled = Config.Bind("05 - Uncommon Items", "Old War Stealthkit", true, "Enable changes to this item.").Value;
            Ukulele.enabled = Config.Bind("05 - Uncommon Items", "Ukulele", true, "Enable changes to this item.").Value;
            WarHorn.enabled = Config.Bind("05 - Uncommon Items", "War Horn", true, "Enable changes to this item.").Value;
            WillOWisp.enabled = Config.Bind("05 - Uncommon Items", "Will-o-the-Wisp", true, "Enable changes to this item.").Value;

            FrostRelic.enabled = Config.Bind("06 - Legendary Items", "Frost Relic", true, "Enable changes to this item.").Value;
            FrostRelic.removeFOV = Config.Bind("06 - Legendary Items", "Frost Relic - Disable FOV Modifier", true, "Disables FOV modifier.").Value;
            FrostRelic.removeBubble = Config.Bind("06 - Legendary Items", "Frost Relic - Disable Bubble", true, "Disables bubble visuals.").Value;
            HeadHunter.enabled = Config.Bind("06 - Legendary Items", "Wake of Vultures", true, "Enable changes to this item.").Value;
            Headstompers.enabled = Config.Bind("06 - Legendary Items", "H3AD-ST", true, "Enable changes to this item.").Value;
            Tesla.enabled = Config.Bind("06 - Legendary Items", "Unstable Tesla Coil", true, "Enable changes to this item.").Value;
            CeremonialDagger.enabled = Config.Bind("06 - Legendary Items", "Ceremonial Dagger", true, "Enable changes to this item.").Value;
            MeatHook.enabled = Config.Bind("06 - Legendary Items", "Sentient Meat Hook", true, "Enable changes to this item.").Value;

            MoltenPerf.enabled = Config.Bind("07 - Boss Items", "Molten Perforator", true, "Enable changes to this item.").Value;
            ChargedPerf.enabled = Config.Bind("07 - Boss Items", "Charged Perforator", true, "Enable changes to this item.").Value;
            Disciple.enabled = Config.Bind("07 - Boss Items", "Charged Perforator", true, "Enable changes to this item.").Value;
            QueensGland.enabled = Config.Bind("07 - Boss Items", "Queens Gland", true, "Enable changes to this item.").Value;
            Shatterspleen.enabled = Config.Bind("07 - Boss Items", "Shatterspleen", true, "Enable changes to this item.").Value;

            ShapedGlass.enabled = Config.Bind("08 - Lunar Items", "Shaped Glass", true, "Enable changes to this item.").Value;
            BrittleCrown.enabled = Config.Bind("08 - Lunar Items", "Brittle Crown", true, "Enable changes to this item.").Value;
            Transcendence.enabled = Config.Bind("08 - Lunar Items", "Transcendence", true, "Enable changes to this item.").Value;
            Meteorite.enabled = Config.Bind("08 - Lunar Items", "Glowing Meteorite", true, "Enable changes to this item.").Value;

            Backup.enabled = Config.Bind("09 - Equipment", "The Back-Up", true, "Enable changes to this item.").Value;
            BFG.enabled = Config.Bind("09 - Equipment", "Preon Accumulator", true, "Enable changes to this item.").Value;
            Capacitor.enabled = Config.Bind("09 - Equipment", "Royal Capacitor", true, "Enable changes to this item.").Value;
            Chrysalis.enabled = Config.Bind("09 - Equipment", "Milky Chrysalis", true, "Enable changes to this item.").Value;
            CritHud.enabled = Config.Bind("09 - Equipment", "Ocular HUD", true, "Enable changes to this item.").Value;
            VolcanicEgg.enabled = Config.Bind("09 - Equipment", "Volcanic Egg", true, "Enable changes to this item.").Value;

            //Leave slot 10 for void items

            Vagrant.enabled = Config.Bind("Monsters", "Wandering Vagrant", true, "Enable changes to this monster.").Value;

            CaptainCore.enabled = Config.Bind("Survivors: Captain", "Enable Changes", true, "Enable changes to this survivor.").Value;
            CaptainOrbitalHiddenRealms.enabled = Config.Bind("Survivors: Captain", "Hidden Realm Orbital Skills", true, "Allow Orbital skills in Hiden Realms.").Value;
            Microbots.enabled = Config.Bind("Survivors: Captain", "Defensive Microbots Nerf", true, "Defensive Microbots no longer deletes stationary projectiles like gas clouds and Void Reaver mortars.").Value;
            Shock.enabled = Config.Bind("Survivors: Captain", "No Shock Interrupt", true, "Shock is no longer interrupted by damage.").Value;

            Bandit2Core.enabled = Config.Bind("Survivors: Bandit", "Enable Changes", true, "Enable changes to this survivor.").Value;
            BanditSpecialGracePeriod.enabled = Config.Bind("Survivors: Bandit", "Special Grace Period", true, "Special On-kill effects can trigger if an enemy dies shortly after being hit.").Value;
            BanditSpecialGracePeriod.duration = Config.Bind("Survivors: Bandit", "Special Grace Period Duration", 1.2f, "Length in seconds of Special Grace Period.").Value;
            Bandit2Core.enablePassiveSkillChanges = Config.Bind("Survivors: Bandit", "Enable Passive Skill Changes", true, "Enable passive skill changes for this survivor.").Value;
            Bandit2Core.enablePrimarySkillChanges = Config.Bind("Survivors: Bandit", "Enable Primary Skill Changes", true, "Enable primary skill changes for this survivor.").Value;
            Bandit2Core.enableSecondarySkillChanges = Config.Bind("Survivors: Bandit", "Enable Secondary Skill Changes", true, "Enable secondary skill changes for this survivor.").Value;
            Bandit2Core.enableUtilitySkillChanges = Config.Bind("Survivors: Bandit", "Enable Utility Skill Changes", true, "Enable utility skill changes for this survivor.").Value;
            Bandit2Core.enableSpecialSkillChanges = Config.Bind("Survivors: Bandit", "Enable Special Skill Changes", true, "Enable special skill changes for this survivor.").Value;
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
