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
using RiskyMod.Tweaks.Artifact;
using RiskyMod.Tweaks;
using RiskyMod.Tweaks.CharacterMechanics;
using RiskyMod.Tweaks.RunScaling;
using RiskyMod.VoidFields;
using RiskyMod.Items.DLC1.Boss;
using RiskyMod.Items.DLC1.Equipment;
using RiskyMod.Enemies.Mithrix;
using RiskOfOptions;
using RiskyMod.Allies.DroneChanges;
using RiskyMod.Enemies.Spawnpools;
using RiskyMod.Enemies.DLC1.Voidling;
using System.Runtime.CompilerServices;
using RiskyMod.Allies.DamageResistances;
using RiskyMod.Enemies.DLC2;
using RiskyMod.Survivors.Croco.Tweaks;
using RiskyMod.Survivors.Croco.Contagion;
using RiskyMod.Items.DLC2;
using RiskyMod.Tweaks.Interactables;
using RiskyMod.Survivors.DLC2.Seeker;
using RiskyMod.Survivors.DLC2.FalseSon;

namespace RiskyMod
{
    public static class ConfigFiles
    {
        public static ConfigFile ItemCfg;
        public static ConfigFile SurvivorCfg;
        public static ConfigFile MonsterCfg;
        public static ConfigFile SpawnpoolCfg;
        public static ConfigFile GeneralCfg;

        public static ConfigFile SurvivorCrocoCfg;

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
        private const string seekerString = "Survivors: Seeker";
        private const string falseSonString = "Survivors: False Son";

        private const string monsterString = "Monsters";
        private const string monsterGeneralString = "General";
        private const string monsterMithrixString = "Mithrix";
        private const string monsterVoidlingString = "Voidling";

