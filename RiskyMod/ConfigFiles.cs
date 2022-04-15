using BepInEx.Configuration;
using RiskyMod.Survivors.Commando;
using RiskyMod.Survivors.Croco;
using RiskyMod.Survivors.Engi;
using RiskyMod.Survivors.Huntress;
using RiskyMod.Survivors.Loader;
using RiskyMod.Survivors.Mage;
using RiskyMod.Survivors.Merc;
using RiskyMod.Survivors.Toolbot;
using RiskyMod.Survivors.Treebot;
using RiskyMod.Survivors.Captain;
using RiskyMod.Survivors.Bandit2;
using System.IO;
using RiskyMod.Survivors.DLC1.Railgunner;
using RiskyMod.Survivors.DLC1.VoidFiend;
using RiskyMod.Survivors;
using UnityEngine;
using RiskyMod.Items.Uncommon;
using RiskyMod.Items.Equipment;
using RiskyMod.Items.Lunar;
using RiskyMod.Items.DLC1.Legendary;
using RiskyMod.Items.DLC1.Void;
using RiskyMod.Items.Boss;
using RoR2;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Common;
using RiskyMod.Items.DLC1.Common;
using RiskyMod.Items.DLC1.Uncommon;
using RiskyMod.Fixes;
using RiskyMod.Enemies.Mobs;
using RiskyMod.Enemies.Mobs.Lunar;
using RiskyMod.Enemies.DLC1;
using RiskyMod.Enemies.Bosses;
using RiskyMod.Allies;
using RiskyMod.Items;
using RiskyMod.Enemies;
using RiskyMod.Moon;
using RiskyMod.VoidLocus;
using RiskyMod.Tweaks.Holdouts;
using RiskyMod.Tweaks.Artifacts;
using RiskyMod.Tweaks;
using RiskyMod.Tweaks.CharacterMechanics;
using RiskyMod.Tweaks.RunScaling;
using RiskyMod.VoidFields;

namespace RiskyMod
{
    public static class ConfigFiles
    {
        public static ConfigFile ItemCfg;
        public static ConfigFile SurvivorCfg;
        public static ConfigFile MonsterCfg;

        public static ConfigFile GeneralCfg;

        public static string ConfigFolderPath { get => System.IO.Path.Combine(BepInEx.Paths.ConfigPath, RiskyMod.pluginInfo.Metadata.GUID); }

        private const string coreModuleString = "00. Core Modules";
        private const string gameMechString = "01. Game Mechanics";
        private const string scalingString = "02. Run Scaling";
        private const string interactString = "03. Interactables";
        private const string allyString = "04. Allies";
        private const string artifactString = "05. Artifacts";
        private const string voidFieldsString = "06. Void Fields";
        private const string moonString = "07. Moon";
        private const string voidLocusString = "08. Void Locus";
        private const string miscString = "99. Misc Tweaks";

        private const string uncommonString = "Items - Uncommon";
        private const string commonString = "Items - Common";
        private const string legendaryString = "Items - Legendary";
        private const string voidString = "Items - Void";
        private const string bossString = "Items - Boss";
        private const string lunarString = "Items - Lunar";
        private const string equipmentString = "Items - Equipment";
        private const string itemConfigDescString = "Enable changes to this item.";


        private const string fireSelectString = "Firemode Selection (Client-Side)";
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

        private const string monsterString = "Monsters";

        public static void Init()
        {
            ConfigGeneral();
            ConfigItems();
            ConfigSurvivors();
            ConfigMonsters();
        }

        private static void ConfigGeneral()
        {
            GeneralCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_General.cfg"), true);

            //Core Modules
            AlliesCore.enabled = GeneralCfg.Bind(coreModuleString, "Ally Changes", true, "Enable drone and ally changes.").Value;
            ItemsCore.enabled = GeneralCfg.Bind(coreModuleString, "Item Changes", true, "Enable item changes.").Value;
            SurvivorsCore.enabled = GeneralCfg.Bind(coreModuleString, "Survivor Changes", true, "Enable survivor changes.").Value;
            EnemiesCore.modifyEnemies = GeneralCfg.Bind(coreModuleString, "Monster Changes", true, "Enable enemy changes.").Value;
            MoonCore.enabled = GeneralCfg.Bind(coreModuleString, "Moon Changes", true, "Enable Moon changes.").Value;
            VoidLocusCore.enabled = GeneralCfg.Bind(coreModuleString, "Void Locus Changes", true, "Enable Void Locus changes.").Value;
            VoidFieldsCore.enabled = GeneralCfg.Bind(coreModuleString, "Void Fields Changes", true, "Enable Void Fields changes.").Value;

            //Game Mechanics
            RiskyMod.disableProcChains = GeneralCfg.Bind(gameMechString, "Disable Proc Chains", true, "Remove the proc coefficient on most item effects.").Value;
            ShieldGating.enabled = GeneralCfg.Bind(gameMechString, "Shield Gating", true, "Shields gate against HP damage.").Value;
            TrueOSP.enabled = GeneralCfg.Bind(gameMechString, "True OSP", true, "Makes OSP work against multihits.").Value;
            AIBlacklistItems.enabled = GeneralCfg.Bind(gameMechString, "Expanded AI Blacklist", true, "Adds extra items to the AI Blacklist by default.").Value;
            BarrierDecay.enabled = GeneralCfg.Bind(gameMechString, "Barrier Decay", true, "Barrier decays slower at low barrier values.").Value;
            TeleExpandOnBossKill.enabled = GeneralCfg.Bind(gameMechString, "Tele Expand on Boss Kill", true, "Teleporter expands to cover the whole map when the boss is killed.").Value;
            SmallHoldoutCharging.enabled = GeneralCfg.Bind(gameMechString, "Small Holdout Charging", true, "Void/Moon Holdouts charge at max speed as long as 1 player is charging.").Value;

            //Run Scaling
            CombatDirectorMultiplier.multiplier = GeneralCfg.Bind(scalingString, "Combat Director Credit Multiplier", 1.2f, "Multiply Combat Director credits by this amount. Set to 1 to disable").Value;
            CombatDirectorMultiplier.enabled = CombatDirectorMultiplier.multiplier != 1f;
            MonsterGoldRewards.enabled = GeneralCfg.Bind(scalingString, "Scaled Monster Gold Rewards", true, "Monsters gold rewards are scaled off of the initial price of chests on the stage.").Value;
            LinearScaling.enabled = GeneralCfg.Bind(scalingString, "Experimental: Linear Difficulty Scaling", false, "Makes difficulty scaling linear like in RoR1. Was used in older versions of the mod but ended up getting scrapped since it ended up underscaling enemies compared to Vanilla when looping.").Value;
            NoLevelupHeal.enabled = GeneralCfg.Bind(scalingString, "No Levelup Heal", true, "Monsters don't gain HP when leveling up.").Value;
            RemoveLevelCap.enabled = GeneralCfg.Bind(scalingString, "Increase Monster Level Cap", true, "Increases Monster Level Cap.").Value;
            RemoveLevelCap.maxLevel = GeneralCfg.Bind(scalingString, "Increase Monster Level Cap - Max Level", 9999f, "Maximum monster level if Increase Monster Level Cap is enabled.").Value;
            SceneDirectorMonsterRewards.enabled = GeneralCfg.Bind(scalingString, "SceneDirector Monster Rewards", true, "Monsters that spawn with the map now give the same rewards as teleporter monsters.").Value;

            //Allies
            AllyScaling.normalizeDroneDamage = GeneralCfg.Bind(allyString, "Normalize Drone Damage", true, "Normalize drone damage stats so that they perform the same when using Spare Drone Parts.").Value;
            AlliesCore.nerfDroneParts = GeneralCfg.Bind(allyString, "Spare Drone Parts Changes", true, "Reduce the damage of Spare Drone Parts.").Value;
            AllyScaling.noVoidDeath = GeneralCfg.Bind(allyString, "No Void Death", true, "Allies are immune to Void implosions.").Value;
            NoVoidDamage.enabled = GeneralCfg.Bind(allyString, "No Void Damage", true, "Allies take no damage from Void fog.").Value;
            AllyScaling.noOverheat = GeneralCfg.Bind(allyString, "No Overheat", true, "Allies are immune to Grandparent Overheat.").Value;
            SuperAttackResist.enabled = GeneralCfg.Bind(allyString, "Superattack Resistance", true, "Allies take less damage from superattacks like Vagrant Novas.").Value;

            //Interactables
            ShrineSpawnRate.enabled = GeneralCfg.Bind(interactString, "Mountain/Combat Shrine Playercount Scaling", true, "Mountain/Combat Shrine Director Credit cost scales with playercount.").Value;
            ShrineCombatItems.enabled = GeneralCfg.Bind(interactString, "Shrine of Combat Drops Items", true, "Shrine of Combat drops items for the team on completion.").Value;
            BloodShrineMinReward.enabled = GeneralCfg.Bind(interactString, "Shrine of Blood Minimum Reward", true, "Shrine of Blood always gives at least enough money to buy a small chest.").Value;
            VoidSeedLimit.seedLimit = GeneralCfg.Bind(interactString, "Void Seed Limit", 1, "Limit how many Void Seeds can spawn on 1 stage. Set to negative to disable.").Value;
            VoidSeedLimit.enabled = VoidSeedLimit.seedLimit >= 0;

            //Artifacts
            VengeancePercentHeal.enabled = GeneralCfg.Bind(artifactString, "Reduce Vengeance Healing", true, "Vengeance Doppelgangers receive reduced healing from percent-based healing effects.").Value;
            EnigmaBlacklist.enabled = GeneralCfg.Bind(artifactString, "Enigma Blacklist", true, "Blacklist Lunars and Recycler from the Artifact of Enigma.").Value;

            //Void Fields
            VoidFields.ModifyHoldout.enabled = GeneralCfg.Bind(voidFieldsString, "Modify Holdout Zone", true, "Increase radius and reduces charge duration.").Value;

            //Moon
            Moon.ModifyHoldout.enabled = GeneralCfg.Bind(moonString, "Modify Holdout Zone", true, "Increase radius and reduces charge duration.").Value;
            LessPillars.enabled = GeneralCfg.Bind(moonString, "Reduce Pillar Count", true, "Reduce the amount of pillars required to activate the jump pads.").Value;
            Moon.PillarsDropItems.enabled = GeneralCfg.Bind(moonString, "Pillars Drop Items", true, "Pillars drop items for the team when completed.").Value;
            Moon.PillarsDropItems.whiteChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Common Chance", 50f, "Chance for Pillars to drop Common Items.").Value;
            Moon.PillarsDropItems.greenChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Uncommon Chance", 40f, "Chance for Pillars to drop Uncommon Items.").Value;
            Moon.PillarsDropItems.redChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Legendary Chance", 10f, "Chance for Pillars to drop Legendary Items.").Value;
            Moon.PillarsDropItems.pearlOverwriteChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Pearl Overwrite Chance", 15f, "Chance for nonlegendary Pillar drops to be overwritten with a Pearl.").Value;
            Moon.PillarsDropItems.lunarOverwriteChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Lunar Overwrite Chance", 0f, "Chance for nonlegendary Pillar drops to be overwritten with a random Lunar item.").Value;
            Moon.PillarsDropItems.noOverwriteChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - No Overwrite Chance", 85f, "Chance for nonlegendary Pillar drops to not be overwritten.").Value;

            //Void Locus
            RemoveFog.enabled = GeneralCfg.Bind(voidLocusString, "Remove Fog", true, "Removes Void Fog from the map.").Value;
            VoidLocus.ModifyHoldout.enabled = GeneralCfg.Bind(voidLocusString, "Modify Holdout Zone", true, "Increase radius and reduces charge duration.").Value;
            VoidLocus.PillarsDropItems.enabled = GeneralCfg.Bind(voidLocusString, "Signals Drop Items", true, "Pillars drop items for the team when completed.").Value;
            VoidLocus.PillarsDropItems.whiteChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Common Chance", 50f, "Chance for Signals to drop Common Items.").Value;
            VoidLocus.PillarsDropItems.greenChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Uncommon Chance", 40f, "Chance for Signals to drop Uncommon Items.").Value;
            VoidLocus.PillarsDropItems.redChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Legendary Chance", 10f, "Chance for Signals to drop Legendary Items.").Value;
            VoidLocus.PillarsDropItems.pearlOverwriteChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Pearl Overwrite Chance", 0f, "Chance for nonlegendary Pillar drops to be overwritten with a Pearl.").Value;
            VoidLocus.PillarsDropItems.lunarOverwriteChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Lunar Overwrite Chance", 0f, "Chance for nonlegendary Pillar drops to be overwritten with a random Lunar item.").Value;
            VoidLocus.PillarsDropItems.noOverwriteChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - No Overwrite Chance", 100f, "Chance for nonlegendary Pillar drops to not be overwritten.").Value;

            //Misc
            FixSlayer.enabled = GeneralCfg.Bind(miscString, "Fix Slayer Procs", true, "Bandit/Acrid bonus damage to low hp effect now applies to procs.").Value;
            CloakBuff.enabled = GeneralCfg.Bind(miscString, "Cloak Buff", true, "Increases delay between position updates while cloaked.").Value;
            Shock.enabled = GeneralCfg.Bind(miscString, "No Shock Interrupt", true, "Shock is no longer interrupted by damage.").Value;
            FreezeChampionExecute.enabled = GeneralCfg.Bind(miscString, "Freeze Executes Bosses", true, "Freeze counts as a debuff and can execute bosses at 15% HP.").Value;
            NerfVoidtouched.enabled = GeneralCfg.Bind(miscString, "Nerf Voidtouched", true, "Replaces Voidtouched Collapse with Nullify.").Value;
            PlayerControlledMonsters.enabled = GeneralCfg.Bind(miscString, "Player-Controlled Monster Regen", true, "Gives players health regen + armor when playing as monsters via mods.").Value;
            SlowDownProjectilesModifyDamage.enabled = GeneralCfg.Bind(miscString, "Slowed Projectiles Deal Less Damage", true, "Projectiles slowed by Railgunners Polar Field Device and similar skills deal less damage.").Value;
        }