        public static void Init()
        {
            ConfigGeneral();
            ConfigItems();
            ConfigSurvivors();
            ConfigMonsters();
            ConfigSpawnpools();

            if (SoftDependencies.RiskOfOptionsLoaded) RiskOfOptionsCompat();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RiskOfOptionsCompat()
        {
            ModSettingsManager.SetModIcon(Content.Assets.MiscSprites.ModIcon);

            if (SurvivorsCore.enabled)
            {
                if (CaptainCore.enabled && CaptainCore.enablePrimarySkillChanges)
                {
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(EntityStates.RiskyMod.Captain.FireShotgun.scalePellets));
                }

                if (MageCore.enabled)
                {
                    if (MageCore.iceWallRework || MageCore.enableFireUtility)
                    {
                        ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(MageCore.utilitySelfKnockback));
                    }
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(MageCore.flamethrowerSprintCancel));
                }
            }
        }

        private static void ConfigGeneral()
        {
            GeneralCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_General.cfg"), true);

            //Core Modules
            AlliesCore.enabled = GeneralCfg.Bind(coreModuleString, "Ally Changes", true, "Enable drone and ally changes.").Value;
            ItemsCore.enabled = GeneralCfg.Bind(coreModuleString, "Item Changes", true, "Enable item changes.").Value;
            SurvivorsCore.enabled = GeneralCfg.Bind(coreModuleString, "Survivor Changes", true, "Enable survivor changes.").Value;
            EnemiesCore.modifyEnemies = GeneralCfg.Bind(coreModuleString, "Monster Changes", true, "Enable enemy changes.").Value;
            EnemiesCore.modifySpawns = GeneralCfg.Bind(coreModuleString, "Spawnpool Changes", true, "Enable spawnpool changes.").Value;
            MoonCore.enabled = GeneralCfg.Bind(coreModuleString, "Moon Changes", true, "Enable Moon changes.").Value;
            VoidLocusCore.enabled = GeneralCfg.Bind(coreModuleString, "Void Locus Changes", true, "Enable Void Locus changes.").Value;
            VoidFieldsCore.enabled = GeneralCfg.Bind(coreModuleString, "Void Fields Changes", true, "Enable Void Fields changes.").Value;

            //Game Mechanics
            RiskyMod.disableProcChains = GeneralCfg.Bind(gameMechString, "Disable Proc Chains", true, "Remove the proc coefficient on most item effects.").Value;
            ShieldGating.enabled = GeneralCfg.Bind(gameMechString, "Shield Gating", false, "Shields gate against HP damage.").Value;
            ShieldGating.disableIfGuardiansHeart = GeneralCfg.Bind(gameMechString, "Shield Gating - Disable if Guardians Heart is Loaded", true, "Shield Gating is disabled if Guardians Heart from ClassicItemsReturns is enabled.").Value;
            TrueOSP.enabled = GeneralCfg.Bind(gameMechString, "True OSP", true, "Makes OSP work against multihits.").Value;
            AIBlacklistItems.enabled = GeneralCfg.Bind(gameMechString, "Expanded AI Blacklist", true, "Adds extra items to the AI Blacklist by default.").Value;
            TeleExpandOnBossKill.enabled = GeneralCfg.Bind(gameMechString, "Tele Expand on Boss Kill", true, "Teleporter expands to cover the whole map when the boss is killed.").Value;
            TeleExpandOnBossKill.enableDuringEclipse = GeneralCfg.Bind(gameMechString, "Tele Expand on Boss Kill - Enable During Eclispe", false, "Enables teleporter expansion when playing on Eclipse 2+.").Value;
            LargeLobbyScaling.enabled = GeneralCfg.Bind(gameMechString, "Tele Large Lobby Scaling", true, "Holdout size is increased in lobbies with 5+ players.").Value;

            TeleChargeDuration.enabled = GeneralCfg.Bind(gameMechString, "Tele Charge Duration Increase", false, "Increases teleporter charge duration from 90s to 120s like RoR1.").Value;

            //Run Scaling
            CombatDirectorMultiplier.directorCreditMultiplier = GeneralCfg.Bind(scalingString, "Combat Director Credit Multiplier", 1.4f, "Multiply Combat Director credits by this amount. Set to 1 to disable").Value;
            CombatDirectorMultiplier.enabled = CombatDirectorMultiplier.directorCreditMultiplier != 1f;

            MonsterGoldRewards.enabled = GeneralCfg.Bind(scalingString, "Gold Scaling Tweaks", true, "Lowers gold earn rate scaling. Mainly makes a difference when looping.").Value;
            MonsterGoldRewards.inflationCoefficient = GeneralCfg.Bind(scalingString, "Gold Scaling Tweaks - Inflation Coefficient", 0.2f, "Lowers how fast gold gain scales against chest prices. 0 = Vanilla, 0.4 = Equilibrium").Value;
            if (!MonsterGoldRewards.enabled)
            {
                MonsterGoldRewards.linearize = false;
                MonsterGoldRewards.scaleToInitialDifficulty = false;
                if (CombatDirectorMultiplier.enabled)
                {
                    MonsterGoldRewards.enabled = true;
                }
            }
            else
            {
                MonsterGoldRewards.scaleToDirectorMultiplier = GeneralCfg.Bind(scalingString, "Gold Scaling Tweaks - Scale to Director Multiplier", true, "Proportionally lowers gold scaling by Combat Director Credit Multipier.").Value;
                MonsterGoldRewards.scaleToDirectorMultiplierStage1 = GeneralCfg.Bind(scalingString, "Gold Scaling Tweaks - Scale to Director Multiplier - Stage 1", false, "Enable the above setting on Stage 1.").Value;
            }

            ModdedScaling.enabled = GeneralCfg.Bind(scalingString, "Scaling: Modded Scaling", true, "Lowers the effect of playercount on difficulty scaling. Geared towards large lobbies.").Value;
            LinearScaling.enabled = GeneralCfg.Bind(scalingString, "Scaling: Linear Scaling", false, "Makes difficulty scaling linear like in RoR1. Requires Modded Scaling to be enabled.").Value;
            LinearScaling.swapToExponential = GeneralCfg.Bind(scalingString, "Scaling: Linear Scaling - Swap to Exponential Lategame", false, "Linear Scaling swaps to exponential scaling when it gets outscaled.").Value;

            LoopTeleMountainShrine.enabled = GeneralCfg.Bind(scalingString, "Loop Teleporter Boss Credits", true, "Teleporter Boss director credits increase by 1 Mountain Shrine every loop.").Value;
            
            //Allies
            AntiSplat.enabled = GeneralCfg.Bind(allyString, "No Splat Death", true, "Allies are immune to physics damage.").Value;
            NoVoidDamage.enabled = GeneralCfg.Bind(allyString, "No Void Damage", true, "Allies take no damage from Void fog.").Value;
            NoVoidDamage.alliesInVoidFields = GeneralCfg.Bind(allyString, "Drones in Void Fields", true, "Drones spawn in Void Fields.").Value;
            MegaDrone.allowRepair = GeneralCfg.Bind(allyString, "TC280 - Enable Repairs", true, "TC280s can be repaired after being destroyed.").Value;
            GunnerTurret.allowRepair = GeneralCfg.Bind(allyString, "Gunner Turret - Enable Repairs", true, "Gunner Turrets can be repaired after being destroyed.").Value;
            GunnerTurret.teleportWithPlayer = GeneralCfg.Bind(allyString, "Gunner Turret - Teleport with Player", true, "Gunner Turrets are teleported to the player when starting the Teleporter event.").Value;
            GunnerTurret.teleportToMithrix = GeneralCfg.Bind(allyString, "Gunner Turret - Teleport with Player - Teleport to Mithrix", true, "Gunner Turrets are teleported to the player when fighting Mithrix. Requires Teleport with Player.").Value;
            AntiOverheat.enabled = GeneralCfg.Bind(allyString, "No Overheat Death", true, "Drones cannot die to Overheat.").Value;
            AntiVoidDeath.enabled = GeneralCfg.Bind(allyString, "No Void Death", true, "Drones cannot die to Void Implosions.").Value;
            AntiOneshot.enabled = GeneralCfg.Bind(allyString, "Drone Oneshot Protection", true, "Drones cannot take more than 60% of their health in a single attack.").Value;

            SpawnLimits.maxVoidSeeds = GeneralCfg.Bind(interactString, "Max Void Seeds", 1, "Limit how many Void Seeds can spawn on 1 stage. Vanilla is 3. Set to negative for no limit.").Value;
            ExtraVoidSeedPerLoop.maxExtraSeeds = GeneralCfg.Bind(interactString, "Per-Loop Max Extra Void Seeds", 2, "Increases Max Void Seeds per loop by 1 until this number is reached. Set to negative for no limit.").Value;

            //Artifacts
            Sacrifice.enabled = GeneralCfg.Bind(artifactString, "Sacrifice - No Drop Chance Scaling", true, "Increases drop chance when not using Swarms and prevents drop chance from increasing as the run progresses.").Value;

            //Void Fields
            VoidFields.LargerHoldout.enabled = GeneralCfg.Bind(voidFieldsString, "Larger Holdout Zone", true, "Increase radius of holdout zone.").Value;
            VoidFields.ReduceHoldoutCount.enabled = GeneralCfg.Bind(voidFieldsString, "Less Cells", true, "Reduces Cell count from 9 to 5 and speed up enemy progression.").Value;

            //Moon
            Moon.LargerHoldouts.enabled = GeneralCfg.Bind(moonString, "Larger Holdout Zone", true, "Increase radius of holdout zone.").Value;
            LessPillars.enabled = GeneralCfg.Bind(moonString, "Reduce Pillar Count", true, "Reduce the amount of pillars required to activate the jump pads.").Value;
            
            //Void Locus
            RemoveFog.enabled = GeneralCfg.Bind(voidLocusString, "Remove Fog", false, "Removes Void Fog from the map.").Value;
            VoidLocus.LargerHoldout.enabled = GeneralCfg.Bind(voidLocusString, "Larger Holdout Zone", true, "Increase radius of holdout zone.").Value;
            
            //Misc
            BetterProjectileTracking.enabled = GeneralCfg.Bind(miscString, "Better Projectile Homing", true, "Homing projectiles target based on angle, instead of distance + angle.").Value;
            FreezeChampionExecute.enabled = GeneralCfg.Bind(miscString, "Freeze Executes Bosses", true, "Freeze counts as a debuff and can execute bosses at 15% HP.").Value;
            FreezeChampionExecute.nerfFreeze = GeneralCfg.Bind(miscString, "Freeze Executes Bosses - Nerf Freeze Globally", false, "Freeze execute threshold is reduced to 15% globally. Requires Freeze Executes Bosses.").Value;
            PlayerControlledMonsters.enabled = GeneralCfg.Bind(miscString, "Player-Controlled Monster Tweaks", true, "Gives players health regen + armor when playing as monsters via mods.").Value;
            NullifyDebuff.enabled = GeneralCfg.Bind(miscString, "Nullify Buff", true, "Void Reaver Nullify only takes 2 stacks to apply.").Value;
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
            Crowbar.enabled = ItemCfg.Bind(commonString, "Crowbar", true, itemConfigDescString).Value;

            DelicateWatch.enabled = ItemCfg.Bind(commonString, "Delicate Watch", true, itemConfigDescString).Value;
            DelicateWatch.useBuff = ItemCfg.Bind(commonString, "Delicate Watch - Show Buff on HUD", true, "Damage bonus from this item is shown as a buff on your HUD.").Value;
            PowerElixir.enabled = ItemCfg.Bind(commonString, "Power Elixir", true, itemConfigDescString).Value;

            Fireworks.enabled = ItemCfg.Bind(commonString, "Fireworks", true, itemConfigDescString).Value;
            Fireworks.maxRockets = ItemCfg.Bind(commonString, "Fireworks - Max Rockets", 32, "Max rockets that can spawn from each use. Going above this value raises rocket damage instead.").Value;
            Gasoline.enabled = ItemCfg.Bind(commonString, "Gasoline", true, itemConfigDescString).Value;
            CritGlasses.enabled = ItemCfg.Bind(commonString, "Lensmakers Glasses", false, "Reduce crit chance from 10% to 7% like in RoR1.").Value;
            MonsterTooth.enabled = ItemCfg.Bind(commonString, "Monster Tooth", true, itemConfigDescString).Value;
            StickyBomb.enabled = ItemCfg.Bind(commonString, "Stickybomb", true, itemConfigDescString).Value;
            TougherTimes.enabled = ItemCfg.Bind(commonString, "Tougher Times", true, itemConfigDescString).Value;
            Warbanner.enabled = ItemCfg.Bind(commonString, "Warbanner", true, itemConfigDescString).Value;
            Warbanner.UseModdedBuff = ItemCfg.Bind(commonString, "Warbanner - Use Custom Buff", true, "Warbanner is handled via a custom buff and GameObject so that it doesn't interfere with other mods that rely on it.").Value;

            //Makes it too easy to stunlock things even at very low stacks.
            //StunGrenade.enabled = ItemCfg.Bind(commonString, "Stun Grenade", true, itemConfigDescString).Value;
            StunGrenade.enabled = false;
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
            Infusion.enabled = ItemCfg.Bind(uncommonString, "Infusion", true, itemConfigDescString).Value;
            Infusion.useBuff = ItemCfg.Bind(uncommonString, "Infusion - Show Buff on HUD", true, "HP gained from Infusion is displayed as a buff on your HUD.").Value;
            LeechingSeed.enabled = ItemCfg.Bind(uncommonString, "Leeching Seed", true, itemConfigDescString).Value;
            Daisy.enabled = ItemCfg.Bind(uncommonString, "Lepton Daisy", true, itemConfigDescString).Value;
            Daisy.disableHealPulse = ItemCfg.Bind(uncommonString, "Lepton Daisy - Disable Heal Pulse", true, "Disables the Vanilla Heal Pulse if Lepton Daisy rework is enabled.").Value;

            Guillotine.enabled = ItemCfg.Bind(uncommonString, "Old Guillotine", true, itemConfigDescString).Value;
            Guillotine.reduceVFX = ItemCfg.Bind(uncommonString, "Old Guillotine - Reduce VFX", true, "Reduce how often this item's VFX shows up.").Value;

            Razorwire.enabled = ItemCfg.Bind(uncommonString, "Razorwire", true, itemConfigDescString).Value;
            RedWhip.enabled = ItemCfg.Bind(uncommonString, "Red Whip", true, itemConfigDescString).Value;
            RoseBuckler.enabled = ItemCfg.Bind(uncommonString, "Rose Buckler", false, itemConfigDescString).Value;
            SquidPolyp.enabled = ItemCfg.Bind(uncommonString, "Squid Polyp", true, itemConfigDescString).Value;
            SquidPolyp.ignoreAllyCap = ItemCfg.Bind(uncommonString, "Squid Polyp - Ignore Ally Cap", true, "Squid Polyps ignore the ally cap if changes are enabled.").Value;
            SquidPolyp.scaleCount = ItemCfg.Bind(uncommonString, "Squid Polyp - Stacks Increase Max Squids", false, "Extra stacks allow for more squids to spawn. Will lag in MP.").Value;
            SquidPolyp.cannotCopy = ItemCfg.Bind(uncommonString, "Squid Polyp - CannotCopy Tag", false, "If True, prevent minions like Engi Turrets from receiving this item.").Value;
            SquidPolyp.shareCountWithMinions = ItemCfg.Bind(uncommonString, "Squid Polyp - Share Count with Minions", false, "If True, minions such as Engi Turrets will share their squid count with their owner.").Value;
            Stealthkit.enabled = ItemCfg.Bind(uncommonString, "Old War Stealthkit", true, itemConfigDescString).Value;
            Ukulele.enabled = ItemCfg.Bind(uncommonString, "Ukulele", true, itemConfigDescString).Value;
            WarHorn.enabled = ItemCfg.Bind(uncommonString, "War Horn", true, itemConfigDescString).Value;
            WillOWisp.enabled = ItemCfg.Bind(uncommonString, "Will-o-the-Wisp", true, itemConfigDescString).Value;

            Ignition.enabled = ItemCfg.Bind(uncommonString, "Ignition Tank", true, itemConfigDescString).Value;

            BreachingFin.enabled = ItemCfg.Bind(uncommonString, "Breaching Fin", true, itemConfigDescString).Value;
            NoxiousThorn.enabled = ItemCfg.Bind(uncommonString, "Noxious Thorn", true, itemConfigDescString).Value;
        }

        private static void ConfigLegendaryItems()
        {
            Behemoth.enabled = ItemCfg.Bind(legendaryString, "Brilliant Behemoth", false, itemConfigDescString).Value;
            CeremonialDagger.enabled = ItemCfg.Bind(legendaryString, "Ceremonial Dagger", true, itemConfigDescString).Value;
            Clover.enabled = ItemCfg.Bind(legendaryString, "57 Leaf Clover", false, "Caps how high the Luck stat can go.").Value;
            Clover.luckCap = ItemCfg.Bind(legendaryString, "57 Leaf Clover - Max Luck", 1f, "Maximum Luck value players can reach. Extra Luck is converted to stat boosts.").Value;
            FrostRelic.enabled = ItemCfg.Bind(legendaryString, "Frost Relic", true, itemConfigDescString).Value;
            HappiestMask.enabled = ItemCfg.Bind(legendaryString, "Happiest Mask", true, itemConfigDescString).Value;
            HappiestMask.scaleCount = ItemCfg.Bind(legendaryString, "Happiest Mask - Stacks Increase Max Ghosts", false, "Extra stacks allow for more ghosts to spawn. Will lag in MP.").Value;
            HappiestMask.noGhostLimit = ItemCfg.Bind(legendaryString, "Happiest Mask - Remove Ghost Limit", false, "Removes the ghost limit at all times. Definitely will lag.").Value;
            HeadHunter.enabled = ItemCfg.Bind(legendaryString, "Wake of Vultures", true, itemConfigDescString).Value;
            HeadHunter.perfectedTweak = ItemCfg.Bind(legendaryString, "Wake of Vultures - Perfected Tweak", true, "Perfected Affix gained via Wake of Vultures will not force your health pool to become shields.").Value;
            Headstompers.enabled = ItemCfg.Bind(legendaryString, "H3AD-ST", true, itemConfigDescString).Value;
            Headstompers.useBuff = ItemCfg.Bind(legendaryString, "H3AD-ST - Show Buff on HUD", true, "H3AD-ST cooldown is shown as a buff on your HUD. Disabling this prevents Blast Shower from clearing the item's cooldown.").Value;
            Scorpion.enabled = ItemCfg.Bind(legendaryString, "Symbiotic Scorpion", true, itemConfigDescString).Value;

            SpareDroneParts.enabled = ItemCfg.Bind(legendaryString, "Spare Drone Parts", true, itemConfigDescString).Value;
            SpareDroneParts.ignoreAllyCap = ItemCfg.Bind(legendaryString, "Spare Drone Parts - Ignore Ally Cap", true, "Col. Droneman ignores the ally cap if changes are enabled.").Value;

            Aegis.enabled = ItemCfg.Bind(legendaryString, "Aegis", true, itemConfigDescString).Value;

            Tesla.enabled = ItemCfg.Bind(legendaryString, "Unstable Tesla Coil", true, itemConfigDescString).Value;
            Raincoat.enabled = ItemCfg.Bind(legendaryString, "Bens Raincoat", true, itemConfigDescString).Value;
            Raincoat.replaceIcons = ItemCfg.Bind(legendaryString, "Bens Raincoat - Use Modded Icons", false, "Replace the Vanilla buff icons for this item.").Value;
        
            LaserScope.enabled = ItemCfg.Bind(legendaryString, "Laser Scope", true, itemConfigDescString).Value;

            Overspill.enabled = ItemCfg.Bind(legendaryString, "Runic Lens", true, itemConfigDescString).Value;
            GrowthNectar.enabled = ItemCfg.Bind(legendaryString, "Growth Nectar", true, itemConfigDescString).Value;
        }

        private static void ConfigVoidItems()
        {
            Dungus.enabled = ItemCfg.Bind(voidString, "Weeping Fungus", true, itemConfigDescString).Value;

            SaferSpaces.enabled = ItemCfg.Bind(voidString, "Safer Spaces", true, itemConfigDescString).Value;
            SaferSpaces.addIframes = ItemCfg.Bind(voidString, "Safer Spaces - I-frames on Proc", false, "Gives brief invincibility when triggering this effect.").Value;

            PlasmaShrimp.enabled = ItemCfg.Bind(voidString, "Plasma Shrimp", true, itemConfigDescString).Value;
            VoidWisp.enabled = ItemCfg.Bind(voidString, "Voidsent Flame", true, itemConfigDescString).Value;
            Polylute.enabled = ItemCfg.Bind(voidString, "Polylute", true, itemConfigDescString).Value;
            VoidRing.enabled = ItemCfg.Bind(voidString, "Singularity Band", true, itemConfigDescString).Value;

            Zoea.ignoreAllyCap = ItemCfg.Bind(voidString, "Newly Hatched Zoea - Ignore Ally Cap", true, "Zoea Allies ignore the ally cap.").Value;
            Zoea.maxAllyCount = ItemCfg.Bind(voidString, "Newly Hatched Zoea - Max Allies per Player", 6, "Max Void allies each player can summon.").Value;
            Zoea.enabled = ItemCfg.Bind(voidString, "Newly Hatched Zoea", true, "Use the Max Allies per Player setting.").Value;
        }

        private static void ConfigBossItems()
        {
            ChargedPerf.enabled = ItemCfg.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            Disciple.enabled = ItemCfg.Bind(bossString, "Charged Perforator", true, itemConfigDescString).Value;
            Knurl.enabled = ItemCfg.Bind(bossString, "Titanic Knurl", true, itemConfigDescString).Value;
            MoltenPerf.enabled = ItemCfg.Bind(bossString, "Molten Perforator", true, itemConfigDescString).Value;

            QueensGland.enabled = ItemCfg.Bind(bossString, "Queens Gland", true, itemConfigDescString).Value;
            QueensGland.ignoreAllyCap = ItemCfg.Bind(bossString, "Queens Gland - Ignore Ally Cap", true, "Queens Gland Guards ignore the ally cap if changes are enabled.").Value;

            Shatterspleen.enabled = ItemCfg.Bind(bossString, "Shatterspleen", true, itemConfigDescString).Value;

            EmpathyCores.ignoreAllyCap = ItemCfg.Bind(bossString, "Empathy Cores - Ignore Ally Cap", true, "Empathy Cores ignore the ally cap.").Value;
            EmpathyCores.enabled = EmpathyCores.enabled && EmpathyCores.ignoreAllyCap;

            DefenseNucleus.enabled = ItemCfg.Bind(bossString, "Defense Nucleus", true, itemConfigDescString).Value;
            DefenseNucleus.removeAllyScaling = ItemCfg.Bind(bossString, "Defense Nucleus - Remove Ally Count Scaling", true, "Stacks increase ally damage and health instead of max allies.").Value;
            DefenseNucleus.inheritEliteAffix = ItemCfg.Bind(bossString, "Defense Nucleus - Inherit Elite Affix", true, "Defense Nucleus Alpha Constructs inherit the Elite Affix of the enemy that was killed. This removes their spawn VFX.").Value;
            DefenseNucleus.ignoreAllyCap = ItemCfg.Bind(bossString, "Defense Nucleus - Ignore Ally Cap", true, "Defense Nucleus Alpha Constructs ignore the ally cap if changes are enabled.").Value;

            HalcyonSeed.enabled = ItemCfg.Bind(bossString, "Halcyon Seed", true, itemConfigDescString).Value;
        }

        private static void ConfigLunars()
        {
            BrittleCrown.enabled = ItemCfg.Bind(lunarString, "Brittle Crown", true, itemConfigDescString).Value;
            Meteorite.enabled = ItemCfg.Bind(lunarString, "Glowing Meteorite", true, itemConfigDescString).Value;
            ShapedGlass.enabled = ItemCfg.Bind(lunarString, "Shaped Glass", true, itemConfigDescString).Value;
            Transcendence.enabled = ItemCfg.Bind(lunarString, "Transcendence", true, itemConfigDescString).Value;
        }

        private static void ConfigEquipment()
        {
            Backup.enabled = ItemCfg.Bind(equipmentString, "The Back-Up", true, itemConfigDescString).Value;
            Backup.ignoreTeamLimit = !ItemCfg.Bind(equipmentString, "The Back-Up: Limit Drones", false, "Back-Up drones count towards the ally cap.").Value;
            BackupTracker.maxCount = ItemCfg.Bind(equipmentString, "The Back-Up: Max Drones", 8, "Max active Backup Drones.").Value;
            BFG.enabled = ItemCfg.Bind(equipmentString, "Preon Accumulator", true, itemConfigDescString).Value;
            Capacitor.enabled = ItemCfg.Bind(equipmentString, "Royal Capacitor", true, itemConfigDescString).Value;
            Chrysalis.enabled = ItemCfg.Bind(equipmentString, "Milky Chrysalis", true, itemConfigDescString).Value;
            CritHud.enabled = ItemCfg.Bind(equipmentString, "Ocular HUD", true, itemConfigDescString).Value;
            SuperLeech.enabled = ItemCfg.Bind(equipmentString, "Super Massive Leech", true, itemConfigDescString).Value;
            VolcanicEgg.enabled = ItemCfg.Bind(equipmentString, "Volcanic Egg", true, itemConfigDescString).Value;
            Goobo.enabled = ItemCfg.Bind(equipmentString, "Goobo Jr.", true, itemConfigDescString).Value;
        }

        private static void ConfigSurvivors()
        {
            SurvivorCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Survivors.cfg"), true);
            CommandoCore.enabled = SurvivorCfg.Bind(commandoString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            CommandoCore.phaseRoundChanges = SurvivorCfg.Bind(commandoString, "Phase Round Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.rollChanges = SurvivorCfg.Bind(commandoString, "Tactical Dive Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.suppressiveChanges = SurvivorCfg.Bind(commandoString, "Suppressive Fire Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.grenadeChanges = SurvivorCfg.Bind(commandoString, "Frag Grenade Changes", true, "Enable changes to this skill.").Value;
            CommandoCore.grenadeRemoveFalloff = SurvivorCfg.Bind(commandoString, "Frag Grenade - Remove Falloff", true, "Remove blast falloff from Frag Grenade. Requires Frag Grenade Changes = true.").Value;
            CommandoCore.skillLightningRound = SurvivorCfg.Bind(commandoString, "Phase Lightning Secondary Skill", true, "Adds a new secondary skill.").Value;
            CommandoCore.replacePhaseRound = SurvivorCfg.Bind(commandoString, "Phase Lightning - Replace Default", false, "Removes Phase Round and replaces it with this skill.").Value;
            CommandoCore.skillShrapnelBarrage = SurvivorCfg.Bind(commandoString, "Shrapnel Barrage Special Skill", true, "Adds a new special skill.").Value;
            CommandoCore.replaceSuppressive = SurvivorCfg.Bind(commandoString, "Shrapnel Barrage - Replace Default", false, "Removes Suppressive Fire and replaces it with this skill.").Value;

            HuntressCore.enabled = SurvivorCfg.Bind(huntressString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            HuntressCore.strafeChanges = SurvivorCfg.Bind(huntressString, "Strafe Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.flurryChanges = SurvivorCfg.Bind(huntressString, "Flurry Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.laserGlaiveChanges = SurvivorCfg.Bind(huntressString, "Laser Glaive Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.blinkChanges = SurvivorCfg.Bind(huntressString, "Blink Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.arrowRainChanges = SurvivorCfg.Bind(huntressString, "Arrow Rain Changes", true, "Enable changes to this skill.").Value;
            HuntressCore.ballistaChanges = SurvivorCfg.Bind(huntressString, "Ballista Changes", true, "Enable changes to this skill.").Value;

            ToolbotCore.enabled = SurvivorCfg.Bind(toolbotString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            ToolbotCore.enableRebarChanges = SurvivorCfg.Bind(toolbotString, "Rebar Puncher Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableScrapChanges = SurvivorCfg.Bind(toolbotString, "Scrap Launcher Changes", true, "Enable changes to this skill.").Value;
            
            ToolbotCore.sawPhysics = SurvivorCfg.Bind(toolbotString, "Power Saw Physics Changes", true, "Makes Power Saw physics more consistent.").Value;

            ToolbotCore.sawHitbox = SurvivorCfg.Bind(toolbotString, "Power Saw Hitbox Changes", true, "Increases Power Saw hitbox size by 50%.").Value;
            ToolbotCore.sawBarrierOnHit = SurvivorCfg.Bind(toolbotString, "Power Saw Barrier On Hit", true, "Power Saw gives barrier-on-hit.").Value;
            ToolbotCore.enableSecondarySkillChanges = SurvivorCfg.Bind(toolbotString, "Blast Canister Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enablePowerModeChanges = SurvivorCfg.Bind(toolbotString, "Power Mode Changes", true, "Enable changes to this skill.").Value;

            EngiCore.enabled = SurvivorCfg.Bind(engiString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            PressureMines.enabled = SurvivorCfg.Bind(engiString, "Pressure Mine Changes", true, "Pressure Mines only detonate when fully armed.").Value;
            EngiCore.harpoonRangeTweak = SurvivorCfg.Bind(engiString, "Thermal Harpoon Changes", true, "Increases Thermal Harpoon targeting range.").Value;
            TurretChanges.normalizeStats = SurvivorCfg.Bind(engiString, "Normalize Turret Stats", true, "Reduces Turret base damage to 12 while keeping damage output the same.").Value;
            BubbleDefenseMatrix.enabled = SurvivorCfg.Bind(engiString, "Bubble Shield Changes", true, "Enable changes to this skill.").Value;

            MageCore.enabled = SurvivorCfg.Bind(mageString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            MageCore.modifyFireBolt = SurvivorCfg.Bind(mageString, "Fire Bolt Changes", true, "Enable changes to this skill.").Value;
            MageCore.modifyPlasmaBolt = SurvivorCfg.Bind(mageString, "Plasma Bolt Changes", true, "Enable changes to this skill.").Value;
            MageCore.m2RemoveNanobombGravity = SurvivorCfg.Bind(mageString, "Nanobomb - Remove Gravity", false, "Removes projectile drop from Nanobomb so it behaves like it did in Early Access.").Value;
            MageCore.buffNanoSpear = SurvivorCfg.Bind(mageString, "Nanospear Changes", true, "Enable changes to this skill.").Value;

            MageCore.ionSurgeUtility = SurvivorCfg.Bind(mageString, "Ion Surge - Move to Utility Slot", false, "Moves Ion Surge to the Utility slot.").Value;
            MageCore.ionSurgeUtilityKeepSpecial = SurvivorCfg.Bind(mageString, "Ion Surge - Move to Utility Slot - Keep Special", false, "If Ion Surge Utility is enabled, keep the Special version as a selectable skill.").Value;
            MageCore.ionSurgeShock = SurvivorCfg.Bind(mageString, "Ion Surge - Shock", true, "Ion Surge shocks enemies.").Value;

            MageCore.utilitySelfKnockback = SurvivorCfg.Bind(mageString, "Utility Self Knockback", false, "(Client-Side) Snapfreeze Rework and Blaze Storm apply self-knockback when used midair.");
            MageCore.utilitySelfKnockback.SettingChanged += MageCore.UpdatePushSetting;


            MageCore.iceWallRework = SurvivorCfg.Bind(mageString, "Snapfreeze Rework", true, "Snapfreeze can target midair enemies.").Value;
            Survivors.Mage.SkillTweaks.IceWallDefense.enabled = SurvivorCfg.Bind(mageString, "Snapfreeze Defense", true, "Snapfreeze blocks ranged attacks from enemies.").Value;
            MageCore.enableFireUtility = SurvivorCfg.Bind(mageString, "Blaze Storm Utility Skill", false, "Enables this custom skill.").Value;
            MageCore.flamethrowerSprintCancel = SurvivorCfg.Bind(mageString, "Flamethrower - Sprint Cancel", true, "(Client-Side) Sprinting cancels Flamethrower.");
            
            MageCore.enableLightningSpecial = SurvivorCfg.Bind(mageString, "Electrocute Special Skill", false, "Enables this custom skill. May be buggy.").Value;

            MercCore.enabled = SurvivorCfg.Bind(mercString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            MercCore.buffDefaultShift = SurvivorCfg.Bind(mercString, "Blinding Assault Changes", true, "Buff the damage of this skill.").Value;

            TreebotCore.enabled = SurvivorCfg.Bind(treebotString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            TreebotCore.drillChanges = SurvivorCfg.Bind(treebotString, "DIRECTIVE Drill Changes", true, "Enable changes to this skill.").Value;
            TreebotCore.defaultUtilityHeal = SurvivorCfg.Bind(treebotString, "DIRECTIVE Disperse Healing", true, "DIRECTIVE Disperse heals for 5% HP per target hit.").Value;
            TreebotCore.fruitChanges = SurvivorCfg.Bind(treebotString, "DIRECTIVE Harvest Changes", true, "Enable changes to this skill.").Value;
            DropFruitOnHit.enabled = SurvivorCfg.Bind(treebotString, "DIRECTIVE Harvest - Fruit On Hit", false, "Fruiting enemies have a chance to drop fruits when hit.").Value;
            
            LoaderCore.enabled = SurvivorCfg.Bind(loaderString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            LoaderCore.modifyStats = SurvivorCfg.Bind(loaderString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;
            LoaderCore.zapFistChanges = SurvivorCfg.Bind(loaderString, "Thunder Gauntlet Changes", true, "Enable changes to this skill.").Value;
            LoaderCore.slamChanges = SurvivorCfg.Bind(loaderString, "Thunderslam Changes", true, "Enable changes to this skill.").Value;
            LoaderCore.pylonChanges = SurvivorCfg.Bind(loaderString, "M551 Pylon Changes", true, "Enable changes to this skill.").Value;

            CrocoCore.enabled = SurvivorCfg.Bind(crocoString, "Enable Changes", true, "Enable changes to this survivor. Skill options unavailable due to all the changes being too interlinked.").Value;
            BlightReturns.enabled = SurvivorCfg.Bind(crocoString, "Blight Returns", true, "Reworks Blight to be similar to Corrosive Wounds from Returns.").Value;
            BiggerLeapHitbox.enabled = SurvivorCfg.Bind(crocoString, "Extend Leap Collision Box", true, "Acrid's Shift skills have a larger collision hitbox. Damage radius remains the same.").Value;
            ShiftAirControl.enabled = SurvivorCfg.Bind(crocoString, "Leap Air Control", false, "Acrid's Shift skills gain increased air control at high move speeds (causes momentum loss).").Value;
            UtilityKnockdown.enabled = SurvivorCfg.Bind(crocoString, "Leap Knockdown", true, "Acrid's Shift skills knock airborne enemies downwards.").Value;
            BuffFrenziedLeap.enabled = SurvivorCfg.Bind(crocoString, "Frenzied Leap Changes", true, "Enable changes to this skill.").Value;
            BuffShiftPuddleProc.enabled = SurvivorCfg.Bind(crocoString, "Acid Puddle Buff", true, "Buffs proc coefficient of Acid Puddles.").Value;
            ContagionPassive.enabled = SurvivorCfg.Bind(crocoString, "Contagion Passive Skill", true, "Enables the Contagion passive skill.").Value;
            ContagionPassive.useDebuffSpread = SurvivorCfg.Bind(crocoString, "Contagion - Debuff Spread on Kill", false, "Contagion makes Blight and Poison spread on kill.").Value;
            ContagionPassive.useUniqueSpecialDebuff = SurvivorCfg.Bind(crocoString, "Contagion - Unique Special Debuff", true, "Contagion uses a unique DoT for Epidemic.").Value;

            CaptainCore.enabled = SurvivorCfg.Bind(captainString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            Microbots.deletionRestrictions = SurvivorCfg.Bind(captainString, "Defensive Microbots Nerf", true, "Defensive Microbots no longer deletes stationary projectiles like gas clouds and Void Reaver mortars.").Value;
            Microbots.droneScaling = SurvivorCfg.Bind(captainString, "Defensive Microbots Drone Scaling", true, "Defensive Microbots scale with drone count instead of attack speed.").Value;
            CaptainCore.enablePrimarySkillChanges = SurvivorCfg.Bind(captainString, "Enable Primary Skill Changes", true, "Enable primary skill changes for this survivor.").Value;
            EntityStates.RiskyMod.Captain.FireShotgun.scalePellets = SurvivorCfg.Bind(captainString, "Enable Primary Skill Changes - Scale Pellets with Attackspeed", true, "Vulcan Shotgun gains extra pellets with attack speed.");
            EntityStates.RiskyMod.Captain.FireShotgun.scalePellets.SettingChanged += CaptainCore.ScalePellets_SettingChanged;


            CaptainCore.modifyTaser = SurvivorCfg.Bind(captainString, "Power Taser Changes", true, "Enable changes to this skill.").Value;
            CaptainCore.nukeAmmopackNerf = SurvivorCfg.Bind(captainString, "Diablo Strike Ammopack Nerf", true, "Ammopacks only restore half of Diablo Strike's charge. Intended for use with Beacon: Resupply changes.").Value;
            CaptainCore.nukeProc = SurvivorCfg.Bind(captainString, "Diablo Strike Proc Coeficient", true, "Increases Diablo Strike's proc coefficient.").Value;
            CaptainCore.nukeFalloff = SurvivorCfg.Bind(captainString, "Diablo Strike Remove Falloff", true, "Removes radius falloff from Diablo Strike.").Value;
            CaptainCore.disableNukeFriendlyFire = SurvivorCfg.Bind(captainString, "Diablo Strike Disable Friendly Fire", false, "Diablo Strike only hurts yourself instead of all teammates.").Value;

            BeaconRework.healCooldown = SurvivorCfg.Bind(captainString, "Beacon: Healing - Enable Cooldown", true, "Allow this beacon to be re-used on a cooldown.").Value;

            BeaconRework.hackCooldown = SurvivorCfg.Bind(captainString, "Beacon: Hack - Enable Cooldown", false, "Allow this beacon to be re-used on a cooldown.").Value;
            BeaconRework.hackChanges = SurvivorCfg.Bind(captainString, "Beacon: Hack - Enable Changes", false, "Enable changes to the effect of this beacon.").Value;
            BeaconRework.hackDisable = SurvivorCfg.Bind(captainString, "Beacon: Hack - Disable", false, "Removes this Beacon from the game.").Value;

            BeaconRework.shockCooldown = SurvivorCfg.Bind(captainString, "Beacon: Shocking - Enable Cooldown", true, "Allow this beacon to be re-used on a cooldown.").Value;
            BeaconRework.shockChanges = SurvivorCfg.Bind(captainString, "Beacon: Shocking - Enable Changes", true, "Enable changes to the effect of this beacon.").Value;

            BeaconRework.resupplyCooldown = SurvivorCfg.Bind(captainString, "Beacon: Resupply - Enable Cooldown", true, "Allow this beacon to be re-used on a cooldown.").Value;
            BeaconRework.resupplyChanges = SurvivorCfg.Bind(captainString, "Beacon: Resupply - Enable Changes", true, "Enable changes to the effect of this beacon.").Value;

            CaptainCore.beaconRework = SurvivorCfg.Bind(captainString, "Beacon Changes", true, "Beacons can be replaced on a cooldown and reworks Supply and Hack beacons. Disabling this disables all beacon-related changes.").Value;
            CaptainCore.beaconRework = CaptainCore.beaconRework
                && (BeaconRework.healCooldown
                || BeaconRework.hackCooldown|| BeaconRework.hackChanges
                || BeaconRework.shockCooldown || BeaconRework.shockChanges
                || BeaconRework.resupplyChanges || BeaconRework.resupplyCooldown);
            BeaconRework.CaptainDeployableManager.allowLysateStack = SurvivorCfg.Bind(captainString, "Beacon Changes - Infinite Lysate Cell Stacking", false, "If Beacon Changes are enabled, allow stocks to be infinitely increased with Lysate Cells.").Value;

            Bandit2Core.enabled = SurvivorCfg.Bind(banditString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            Bandit2Core.modifyStats = SurvivorCfg.Bind(banditString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;

            PersistentDesperado.enabled = SurvivorCfg.Bind(banditString, "Desperado Persist", false, "Desperado stacks are weaker but last between stages.").Value;
            PersistentDesperado.damagePerBuff = SurvivorCfg.Bind(banditString, "Desperado Persist - Damage Multiplier", 0.01f, "Revolver damage multiplier per Desperado stack if Desperado Persist is enabled.").Value;

            BackstabRework.enabled = SurvivorCfg.Bind(banditString, "Backstab Changes", true, "Backstabs minicrit for 50% bonus damage and crit chance is converted to crit damage.").Value;
            BuffHemorrhage.enableProcs = SurvivorCfg.Bind(banditString, "Hemmorrhage - Enable Procs", true, "Hemmorrhage damage has a low proc coefficient.").Value;
            BuffHemorrhage.bypassArmor = SurvivorCfg.Bind(banditString, "Hemmorrhage - Ignore Armor", false, "Hemmorrhage ignores positive armor.").Value;

            //BuffHemorrhage.enableCrit = SurvivorCfg.Bind(banditString, "Hemmorrhage - Count as Crit", true, "Hemmorrhagedamage counts as crits.").Value;
            BuffHemorrhage.enableCrit = false;  //hitsound is obnoxious

            Bandit2Core.burstChanges = SurvivorCfg.Bind(banditString, "Burst Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.blastChanges = SurvivorCfg.Bind(banditString, "Blast Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.noKnifeCancel = SurvivorCfg.Bind(banditString, "Knife While Reloading", true, "Knife skills can be used without interrupting your reload.").Value;
            Bandit2Core.knifeChanges = SurvivorCfg.Bind(banditString, "Serrated Dagger Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.knifeThrowChanges = SurvivorCfg.Bind(banditString, "Serrated Shiv Changes", true, "Enable changes to this skill.").Value;
            Bandit2Core.utilityFix = SurvivorCfg.Bind(banditString, "Smokebomb Fix", true, "Fixes various bugs with Smokebomb.").Value;
            Bandit2Core.specialRework = SurvivorCfg.Bind(banditString, "Special Rework", true, "Makes Resets/Desperado a selectable passive and adds a new Special skill.").Value;

            VoidFiendCore.enabled = SurvivorCfg.Bind(voidFiendString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            VoidFiendCore.corruptOnKill = SurvivorCfg.Bind(voidFiendString, "Corruption on Kill", true, "Gain Corruption on kill. Lowers passive Corruption gain and Corrupted form duration.").Value;
            VoidFiendCore.corruptMeterTweaks = SurvivorCfg.Bind(voidFiendString, "Corruption Meter Tweaks", true, "Faster decay, slower passive buildup. Corrupted Suppress can be used as long as you have the HP for it. Meant to be used with Corrupt on Kill.").Value;
            VoidFiendCore.noCorruptCrit = SurvivorCfg.Bind(voidFiendString, "No Corruption on Crit", true, "Disables Corruption gain on crit.").Value;
            VoidFiendCore.noCorruptHeal = SurvivorCfg.Bind(voidFiendString, "No Corruption loss on Heal", true, "Disables Corruption loss on heal.").Value;
            VoidFiendCore.removeCorruptArmor = SurvivorCfg.Bind(voidFiendString, "No Corrupt Mode Bonus Armor", true, "Disables bonus armor while Corrupted.").Value;
            VoidFiendCore.secondaryMultitask = SurvivorCfg.Bind(voidFiendString, "Secondary Multitasking", true, "Drown and Suppress can be fired while charging Flood.").Value;
            UtilityFallImmune.enabled = SurvivorCfg.Bind(voidFiendString, "Trespass Changes", true, "Enable changes to this skill.").Value;

            SeekerCore.enabled = SurvivorCfg.Bind(seekerString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            SeekerCore.noSelfRevive = SurvivorCfg.Bind(seekerString, "No Self Revive", true, "Seeker no longer gains a self-revive at 7 meditate stacks.").Value;
            SeekerCore.unseenHandScalesDamage = SurvivorCfg.Bind(seekerString, "Unseen Hand scales damage.", true, "Unseen Hand scales damage instead of healing.").Value;

            FalseSonCore.enabled = SurvivorCfg.Bind(falseSonString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            FalseSonCore.modifyBaseStats = SurvivorCfg.Bind(falseSonString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;
            FalseSonCore.increaseGrowthCost = SurvivorCfg.Bind(falseSonString, "Increase Growth Cost", true, "Increases health required per growth stack to compensate for the health item buffs in this mod.").Value;
            FalseSonCore.modifyPassive = SurvivorCfg.Bind(falseSonString, "Modify Passive", true, "Increases passive armor and reduces passive health regen.").Value;
            FalseSonCore.buffLaser = SurvivorCfg.Bind(falseSonString, "Laser of the Father Changes", true, "Enable changes to this skill.").Value;

            if (!SurvivorsCore.enabled)
            {
                Bandit2Core.enabled = false;
                CaptainCore.enabled = false;
                CommandoCore.enabled = false;
                CrocoCore.enabled = false;
                VoidFiendCore.enabled = false;
                EngiCore.enabled = false;
                HuntressCore.enabled = false;
                LoaderCore.enabled = false;
                MageCore.enabled = false;
                MercCore.enabled = false;
                ToolbotCore.enabled = false;
                TreebotCore.enabled = false;
                SeekerCore.enabled = false;
                FalseSonCore.enabled = false;
            }
        }

        private static void ConfigMonsters()
        {
            MonsterCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Monsters.cfg"), true);

            EnemiesCore.infernoCompat = MonsterCfg.Bind("Compatibility", "Inferno Compatibility", true, "Disable certain changes that may conflict with Inferno.").Value;

            MithrixCore.enabled = MonsterCfg.Bind(monsterMithrixString, "Enable Changes", true, "Enable changes to this monster.").Value;
            MithrixTargetPrioritization.enabled = MonsterCfg.Bind(monsterMithrixString, "Prioritize Players", true, "Mithrix always tries to prioritize targeting players when possible. May result in him completely ignoring Engi Turrets.").Value;
            MithrixFreezeImmune.enabled = MonsterCfg.Bind(monsterMithrixString, "Freeze Immunity", false, "Mithrix cannot be frozen.").Value;
            MithrixHealthBuff.enabled = MonsterCfg.Bind(monsterMithrixString, "Health Buff", true, "Buffs Mithrix's health.").Value;
            MithrixDebuffResist.enabled = MonsterCfg.Bind(monsterMithrixString, "Debuff Resistance", true, "Attack Speed and Move Speed reduction are less effective against Mithrix.").Value;

            VoidlingCore.enabled = MonsterCfg.Bind(monsterVoidlingString, "Enable Changes", true, "Enable changes to this monster.").Value;
            VoidlingStats.modifyHP = MonsterCfg.Bind(monsterVoidlingString, "Reduce HP", true, "Reduces Voidling HP.").Value;
            VoidlingTargetPrioritization.enabled = MonsterCfg.Bind(monsterVoidlingString, "Prioritize Players", true, "This monster always tries to prioritize targeting players when possible.").Value;
            
            Jellyfish.enabled = MonsterCfg.Bind(monsterString, "Jellyfish", true, "Enable changes to this monster.").Value;
            Imp.enabled = MonsterCfg.Bind(monsterString, "Imp", true, "Enable changes to this monster.").Value;
            HermitCrab.enabled = MonsterCfg.Bind(monsterString, "Hermit Crab", true, "Enable changes to this monster.").Value;
            Lemurian.enabled = MonsterCfg.Bind(monsterString, "Lemurian", true, "Enable changes to this monster.").Value;

            Mushrum.enabled = MonsterCfg.Bind(monsterString, "Mini Mushrum", true, "Enable changes to this monster.").Value;
            Bison.enabled = MonsterCfg.Bind(monsterString, "Bighorn Bison", true, "Enable changes to this monster.").Value;

            GreaterWisp.enabled = MonsterCfg.Bind(monsterString, "Greater Wisp", true, "Enable changes to this monster.").Value;

            Parent.enabled = MonsterCfg.Bind(monsterString, "Parent", true, "Enable changes to this monster.").Value;

            LunarExploder.enabled = MonsterCfg.Bind(monsterString, "Lunar Exploder", true, "Enable changes to this monster.").Value;
            LunarWisp.enabled = MonsterCfg.Bind(monsterString, "Lunar Wisp", true, "Enable changes to this monster.").Value;
            LunarWisp.removeHitscan = MonsterCfg.Bind(monsterString, "Lunar Wisp - Remove Hitscan", true, "Changes the Lunar Wisp attack from hitscan to projectile.").Value;
            LunarWisp.disableProjectileOnKill = MonsterCfg.Bind(monsterString, "Lunar Wisp - Disable Projectile On-Kill Effects", true, "Prevents on-kill effects from proccing when this monster's projectiles are shot down.").Value;

            Titan.enabled = MonsterCfg.Bind(monsterString, "Stone Titan", true, "Enable changes to this monster.").Value;
            BeetleQueen.enabled = MonsterCfg.Bind(monsterString, "Beetle Queen", true, "Enable changes to this monster.").Value;
            Vagrant.enabled = MonsterCfg.Bind(monsterString, "Wandering Vagrant", true, "Enable changes to this monster.").Value;
            Vagrant.disableProjectileOnKill = MonsterCfg.Bind(monsterString, "Wandering Vagrant - Disable Projectile On-Kill Effects", true, "Prevents on-kill effects from proccing when this monster's projectiles are shot down.").Value;
            Gravekeeper.disableProjectileOnKill = MonsterCfg.Bind(monsterString, "Grovetender - Disable Projectile On-Kill Effects", true, "Prevents on-kill effects from proccing when this monster's projectiles are shot down.").Value;
            SCU.enabled = MonsterCfg.Bind(monsterString, "Solus Control Unit", true, "Enable changes to this monster.").Value;
            Worm.enabled = MonsterCfg.Bind(monsterString, "Magma/Overloading Worm", true, "Enable changes to this monster.").Value;

            Aurelionite.enabled = MonsterCfg.Bind(monsterString, "Aurelionite", true, "Enable changes to this monster.").Value;
            Aurelionite.modifyStats = MonsterCfg.Bind(monsterString, "Aurelionite Stats", true, "Modify stats. (Separate from Aurelionite option)").Value;
            AWU.enabled = MonsterCfg.Bind(monsterString, "Alloy Worship Unit", true, "Enable changes to this monster.").Value;

            BlindPest.enabled = MonsterCfg.Bind(monsterString, "Blind Pest", true, "Enable changes to this monster.").Value;
            XiConstruct.enabled = MonsterCfg.Bind(monsterString, "Xi Construct", true, "Enable changes to this monster.").Value;

            VoidInfestor.enabled = MonsterCfg.Bind(monsterString, "Void Infestor", true, "Enable changes to this monster.").Value;
            VoidInfestor.noVoidAllies = MonsterCfg.Bind(monsterString, "Void Infestor - No Ally Infestation", true, "Void Infestors can't possess allies.").Value;

            VoidReaver.enabled = MonsterCfg.Bind(monsterString, "Void Reaver", true, "Enable changes to this monster.").Value;

            Child.enabled = MonsterCfg.Bind(monsterString, "Child", true, "Enable changes to this monster.").Value;
            Scorchling.enabled = MonsterCfg.Bind(monsterString, "Scorch Wurm", true, "Enable changes to this monster.").Value;
        }

        private static void ConfigSpawnpools()
        {
            SpawnpoolCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Spawnpools.cfg"), true);

            EnemiesCore.spawnpoolDLCReplacementFix = SpawnpoolCfg.Bind("General", "Fix DLC Enemy Replacements (Breaks spawnpools without SotV)", true, "Restores SotV enemy replacements.").Value;

            TitanicPlains.enabled = SpawnpoolCfg.Bind("Stage 1", "Titanic Plains", true, "Enable spawnpool changes on this stage.").Value;
            DistantRoost.enabled = SpawnpoolCfg.Bind("Stage 1", "Distant Roost", true, "Enable spawnpool changes on this stage.").Value;
            SnowyForest.enabled = SpawnpoolCfg.Bind("Stage 1", "Siphoned Forest", true, "Enable spawnpool changes on this stage.").Value;
            Lakes.enabled = SpawnpoolCfg.Bind("Stage 1", "Verdant Falls", true, "Enable spawnpool changes on this stage.").Value;

            VillageNight.enabled = SpawnpoolCfg.Bind("Stage 1", "Disturbed Impact", true, "Enable spawnpool changes on this stage.").Value;
            LakesNight.enabled = SpawnpoolCfg.Bind("Stage 1", "Viscous Falls", true, "Enable spawnpool changes on this stage.").Value;

            Wetland.enabled = SpawnpoolCfg.Bind("Stage 2", "Wetland Aspect", true, "Enable spawnpool changes on this stage.").Value;
            GooLake.enabled = SpawnpoolCfg.Bind("Stage 2", "Abandoned Aqueduct", true, "Enable spawnpool changes on this stage.").Value;

            DampCaveSimple.enabled = SpawnpoolCfg.Bind("Stage 4", "Abyssal Depths", true, "Enable spawnpool changes on this stage.").Value;
            StadiaJungle.enabled = SpawnpoolCfg.Bind("Stage 4", "Stadia Jungle", true, "Enable spawnpool changes on this stage.").Value;
            SirensCall.enabled = SpawnpoolCfg.Bind("Stage 4", "Sirens Call", true, "Enable spawnpool changes on this stage.").Value;

            SkyMeadow.enabled = SpawnpoolCfg.Bind("Stage 5", "Sky Meadow", true, "Enable spawnpool changes on this stage.").Value;
            HelminthRoost.enabled = SpawnpoolCfg.Bind("Stage 5", "Helminth Hatchery", true, "Enable spawnpool changes on this stage.").Value;
        }
    }
}