        private static void ConfigItems()
        {
            ItemCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Items.cfg"), true);
            ConfigCommonItems();
            ConfigUncommonItems();
            ConfigLegendaryItems();
            ConfigBossItems();
            ConfigLunars();
            ConfigEquipment();
            ConfigVoidItems();
        }

        private static void ConfigCommonItems()
        {
            BisonSteak.enabled = ItemCfg.Bind(commonString, "Bison Steak", true, itemConfigDescString).Value;
            Bungus.enabled = ItemCfg.Bind(commonString, "Bustling Fungus", true, itemConfigDescString).Value;
            CautiousSlug.enabled = ItemCfg.Bind(commonString, "Cautious Slug", true, itemConfigDescString).Value;
            Crowbar.enabled = ItemCfg.Bind(commonString, "Crowbar", true, itemConfigDescString).Value;
            DelicateWatch.enabled = ItemCfg.Bind(commonString, "Delicate Watch", true, itemConfigDescString).Value;
            Fireworks.enabled = ItemCfg.Bind(commonString, "Fireworks", true, itemConfigDescString).Value;
            Fireworks.maxRockets = ItemCfg.Bind(commonString, "Fireworks - Max Rockets", 32, "Max rockets that can spawn from each use. Going above this value raises rocket damage instead.").Value;
            Gasoline.enabled = ItemCfg.Bind(commonString, "Gasoline", true, itemConfigDescString).Value;
            CritGlasses.enabled = ItemCfg.Bind(commonString, "Lensmakers Glasses", true, itemConfigDescString).Value;
            MonsterTooth.enabled = ItemCfg.Bind(commonString, "Monster Tooth", true, itemConfigDescString).Value;
            StickyBomb.enabled = ItemCfg.Bind(commonString, "Stickybomb", true, itemConfigDescString).Value;
            TougherTimes.enabled = ItemCfg.Bind(commonString, "Tougher Times", true, itemConfigDescString).Value;
            Warbanner.enabled = ItemCfg.Bind(commonString, "Warbanner", true, itemConfigDescString).Value;
        }

        private static void ConfigUncommonItems()
        {
            AtG.enabled = ItemCfg.Bind(uncommonString, "AtG Missile", true, itemConfigDescString).Value;
            AtG.useOrb = ItemCfg.Bind(uncommonString, "AtG Missile - Use OrbAttack", true, "Makes AtG missiles behave like Plasma Shrimp when fired off of weak attacks to improve performance.").Value;
            AtG.alwaysOrb = ItemCfg.Bind(uncommonString, "AtG Missile - Always Use OrbAttack", false, "AtG Missiles always fire orbattacks regardless of the hit's strength.").Value;
            Bandolier.enabled = ItemCfg.Bind(uncommonString, "Bandolier", true, itemConfigDescString).Value;
            Berzerker.enabled = ItemCfg.Bind(uncommonString, "Berzerkers Pauldron", true, itemConfigDescString).Value;
            Chronobauble.enabled = ItemCfg.Bind(uncommonString, "Chronobauble", true, itemConfigDescString).Value;
            ElementalBands.enabled = ItemCfg.Bind(uncommonString, "Runalds and Kjaros Bands", true, itemConfigDescString).Value;
            Harpoon.enabled = ItemCfg.Bind(uncommonString, "Hunters Harpoon", true, itemConfigDescString).Value;
            HarvesterScythe.enabled = ItemCfg.Bind(uncommonString, "Harvesters Scythe", true, itemConfigDescString).Value;
            Infusion.enabled = ItemCfg.Bind(uncommonString, "Infusion", true, itemConfigDescString).Value;
            LeechingSeed.enabled = ItemCfg.Bind(uncommonString, "Leeching Seed", true, itemConfigDescString).Value;
            Daisy.enabled = ItemCfg.Bind(uncommonString, "Lepton Daisy", true, itemConfigDescString).Value;
            Guillotine.enabled = ItemCfg.Bind(uncommonString, "Old Guillotine", true, itemConfigDescString).Value;
            Guillotine.reduceVFX = ItemCfg.Bind(uncommonString, "Old Guillotine - Reduce VFX", true, "Reduce how often this item's VFX shows up.").Value;
            Razorwire.enabled = ItemCfg.Bind(uncommonString, "Razorwire", true, itemConfigDescString).Value;
            RedWhip.enabled = ItemCfg.Bind(uncommonString, "Red Whip", true, itemConfigDescString).Value;
            RoseBuckler.enabled = ItemCfg.Bind(uncommonString, "Rose Buckler", true, itemConfigDescString).Value;
            SquidPolyp.enabled = ItemCfg.Bind(uncommonString, "Squid Polyp", true, itemConfigDescString).Value;
            SquidPolyp.scaleCount = ItemCfg.Bind(uncommonString, "Squid Polyp - Stacks Increase Max Squids", false, "Extra stacks allow for more squids to spawn. Will lag in MP.").Value;
            Stealthkit.enabled = ItemCfg.Bind(uncommonString, "Old War Stealthkit", true, itemConfigDescString).Value;
            Ukulele.enabled = ItemCfg.Bind(uncommonString, "Ukulele", true, itemConfigDescString).Value;
            WarHorn.enabled = ItemCfg.Bind(uncommonString, "War Horn", true, itemConfigDescString).Value;
            WillOWisp.enabled = ItemCfg.Bind(uncommonString, "Will-o-the-Wisp", true, itemConfigDescString).Value;
        }

        private static void ConfigLegendaryItems()
        {
            Behemoth.enabled = ItemCfg.Bind(legendaryString, "Brilliant Behemoth", true, itemConfigDescString).Value;
            BottledChaos.enabled = ItemCfg.Bind(legendaryString, "Bottled Chaos", true, itemConfigDescString).Value;
            Brainstalks.enabled = ItemCfg.Bind(legendaryString, "Brainstalks", true, itemConfigDescString).Value;
            CeremonialDagger.enabled = ItemCfg.Bind(legendaryString, "Ceremonial Dagger", true, itemConfigDescString).Value;
            Clover.enabled = ItemCfg.Bind(legendaryString, "57 Leaf Clover", false, "Caps how high the Luck stat can go.").Value;
            Clover.luckCap = ItemCfg.Bind(legendaryString, "57 Leaf Clove - Max Luck", 1f, "Maximum Luck value players can reach. Extra Luck is converted to stat boosts.").Value;
            FrostRelic.enabled = ItemCfg.Bind(legendaryString, "Frost Relic", true, itemConfigDescString).Value;
            FrostRelic.removeFOV = ItemCfg.Bind(legendaryString, "Frost Relic - Disable FOV Modifier", true, "Disables FOV modifier.").Value;
            FrostRelic.removeBubble = ItemCfg.Bind(legendaryString, "Frost Relic - Disable Bubble", true, "Disables bubble visuals.").Value;
            HappiestMask.enabled = ItemCfg.Bind(legendaryString, "Happiest Mask", true, itemConfigDescString).Value;
            HappiestMask.scaleCount = ItemCfg.Bind(legendaryString, "Happiest Mask - Stacks Increase Max Ghosts", false, "Extra stacks allow for more ghosts to spawn. Will lag in MP.").Value;
            HappiestMask.noGhostLimit = ItemCfg.Bind(legendaryString, "Happiest Mask - Remove Ghost Limit", false, "Removes the ghost limit at all times. Definitely will lag.").Value;
            HeadHunter.enabled = ItemCfg.Bind(legendaryString, "Wake of Vultures", true, itemConfigDescString).Value;
            HeadHunter.perfectedTweak = ItemCfg.Bind(legendaryString, "Wake of Vultures - Perfected Tweak", true, "Perfected Affix gained via Wake of Vultures will not force your health pool to bec").Value;
            Headstompers.enabled = ItemCfg.Bind(legendaryString, "H3AD-ST", true, itemConfigDescString).Value;
            LaserTurbine.enabled = ItemCfg.Bind(legendaryString, "Resonance Disc", true, itemConfigDescString).Value;
            MeatHook.enabled = ItemCfg.Bind(legendaryString, "Sentient Meat Hook", true, itemConfigDescString).Value;
            Raincoat.enabled = ItemCfg.Bind(legendaryString, "Bens Raincoat", true, itemConfigDescString).Value;
            Tesla.enabled = ItemCfg.Bind(legendaryString, "Unstable Tesla Coil", true, itemConfigDescString).Value;

            //Turns out SS2's Gadget increased initial hit damage by 50%, which lead to a 3x total damage multiplier, which is what this item does already.
            //LaserScope.enabled = ItemCfg.Bind(legendaryString, "Laser Scope", true, itemConfigDescString).Value;
            LaserScope.enabled = false;

        }

        private static void ConfigVoidItems()
        {
            Dungus.enabled = ItemCfg.Bind(voidString, "Weeping Fungus", true, itemConfigDescString).Value;

            //Doesn't seem strong enough to actually need a nerf. Opal is a bigger culprit.
            //SaferSpaces.enabled = ItemCfg.Bind(voidString, "Safer Spaces", true, itemConfigDescString).Value;
            SaferSpaces.enabled = false;

            PlasmaShrimp.enabled = ItemCfg.Bind(voidString, "Plasma Shrimp", true, itemConfigDescString).Value;
            VoidWisp.enabled = ItemCfg.Bind(voidString, "Voidsent Flame", true, itemConfigDescString).Value;
            Polylute.enabled = ItemCfg.Bind(voidString, "Polylute", true, itemConfigDescString).Value;
            VoidRing.enabled = ItemCfg.Bind(voidString, "Singularity Band", true, itemConfigDescString).Value;
        }

        private static void ConfigBossItems()
        {
            ChargedPerf.enabled = ItemCfg.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            Disciple.enabled = ItemCfg.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            Knurl.enabled = ItemCfg.Bind(bossString, "Titanic Knurl", true, itemConfigDescString).Value;
            MoltenPerf.enabled = ItemCfg.Bind(bossString, "Molten Perforator", true, itemConfigDescString).Value;
            QueensGland.enabled = ItemCfg.Bind(bossString, "Queens Gland", true, itemConfigDescString).Value;
            Shatterspleen.enabled = ItemCfg.Bind(bossString, "Shatterspleen", true, itemConfigDescString).Value;
        }

        private static void ConfigLunars()
        {
            //Currently split into a separate mod. Will likely remain that way since 
            //Gesture.enabled = ItemCfg.Bind(lunarString, "Gesture of the Drowned", true, itemConfigDescString).Value;
            Gesture.enabled = false;

            BrittleCrown.enabled = ItemCfg.Bind(lunarString, "Brittle Crown", true, itemConfigDescString).Value;
            Meteorite.enabled = ItemCfg.Bind(lunarString, "Glowing Meteorite", true, itemConfigDescString).Value;
            ShapedGlass.enabled = ItemCfg.Bind(lunarString, "Shaped Glass", true, itemConfigDescString).Value;
            Transcendence.enabled = ItemCfg.Bind(lunarString, "Transcendence", true, itemConfigDescString).Value;
        }

        private static void ConfigEquipment()
        {
            Backup.enabled = ItemCfg.Bind(equipmentString, "The Back-Up", true, itemConfigDescString).Value;
            Backup.ignoreTeamLimit = !ItemCfg.Bind(equipmentString, "The Back-Up: Limit Drones", false, "Back-Up drones count towards the ally cap.").Value;
            BackupTracker.maxCount = ItemCfg.Bind(equipmentString, "The Back-Up: Max Drones", 4, "Max active Backup Drones.").Value;
            BFG.enabled = ItemCfg.Bind(equipmentString, "Preon Accumulator", true, itemConfigDescString).Value;
            Capacitor.enabled = ItemCfg.Bind(equipmentString, "Royal Capacitor", true, itemConfigDescString).Value;
            Chrysalis.enabled = ItemCfg.Bind(equipmentString, "Milky Chrysalis", true, itemConfigDescString).Value;
            CritHud.enabled = ItemCfg.Bind(equipmentString, "Ocular HUD", true, itemConfigDescString).Value;
            Fruit.enabled = ItemCfg.Bind(equipmentString, "Foreign Fruit", true, itemConfigDescString).Value;
            SuperLeech.enabled = ItemCfg.Bind(equipmentString, "Super Massive Leech", true, itemConfigDescString).Value;
            VolcanicEgg.enabled = ItemCfg.Bind(equipmentString, "Volcanic Egg", true, itemConfigDescString).Value;
        }

        private static void ConfigFireSelect()
        {
            FireSelect.scrollSelection = SurvivorCfg.Bind(fireSelectString, "Use Scrollwheel", true, "Mouse Scroll Wheel changes firemode").Value;
            FireSelect.swapButton = SurvivorCfg.Bind(fireSelectString, "Next Firemode", KeyCode.None, "Button to swap to the next firemode.").Value;
            FireSelect.prevButton = SurvivorCfg.Bind(fireSelectString, "Previous Firemode", KeyCode.None, "Button to swap to the previous firemode.").Value;
            CaptainFireModes.enabled = SurvivorCfg.Bind(fireSelectString, "Captain: Enable Fire Select", false, "Enable firemode selection for Captain's shotgun (requires primary changes to be enabled).").Value;
            CaptainFireModes.defaultButton = SurvivorCfg.Bind(fireSelectString, "Captain: Swap to Default", KeyCode.None, "Button to swap to the Default firemode.").Value;
            CaptainFireModes.autoButton = SurvivorCfg.Bind(fireSelectString, "Captain: Swap to Auto", KeyCode.None, "Button to swap to the Auto firemode.").Value;
            CaptainFireModes.chargeButton = SurvivorCfg.Bind(fireSelectString, "Captain: Swap to Charged", KeyCode.None, "Button to swap to the Charged firemode.").Value;
            BanditFireModes.enabled = SurvivorCfg.Bind(fireSelectString, "Bandit: Enable Fire Select", false, "Enable firemode selection for Bandit's primaries (requires primary changes to be enabled).").Value;
            BanditFireModes.defaultButton = SurvivorCfg.Bind(fireSelectString, "Bandit: Swap to Default", KeyCode.None, "Button to swap to the Default firemode.").Value;
            BanditFireModes.spamButton = SurvivorCfg.Bind(fireSelectString, "Bandit: Swap to Spam", KeyCode.None, "Button to swap to the Spam firemode.").Value;
        }

        private static void ConfigSurvivors()
        {
            SurvivorCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Survivors.cfg"), true);
            ConfigFireSelect();
            CommandoCore.enabled = SurvivorCfg.Bind(commandoString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            CommandoCore.fixPrimaryFireRate = SurvivorCfg.Bind(commandoString, "Double Tap - Fix Scaling", true, "Fixes Double Tap having a low attack speed cap.").Value;
            CommandoCore.phaseRoundChanges = SurvivorCfg.Bind(commandoString, "Phase Round Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.rollChanges = SurvivorCfg.Bind(commandoString, "Tactical Dive Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.suppressiveChanges = SurvivorCfg.Bind(commandoString, "Suppressive Fire Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.grenadeChanges = SurvivorCfg.Bind(commandoString, "Frag Grenade Changes", true, "Enable changes to this skill.").Value;

            HuntressCore.enabled = SurvivorCfg.Bind(huntressString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            HuntressCore.HuntressTargetingMode = SurvivorCfg.Bind(huntressString, "Targeting Mode (Client-Side)", BullseyeSearch.SortMode.Angle, "How Huntress's target prioritization works.").Value;
            HuntressCore.increaseAngle = SurvivorCfg.Bind(huntressString, "Increase Targeting Angle", true, "Increase max targeting angle.").Value;
            HuntressCore.strafeChanges = SurvivorCfg.Bind(huntressString, "Strafe Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.flurryChanges = SurvivorCfg.Bind(huntressString, "Flurry Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.laserGlaiveChanges = SurvivorCfg.Bind(huntressString, "Laser Glaive Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.blinkChanges = SurvivorCfg.Bind(huntressString, "Blink Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.arrowRainChanges = SurvivorCfg.Bind(huntressString, "Arrow Rain Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.ballistaChanges = SurvivorCfg.Bind(huntressString, "Ballista Changes", true, "Enable changes to this skill.").Value;

            ToolbotCore.enabled = SurvivorCfg.Bind(toolbotString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            ToolbotCore.enableNailgunChanges = SurvivorCfg.Bind(toolbotString, "Nailgun Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableRebarChanges = SurvivorCfg.Bind(toolbotString, "Rebar Puncher Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableScrapChanges = SurvivorCfg.Bind(toolbotString, "Scrap Launcher Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableSawChanges = SurvivorCfg.Bind(toolbotString, "Power Saw Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableSecondarySkillChanges = SurvivorCfg.Bind(toolbotString, "Blast Canister Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableRetoolChanges = SurvivorCfg.Bind(toolbotString, "Retool Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enablePowerModeChanges = SurvivorCfg.Bind(toolbotString, "Power Mode Changes", true, "Enable changes to this skill.").Value;

            EngiCore.enabled = SurvivorCfg.Bind(engiString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            PressureMines.enabled = SurvivorCfg.Bind(engiString, "Pressure Mine Changes", true, "Pressure Mines only detonate when fully armed.").Value;
            TurretChanges.turretChanges = SurvivorCfg.Bind(engiString, "Stationary Turret Changes", true, "Enable changes to Stationary Turrets.").Value;
            TurretChanges.mobileTurretChanges = SurvivorCfg.Bind(engiString, "Mobile Turret Changes", true, "Enable changes to Mobile Turrets.").Value;

            MageCore.enabled = SurvivorCfg.Bind(mageString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            MageCore.modifyFireBolt = SurvivorCfg.Bind(mageString, "Fire Bolt Changes", true, "Enable changes to this skill.").Value;
            MageCore.modifyPlasmaBolt = SurvivorCfg.Bind(mageString, "Plasma Bolt Changes", true, "Enable changes to this skill.").Value;
            MageCore.m2RemoveNanobombGravity = SurvivorCfg.Bind(mageString, "Nanobomb - Remove Gravity", true, "Removes projectile drop from Nanobomb so it behaves like it did pre-1.0 update.").Value;
            MageCore.flamethrowerSprintCancel = SurvivorCfg.Bind(mageString, "Flamethrower - Sprint Cancel", true, "Sprinting cancels Flamethrower.").Value;
            MageCore.ionSurgeMovementScaling = SurvivorCfg.Bind(mageString, "Vanilla Ion Surge - Movement Scaling", false, "Ion Surge jump height scales with movement speed.").Value;
            MageCore.ionSurgeShock = SurvivorCfg.Bind(mageString, "Vanilla Ion Surge - Shock", true, "Ion Surge shocks enemies.").Value;
            MageCore.iceWallRework = SurvivorCfg.Bind(mageString, "Snapfreeze Rework", true, "Adds blast jumping to Snapfreeze.").Value;
            MageCore.ionSurgeRework = SurvivorCfg.Bind(mageString, "Ion Surge Rework", true, "Moves Ion Surge to the Utility slot and changes it into a blast jumping skill.").Value;

            MercCore.enabled = SurvivorCfg.Bind(mercString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            MercCore.modifyStats = SurvivorCfg.Bind(mercString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;
            MercCore.m1ComboFinishTweak = SurvivorCfg.Bind(mercString, "M1 Attack Speed Tweak (Client-Side)", true, "Makes the 3rd hit of Merc's M1 be unaffected by attack speed for use with combo tech.").Value;

            TreebotCore.enabled = SurvivorCfg.Bind(treebotString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            TreebotCore.drillChanges = SurvivorCfg.Bind(treebotString, "DIRECTIVE Drill Changes", true, "Enable changes to this skill.").Value;
            TreebotCore.swapUtilityEffects = SurvivorCfg.Bind(treebotString, "Utility - Swap Effects", true, "Swaps the effects of REXs Utilities").Value;
            ModifyUtilityForce.enabled = SurvivorCfg.Bind(treebotString, "Utility - Modify Force", true, "Modifies the force of REXs Utilities.").Value;
            TreebotCore.fruitChanges = SurvivorCfg.Bind(treebotString, "DIRECTIVE Harvest Changes", true, "Enable changes to this skill.").Value;

            LoaderCore.enabled = SurvivorCfg.Bind(loaderString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            LoaderCore.grappleCancelsSprint = SurvivorCfg.Bind(loaderString, "Secondaries Cancel Sprint", false, "Loader's Grapple cancels sprinting.").Value;
            LoaderCore.shiftCancelsSprint = SurvivorCfg.Bind(loaderString, "Utilities Cancel Sprint", false, "Loader's Big Punches cancel sprinting.").Value;
            LoaderCore.modifyStats = SurvivorCfg.Bind(loaderString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;
            LoaderCore.zapFistChanges = SurvivorCfg.Bind(loaderString, "Thunder Gauntlet Changes", true, "Enable changes to this skill.").Value;
            LoaderCore.slamChanges = SurvivorCfg.Bind(loaderString, "Thunderslam Changes", true, "Enable changes to this skill.").Value;
            LoaderCore.pylonChanges = SurvivorCfg.Bind(loaderString, "M551 Pylon Changes", true, "Enable changes to this skill.").Value;

            CrocoCore.enabled = SurvivorCfg.Bind(crocoString, "Enable Changes", true, "Enable changes to this survivor. Skill options unavailable due to all the changes being too interlinked.").Value;
            CrocoCore.gameplayRework = SurvivorCfg.Bind(crocoString, "Gameplay Rework", true, "A full rework of Acrid's skills.").Value;
            BiggerMeleeHitbox.enabled = SurvivorCfg.Bind(crocoString, "Extend Melee Hitbox", true, "Extends Acrid's melee hitbox so he can hit Vagrants while standing on top of them.").Value;
            BlightStack.enabled = SurvivorCfg.Bind(crocoString, "Blight Duration Reset", true, "Blight stacks like Bleed.").Value;
            RemovePoisonDamageCap.enabled = SurvivorCfg.Bind(crocoString, "Remove Poison Damage Cap", true, "Poison no longer has a hidden damage cap.").Value;
            BiggerLeapHitbox.enabled = SurvivorCfg.Bind(crocoString, "Extend Leap Collision Box", true, "Acrid's Shift skills have a larger collision hitbox. Damage radius remains the same.").Value;
            ShiftAirControl.enabled = SurvivorCfg.Bind(crocoString, "Leap Air Control", true, "Acrid's Shift skills gain increased air control at high move speeds.").Value;

            CaptainCore.enabled = SurvivorCfg.Bind(captainString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            CaptainOrbitalHiddenRealms.enabled = SurvivorCfg.Bind(captainString, "Hidden Realm Orbital Skills", true, "Allow Orbital skills in Hiden Realms.").Value;
            Microbots.enabled = SurvivorCfg.Bind(captainString, "Defensive Microbots Nerf", true, "Defensive Microbots no longer deletes stationary projectiles like gas clouds and Void Reaver mortars.").Value;
            CaptainCore.enablePrimarySkillChanges = SurvivorCfg.Bind(captainString, "Enable Primary Skill Changes", true, "Enable primary skill changes for this survivor.").Value;
            CaptainCore.modifyTaser = SurvivorCfg.Bind(captainString, "Power Taser Changes", true, "Enable changes to this skill.").Value;
            CaptainCore.nukeBuff = SurvivorCfg.Bind(captainString, "Diablo Strike Changes", true, "Enable changes to this skill.").Value;
            CaptainCore.beaconRework = SurvivorCfg.Bind(captainString, "Beacon Changes", true, "Beacons can be replaced on a cooldown and reworks Supply and Hack beacons.").Value;

            Bandit2Core.enabled = SurvivorCfg.Bind(banditString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            BanditSpecialGracePeriod.enabled = SurvivorCfg.Bind(banditString, "Special Grace Period", true, "Special On-kill effects can trigger if an enemy dies shortly after being hit.").Value;
            BanditSpecialGracePeriod.duration = SurvivorCfg.Bind(banditString, "Special Grace Period Duration", 1.2f, "Length in seconds of Special Grace Period.").Value;
            DesperadoRework.enabled = SurvivorCfg.Bind(banditString, "Persistent Desperado", true, "Desperado stacks are weaker but last between stages.").Value;
            BackstabRework.enabled = SurvivorCfg.Bind(banditString, "Backstab Changes", true, "Backstabs minicrit for 50% bonus damage and crit chance is converted to crit damage..").Value;
            BuffHemorrhage.ignoreArmor = SurvivorCfg.Bind(banditString, "Hemmorrhage - Ignore Armor", true, "Hemmorrhage damage ignores positive armor.").Value;
            BuffHemorrhage.enableProcs = SurvivorCfg.Bind(banditString, "Hemmorrhage - Enable Procs", true, "Hemmorrhage has a low proc coefficient.").Value;

            //BuffHemorrhage.enableCrit = SurvivorCfg.Bind(banditString, "Hemmorrhage - Count as Crit", true, "Hemmorrhagedamage counts as crits.").Value;
            BuffHemorrhage.enableCrit = false;  //hitsound is obnoxious

            Bandit2Core.burstChanges = SurvivorCfg.Bind(banditString, "Burst Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.blastChanges = SurvivorCfg.Bind(banditString, "Blast Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.noKnifeCancel = SurvivorCfg.Bind(banditString, "Knife While Reloading", true, "Knife skills can be used without interrupting your reload.").Value;
            Bandit2Core.knifeChanges = SurvivorCfg.Bind(banditString, "Serrated Dagger Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.knifeThrowChanges = SurvivorCfg.Bind(banditString, "Serrated Shiv Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.utilityFix = SurvivorCfg.Bind(banditString, "Smokebomb Fix", true, "Fixes various bugs with Smokebomb.").Value;
            Bandit2Core.specialRework = SurvivorCfg.Bind(banditString, "Special Rework", true, "Makes Resets/Desperado a selectable passive and adds a new Special skill.").Value;

            RailgunnerCore.enabled = SurvivorCfg.Bind(railgunnerString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            Survivors.DLC1.Railgunner.FixBungus.enabled = SurvivorCfg.Bind(railgunnerString, "Fix Bungus", true, "Removes self knockback force when on the ground.").Value;

            VoidFiendCore.enabled = SurvivorCfg.Bind(voidFiendString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            VoidFiendCore.fasterCorruptTransition = SurvivorCfg.Bind(voidFiendString, "Faster Corrupt Transition", true, "Speed up the corruption transform animation.").Value;
            VoidFiendCore.corruptOnKill = SurvivorCfg.Bind(voidFiendString, "Corruption on Kill", true, "Gain Corruption on kill. Lowers passive Corruption gain and Corrupted form duration.").Value;
            VoidFiendCore.corruptMeterTweaks = SurvivorCfg.Bind(voidFiendString, "Corruption Meter Tweaks", true, "Faster decay, slower passive buildup. Corrupted Suppress can be used as long as you have the HP for it. Meant to be used with Corrupt on Kill.").Value;
            VoidFiendCore.noCorruptCrit = SurvivorCfg.Bind(voidFiendString, "No Corruption on Crit", true, "Disables Corruption gain on crit.").Value;
            VoidFiendCore.noCorruptHeal = SurvivorCfg.Bind(voidFiendString, "No Corruption loss on Heal", true, "Disables Corruption loss on heal.").Value;
            VoidFiendCore.secondaryMultitask = SurvivorCfg.Bind(voidFiendString, "Secondary Multitasking", true, "Drown and Suppress can be fired while charging Flood.").Value;
        }

        private static void ConfigMonsters()
        {
            MonsterCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Monsters.cfg"), true);
            FixVengeanceLeveling.enabled = MonsterCfg.Bind(monsterString, "Fix Vengeance Doppelganger Levels", true, "Fix Vengeance Doppelgangers not leveling up.").Value;

            Beetle.enabled = MonsterCfg.Bind(monsterString, "Beetle", true, "Enable changes to this monster.").Value;
            Jellyfish.enabled = MonsterCfg.Bind(monsterString, "Jellyfish", true, "Enable changes to this monster.").Value;
            Imp.enabled = MonsterCfg.Bind(monsterString, "Imp", true, "Enable changes to this monster.").Value;
            HermitCrab.enabled = MonsterCfg.Bind(monsterString, "Hermit Crab", true, "Enable changes to this monster.").Value;

            Golem.enabled = MonsterCfg.Bind(monsterString, "Stone Golem", true, "Enable changes to this monster.").Value;
            Mushrum.enabled = MonsterCfg.Bind(monsterString, "Mini Mushrum", true, "Enable changes to this monster.").Value;

            Bronzong.enabled = MonsterCfg.Bind(monsterString, "Brass Contraption", true, "Enable changes to this monster.").Value;
            GreaterWisp.enabled = MonsterCfg.Bind(monsterString, "Greater Wisp", true, "Enable changes to this monster.").Value;

            Parent.enabled = MonsterCfg.Bind(monsterString, "Parent", true, "Enable changes to this monster.").Value;

            LunarGolem.enabled = MonsterCfg.Bind(monsterString, "Lunar Golem", true, "Enable changes to this monster.").Value;
            LunarWisp.enabled = MonsterCfg.Bind(monsterString, "Lunar Wisp", true, "Enable changes to this monster.").Value;

            BeetleQueen.enabled = MonsterCfg.Bind(monsterString, "Beetle Queen", true, "Enable changes to this monster.").Value;
            Vagrant.enabled = MonsterCfg.Bind(monsterString, "Wandering Vagrant", true, "Enable changes to this monster.").Value;
            Gravekeeper.enabled = MonsterCfg.Bind(monsterString, "Grovetender", true, "Enable changes to this monster.").Value;
            SCU.enabled = MonsterCfg.Bind(monsterString, "Solus Control Unit", true, "Enable changes to this monster.").Value;

            AWU.enabled = MonsterCfg.Bind(monsterString, "Alloy Worship Unit", true, "Enable changes to this monster.").Value;

            BlindPest.enabled = MonsterCfg.Bind(monsterString, "Blind Pest", true, "Enable changes to this monster.").Value;

            VoidInfestor.enabled = MonsterCfg.Bind(monsterString, "Void Infestor", true, "Enable changes to this monster.").Value;
            VoidInfestor.noVoidAllies = MonsterCfg.Bind(monsterString, "Void Infestor - No Ally Infestation", true, "Void Infestors can't possess allies.").Value;
        }
    }
}
