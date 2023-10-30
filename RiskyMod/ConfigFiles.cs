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
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(FireSelect.scrollSelection));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(FireSelect.swapButton));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(FireSelect.prevButton));

            if (SurvivorsCore.enabled)
            {
                if (CaptainCore.enabled && CaptainCore.enablePrimarySkillChanges)
                {
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(CaptainFireModes.enabled));
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(CaptainFireModes.defaultButton));
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(CaptainFireModes.autoButton));
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(CaptainFireModes.chargeButton));
                }

                if (Bandit2Core.enabled && (Bandit2Core.blastChanges || Bandit2Core.burstChanges))
                {
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(BanditFireModes.enabled));
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(BanditFireModes.defaultButton));
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(BanditFireModes.spamButton));
                }

                if (MageCore.enabled)
                {
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(MageCore.ionSurgeMovementScaling));

                    if (MageCore.iceWallRework || MageCore.enableFireUtility)
                    {
                        ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(MageCore.utilitySelfKnockback));
                    }
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(MageCore.flamethrowerSprintCancel));
                }

                if (MercCore.enabled)
                {
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(MercCore.m1ComboFinishTweak));
                }

                if (VoidFiendCore.enabled)
                {
                    ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(UtilityMoveSpeedScaling.disableScaling));
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
            ShieldGating.enabled = GeneralCfg.Bind(gameMechString, "Shield Gating", true, "Shields gate against HP damage.").Value;
            TrueOSP.enabled = GeneralCfg.Bind(gameMechString, "True OSP", true, "Makes OSP work against multihits.").Value;
            AIBlacklistItems.enabled = GeneralCfg.Bind(gameMechString, "Expanded AI Blacklist", true, "Adds extra items to the AI Blacklist by default.").Value;
            BarrierDecay.enabled = GeneralCfg.Bind(gameMechString, "Barrier Decay", true, "Barrier decays slower at low barrier values.").Value;
            TeleExpandOnBossKill.enabled = GeneralCfg.Bind(gameMechString, "Tele Expand on Boss Kill", true, "Teleporter expands to cover the whole map when the boss is killed.").Value;
            TeleExpandOnBossKill.enableDuringEclipse = GeneralCfg.Bind(gameMechString, "Tele Expand on Boss Kill - Enable During Eclispe", false, "Enables teleporter expansion when playing on Eclipse 2+.").Value;

            TeleChargeDuration.enabled = GeneralCfg.Bind(gameMechString, "Tele Charge Duration Increase", false, "Increases teleporter charge duration from 90s to 120s like RoR1.").Value;

            SmallHoldoutCharging.enabled = GeneralCfg.Bind(gameMechString, "Small Holdout Charging", true, "Void/Moon Holdouts charge at max speed as long as 1 player is charging.").Value;

            //Run Scaling
            CombatDirectorMultiplier.directorCreditMultiplier = GeneralCfg.Bind(scalingString, "Combat Director Credit Multiplier", 1.4f, "Multiply Combat Director credits by this amount. Set to 1 to disable").Value;
            CombatDirectorMultiplier.enabled = CombatDirectorMultiplier.directorCreditMultiplier != 1f;

            MonsterGoldRewards.enabled = GeneralCfg.Bind(scalingString, "Gold Scaling Tweaks", true, "Lowers gold earn rate scaling. Mainly makes a difference when looping.").Value;
            if (!MonsterGoldRewards.enabled)
            {
                MonsterGoldRewards.linearize = false;
                MonsterGoldRewards.scaleToInitialDifficulty = false;
                if (CombatDirectorMultiplier.enabled)
                {
                    MonsterGoldRewards.enabled = true;
                }
            }

            ModdedScaling.enabled = GeneralCfg.Bind(scalingString, "Scaling: Modded Scaling", true, "Lowers the effect of playercount on difficulty scaling. Geared towards large lobbies.").Value;
            LinearScaling.enabled = GeneralCfg.Bind(scalingString, "Scaling: Linear Scaling", false, "Makes difficulty scaling linear like in RoR1. Requires Modded Scaling to be enabled.").Value;

            NoLevelupHeal.enabled = GeneralCfg.Bind(scalingString, "No Levelup Heal", true, "Monsters don't gain HP when leveling up.").Value;
            RemoveLevelCap.enabled = GeneralCfg.Bind(scalingString, "Increase Monster Level Cap", true, "Increases Monster Level Cap.").Value;
            RemoveLevelCap.maxLevel = GeneralCfg.Bind(scalingString, "Increase Monster Level Cap - Max Level", 9999f, "Maximum monster level if Increase Monster Level Cap is enabled.").Value;
            SceneDirectorMonsterRewards.enabled = GeneralCfg.Bind(scalingString, "SceneDirector Monster Rewards", true, "Monsters that spawn with the map now give the same rewards as teleporter monsters.").Value;

            LoopTeleMountainShrine.enabled = GeneralCfg.Bind(scalingString, "Loop Teleporter Boss Credits", true, "Teleporter Boss director credits increase by 1 Mountain Shrine every loop.").Value;
            NoBossRepeat.enabled = GeneralCfg.Bind(scalingString, "No Teleporter Boss Repeat", true, "Lowers the chance of the same teleporter boss being selected within a loop.").Value;

            //Allies
            AlliesCore.normalizeDroneDamage = GeneralCfg.Bind(allyString, "Normalize Drone Damage", true, "Normalize drone damage stats so that they perform the same when using Spare Drone Parts.").Value;
            AllyScaling.preventExecute = GeneralCfg.Bind(allyString, "No Execute", true, "Allies are immune to being executed by Freeze and similar effects.").Value;
            AllyScaling.noVoidDeath = GeneralCfg.Bind(allyString, "No Void Death", true, "Allies are immune to Void implosions.").Value;
            NoVoidDamage.enabled = GeneralCfg.Bind(allyString, "No Void Damage", true, "Allies take no damage from Void fog.").Value;
            AllyScaling.noOverheat = GeneralCfg.Bind(allyString, "No Overheat", true, "Allies are immune to Grandparent Overheat.").Value;
            SuperAttackResist.enabled = GeneralCfg.Bind(allyString, "Superattack Resistance", true, "Allies take less damage from superattacks like Vagrant Novas.").Value;
            DotZoneResist.enabled = GeneralCfg.Bind(allyString, "Damage Zone Resistance", true, "Allies take less damage from Mushrums gas and Lunar Exploder fire.").Value;
            StricterLeashing.enabled = GeneralCfg.Bind(allyString, "Stricter Leashing", true, "Reduces the minimum distance required for Allies to teleport to you.").Value;
            AlliesCore.beetleGlandDontRetaliate = GeneralCfg.Bind(allyString, "Queens Gland Guards Dont Retaliate", true, "Queens Gland Guards will not fight back if hurt by their owners.").Value;
            MegaDrone.allowRepair = GeneralCfg.Bind(allyString, "TC280 - Enable Repairs", true, "TC280s can be repaired after being destroyed.").Value;
            GunnerTurret.allowRepair = GeneralCfg.Bind(allyString, "Gunner Turret - Enable Repairs", true, "Gunner Turrets can be repaired after being destroyed.").Value;
            GunnerTurret.teleportWithPlayer = GeneralCfg.Bind(allyString, "Gunner Turret - Teleport with Player", true, "Gunner Turrets are teleported to the player when starting the Teleporter event.").Value;
            CheaperRepairs.enabled = GeneralCfg.Bind(allyString, "Cheaper Drone Repairs", true, "Repairing drones is cheaper.").Value;
            AlliesCore.changeScaling = GeneralCfg.Bind(allyString, "Scaling Changes", false, "Ally HP and Damage scales so that they always perform the same on every stage.").Value;
            AlliesCore.buffRegen = GeneralCfg.Bind(allyString, "Regen Changes", true, "Most allies regen to full HP in a fixed amount of time.").Value;

            AlliesCore.ChenChillDroneCompat = GeneralCfg.Bind(allyString, "Compatibility - Chill Drone", true, "Enables ally changes on allies from this mod.").Value;
            AlliesCore.ChenQbDroneCompat = GeneralCfg.Bind(allyString, "Compatibility - Qb Drone", true, "Enables ally changes on allies from this mod.").Value;
            AlliesCore.ChenGradiusCompat = GeneralCfg.Bind(allyString, "Compatibility - Gradius Mod", true, "Enables ally changes on allies from this mod.").Value;
            AlliesCore.SS2Compat = GeneralCfg.Bind(allyString, "Compatibility - Starstorm 2", true, "Enables ally changes on allies from this mod.").Value;
            AlliesCore.SpikestripCompat = GeneralCfg.Bind(allyString, "Compatibility - Spikestrip", true, "Enables ally changes on allies from this mod.").Value;
            AlliesCore.TinkersSatchelCompat = GeneralCfg.Bind(allyString, "Compatibility - Tinkers Satchel", true, "Enables ally changes on allies from this mod.").Value;
            AlliesCore.MoreDronesCompat = GeneralCfg.Bind(allyString, "Compatibility - MoreDrones", true, "Enables ally changes on allies from this mod.").Value;

            //Interactables
            ShrineCombatItems.enabled = GeneralCfg.Bind(interactString, "Shrine of Combat Drops Items", true, "Shrine of Combat drops items for the team on completion.").Value;
            BloodShrineMinReward.enabled = GeneralCfg.Bind(interactString, "Shrine of Blood Minimum Reward", true, "Shrine of Blood always gives at least enough money to buy a small chest.").Value;

            SpawnLimits.maxMountainShrines = GeneralCfg.Bind(interactString, "Max Shrines of the Mountain", 5, "Limit how many Mountain Shrines can spawn on 1 stage. Set to negative for no limit.").Value;
            SpawnLimits.maxCombatShrines = GeneralCfg.Bind(interactString, "Max Shrines of Combat", 3, "Limit how many Combat Shrines can spawn on 1 stage. Set to negative for no limit.").Value;
            SpawnLimits.maxVoidSeeds = GeneralCfg.Bind(interactString, "Max Void Seeds", 1, "Limit how many Void Seeds can spawn on 1 stage. Vanilla is 3. Set to negative for no limit.").Value;

            ScaleCostWithPlayerCount.scaleCombatShrine = GeneralCfg.Bind(interactString, "Shrine of Combat Director Credit Scaling", true, "Increase director credit cost of this interactable based on the amount of players.").Value;
            ScaleCostWithPlayerCount.scaleMountainShrine = GeneralCfg.Bind(interactString, "Shrine of the Mountain Director Credit Scaling", true, "Increase director credit cost of this interactable based on the amount of players.").Value;

            if (SpawnLimits.maxMountainShrines < 0 && SpawnLimits.maxCombatShrines < 0 && SpawnLimits.maxVoidSeeds == 3) SpawnLimits.enabled = false;

            //Artifacts
            BulwarksAmbry.enabled = GeneralCfg.Bind(artifactString, "Bulwarks Ambry Tweaks", true, "Increase key drop chance and guarantee key drops after a certain amount of kills.").Value;
            VengeanceVoidTeam.enabled = GeneralCfg.Bind(artifactString, "Vengeance - Void Team", true, "Vengeance Doppelgangers are considered part of the Void Team.").Value;
            FixVengeanceLeveling.enabled = GeneralCfg.Bind(artifactString, "Vengeance - Fix Levels", true, "Fix Vengeance Doppelgangers not leveling up.").Value;
            VengeancePercentHeal.enabled = GeneralCfg.Bind(artifactString, "Vengeance - Reduce Percent Heals", true, "Vengeance Doppelgangers receive reduced healing from percent-based healing effects.").Value;
            NoVengeanceFallDamage.enabled = GeneralCfg.Bind(artifactString, "Vengeance - Disable Fall Damage", true, "Vengeance Doppelgangers are immune to fall damage.").Value;
            EnigmaBlacklist.enabled = GeneralCfg.Bind(artifactString, "Enigma Blacklist", true, "Blacklist Lunars and Recycler from the Artifact of Enigma.").Value;
            Sacrifice.enabled = GeneralCfg.Bind(artifactString, "Sacrifice - No Drop Chance Scaling", true, "Increases drop chance when not using Swarms and prevents drop chance from increasing as the run progresses.").Value;

            //Void Fields
            VoidFields.ModifyHoldout.enabled = GeneralCfg.Bind(voidFieldsString, "Modify Holdout Zone", true, "Increase radius and reduces charge duration.").Value;
            VoidFields.ReduceHoldoutCount.enabled = GeneralCfg.Bind(voidFieldsString, "Less Cells", true, "Reduces Cell count from 9 to 5 and speed up enemy progression.").Value;

            //VoidFields.FogRework.enabled = GeneralCfg.Bind(voidFieldsString, "Fog Rework", true, "Void Fog is only active during holdouts like on Void Locus.").Value;
            VoidFields.FogRework.enabled = false;

            //Moon
            Moon.ModifyHoldout.enabled = GeneralCfg.Bind(moonString, "Modify Holdout Zone", true, "Increase radius and reduces charge duration.").Value;
            LessPillars.enabled = GeneralCfg.Bind(moonString, "Reduce Pillar Count", true, "Reduce the amount of pillars required to activate the jump pads.").Value;
            Moon.PillarsDropItems.enabled = GeneralCfg.Bind(moonString, "Pillars Drop Items", true, "Pillars drop items for the team when completed.").Value;
            Moon.PillarsDropItems.whiteChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Common Chance", 50f, "Chance for Pillars to drop Common Items.").Value;
            Moon.PillarsDropItems.greenChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Uncommon Chance", 40f, "Chance for Pillars to drop Uncommon Items.").Value;
            Moon.PillarsDropItems.redChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Legendary Chance", 10f, "Chance for Pillars to drop Legendary Items.").Value;
            Moon.PillarsDropItems.lunarChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Lunar Chance", 0f, "Chance for nonlegendary Pillar drops to be overwritten with a random Lunar item.").Value;
            Moon.PillarsDropItems.pearlOverwriteChance = GeneralCfg.Bind(moonString, "Pillars Drop Items - Pearl Override Chance", 15f, "Chance for nonlegendary Pillar drops to be overwritten with a Pearl.").Value;

            //Void Locus
            RemoveFog.enabled = GeneralCfg.Bind(voidLocusString, "Remove Fog", false, "Removes Void Fog from the map.").Value;
            VoidLocus.FogDamage.enabled = GeneralCfg.Bind(voidLocusString, "Tweak Fog Damage", true, "Void Fog damage ticks at a constant rate, instead of ramping up like in Simulacrum.").Value && !RemoveFog.enabled;
            VoidLocus.ModifyHoldout.enabled = GeneralCfg.Bind(voidLocusString, "Modify Holdout Zone", true, "Increase radius and reduces charge duration.").Value;
            VoidLocus.PillarsDropItems.enabled = GeneralCfg.Bind(voidLocusString, "Signals Drop Items", true, "Void Signals drop items for the team when completed.").Value;
            VoidLocus.PillarsDropItems.usePotential = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Drop Void Potential", true, "Void Signals drop Void Potentials. Overwrite chance settings.").Value;
            VoidLocus.PillarsDropItems.whiteChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Common Chance", 45f, "Chance for Signals to drop Common Items.").Value;
            VoidLocus.PillarsDropItems.greenChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Uncommon Chance", 40f, "Chance for Signals to drop Uncommon Items.").Value;
            VoidLocus.PillarsDropItems.redChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Legendary Chance", 5f, "Chance for Signals to drop Legendary Items.").Value;
            VoidLocus.PillarsDropItems.voidChance = GeneralCfg.Bind(voidLocusString, "Signals Drop Items - Void Chance", 10f, "Chance for Signals to drop Void Items.").Value;

            //Misc
            BetterProjectileTracking.enabled = GeneralCfg.Bind(miscString, "Better Projectile Homing", true, "Homing projectiles target based on angle, instead of distance + angle.").Value;
            FixSlayer.enabled = GeneralCfg.Bind(miscString, "Fix Slayer Procs", true, "Bandit/Acrid bonus damage to low hp effect now applies to procs.").Value;
            CloakBuff.enabled = GeneralCfg.Bind(miscString, "Cloak Buff", true, "Increases delay between position updates while cloaked.").Value;
            Shock.enabled = GeneralCfg.Bind(miscString, "No Shock Interrupt", true, "Shock is no longer interrupted by damage.").Value;
            FreezeChampionExecute.enabled = GeneralCfg.Bind(miscString, "Freeze Executes Bosses", true, "Freeze counts as a debuff and can execute bosses at 15% HP.").Value;
            NerfVoidtouched.enabled = GeneralCfg.Bind(miscString, "Nerf Voidtouched", true, "Replaces Voidtouched Collapse with Nullify.").Value;
            PlayerControlledMonsters.enabled = GeneralCfg.Bind(miscString, "Player-Controlled Monster Tweaks", true, "Gives players health regen + armor when playing as monsters via mods.").Value;
            ItemOutOfBounds.enabled = GeneralCfg.Bind(miscString, "Item Out of Bounds Teleport", true, "Items that fall out of bounds get teleported back.").Value;
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
            Bungus.enabled = ItemCfg.Bind(commonString, "Bustling Fungus", true, itemConfigDescString).Value;
            CautiousSlug.enabled = ItemCfg.Bind(commonString, "Cautious Slug", true, itemConfigDescString).Value;
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

            Pennies.disableInBazaar = ItemCfg.Bind(commonString, "Roll of Pennies - Disable in Bazaar", true, "This item does not give gold in the Bazaar Between Time.").Value;


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
            HarvesterScythe.enabled = ItemCfg.Bind(uncommonString, "Harvesters Scythe", false, "Reworks this item to give Crit Chance + Regen on kill.").Value;
            Infusion.enabled = ItemCfg.Bind(uncommonString, "Infusion", true, itemConfigDescString).Value;
            Infusion.useBuff = ItemCfg.Bind(uncommonString, "Infusion - Show Buff on HUD", true, "HP gained from Infusion is displayed as a buff on your HUD.").Value;
            LeechingSeed.enabled = ItemCfg.Bind(uncommonString, "Leeching Seed", true, itemConfigDescString).Value;
            Daisy.enabled = ItemCfg.Bind(uncommonString, "Lepton Daisy", true, itemConfigDescString).Value;

            Guillotine.enabled = ItemCfg.Bind(uncommonString, "Old Guillotine", true, itemConfigDescString).Value;
            Guillotine.reduceVFX = ItemCfg.Bind(uncommonString, "Old Guillotine - Reduce VFX", true, "Reduce how often this item's VFX shows up.").Value;

            GhorsTome.enabled = ItemCfg.Bind(uncommonString, "Ghors Tome", true, itemConfigDescString).Value;
            GhorsTome.disableInBazaar = ItemCfg.Bind(uncommonString, "Ghors Tome - Disable in Bazaar", true, "This item does not give gold in the Bazaar Between Time.").Value;

            Razorwire.enabled = ItemCfg.Bind(uncommonString, "Razorwire", true, itemConfigDescString).Value;
            RedWhip.enabled = ItemCfg.Bind(uncommonString, "Red Whip", true, itemConfigDescString).Value;
            RoseBuckler.enabled = ItemCfg.Bind(uncommonString, "Rose Buckler", true, itemConfigDescString).Value;
            SquidPolyp.enabled = ItemCfg.Bind(uncommonString, "Squid Polyp", true, itemConfigDescString).Value;
            SquidPolyp.ignoreAllyCap = ItemCfg.Bind(uncommonString, "Squid Polyp - Ignore Ally Cap", true, "Squid Polyps ignore the ally cap if changes are enabled.").Value;
            SquidPolyp.scaleCount = ItemCfg.Bind(uncommonString, "Squid Polyp - Stacks Increase Max Squids", false, "Extra stacks allow for more squids to spawn. Will lag in MP.").Value;
            Stealthkit.enabled = ItemCfg.Bind(uncommonString, "Old War Stealthkit", true, itemConfigDescString).Value;
            Ukulele.enabled = ItemCfg.Bind(uncommonString, "Ukulele", true, itemConfigDescString).Value;
            WarHorn.enabled = ItemCfg.Bind(uncommonString, "War Horn", true, itemConfigDescString).Value;
            WillOWisp.enabled = ItemCfg.Bind(uncommonString, "Will-o-the-Wisp", true, itemConfigDescString).Value;

            Shuriken.enabled = ItemCfg.Bind(uncommonString, "Shuriken", true, itemConfigDescString).Value;
        }

        private static void ConfigLegendaryItems()
        {
            Behemoth.enabled = ItemCfg.Bind(legendaryString, "Brilliant Behemoth", true, itemConfigDescString).Value;
            BottledChaos.enabled = ItemCfg.Bind(legendaryString, "Bottled Chaos", true, itemConfigDescString).Value;
            Brainstalks.enabled = ItemCfg.Bind(legendaryString, "Brainstalks", true, itemConfigDescString).Value;
            CeremonialDagger.enabled = ItemCfg.Bind(legendaryString, "Ceremonial Dagger", true, itemConfigDescString).Value;
            Clover.enabled = ItemCfg.Bind(legendaryString, "57 Leaf Clover", false, "Caps how high the Luck stat can go.").Value;
            Clover.luckCap = ItemCfg.Bind(legendaryString, "57 Leaf Clover - Max Luck", 1f, "Maximum Luck value players can reach. Extra Luck is converted to stat boosts.").Value;
            FrostRelic.enabled = ItemCfg.Bind(legendaryString, "Frost Relic", true, itemConfigDescString).Value;
            FrostRelic.removeFOV = ItemCfg.Bind(legendaryString, "Frost Relic - Disable FOV Modifier", true, "Disables FOV modifier.").Value;
            FrostRelic.removeBubble = ItemCfg.Bind(legendaryString, "Frost Relic - Disable Bubble", true, "Disables bubble visuals.").Value;
            HappiestMask.enabled = ItemCfg.Bind(legendaryString, "Happiest Mask", true, itemConfigDescString).Value;
            HappiestMask.scaleCount = ItemCfg.Bind(legendaryString, "Happiest Mask - Stacks Increase Max Ghosts", false, "Extra stacks allow for more ghosts to spawn. Will lag in MP.").Value;
            HappiestMask.noGhostLimit = ItemCfg.Bind(legendaryString, "Happiest Mask - Remove Ghost Limit", false, "Removes the ghost limit at all times. Definitely will lag.").Value;
            HeadHunter.enabled = ItemCfg.Bind(legendaryString, "Wake of Vultures", true, itemConfigDescString).Value;
            HeadHunter.perfectedTweak = ItemCfg.Bind(legendaryString, "Wake of Vultures - Perfected Tweak", true, "Perfected Affix gained via Wake of Vultures will not force your health pool to become shields.").Value;
            Headstompers.enabled = ItemCfg.Bind(legendaryString, "H3AD-ST", true, itemConfigDescString).Value;
            Headstompers.useBuff = ItemCfg.Bind(legendaryString, "H3AD-ST - Show Buff on HUD", true, "H3AD-ST cooldown is shown as a buff on your HUD. Disabling this prevents Blast Shower from clearing the item's cooldown.").Value;
            LaserTurbine.enabled = ItemCfg.Bind(legendaryString, "Resonance Disc", true, itemConfigDescString).Value;
            Scorpion.enabled = ItemCfg.Bind(legendaryString, "Symbiotic Scorpion", true, itemConfigDescString).Value;

            SpareDroneParts.enabled = ItemCfg.Bind(legendaryString, "Spare Drone Parts", true, itemConfigDescString).Value;
            SpareDroneParts.ignoreAllyCap = ItemCfg.Bind(legendaryString, "Spare Drone Parts - Ignore Ally Cap", true, "Col. Droneman ignores the ally cap if changes are enabled.").Value;

            Aegis.enabled = ItemCfg.Bind(legendaryString, "Aegis", true, itemConfigDescString).Value;

            Tesla.enabled = ItemCfg.Bind(legendaryString, "Unstable Tesla Coil", true, itemConfigDescString).Value;
            Raincoat.enabled = ItemCfg.Bind(legendaryString, "Bens Raincoat", true, itemConfigDescString).Value;
            Raincoat.replaceIcons = ItemCfg.Bind(legendaryString, "Bens Raincoat - Use Modded Icons", false, "Replace the Vanilla buff icons for this item.").Value;
        }

        private static void ConfigVoidItems()
        {
            Dungus.enabled = ItemCfg.Bind(voidString, "Weeping Fungus", true, itemConfigDescString).Value;

            //Only re-enable when there's something that actually works.
            Needletick.enabled = false;
            //Needletick.enabled = ItemCfg.Bind(voidString, "Needletick", true, itemConfigDescString).Value;

            SaferSpaces.enabled = ItemCfg.Bind(voidString, "Safer Spaces", true, itemConfigDescString).Value;

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
            //Currently split into a separate mod. Will likely remain that way
            //Gesture.enabled = ItemCfg.Bind(lunarString, "Gesture of the Drowned", true, itemConfigDescString).Value;
            Gesture.enabled = false;

            BrittleCrown.enabled = ItemCfg.Bind(lunarString, "Brittle Crown", true, itemConfigDescString).Value;
            Meteorite.enabled = ItemCfg.Bind(lunarString, "Glowing Meteorite", true, itemConfigDescString).Value;
            ShapedGlass.enabled = ItemCfg.Bind(lunarString, "Shaped Glass", true, itemConfigDescString).Value;
            Transcendence.enabled = ItemCfg.Bind(lunarString, "Transcendence", true, itemConfigDescString).Value;
            Transcendence.alwaysShieldGate = ItemCfg.Bind(lunarString, "Transcendence - Always Shield Gate", true, "Enables shieldgating on this item even when shield gating is disabled.").Value;
            Visions.enabled = ItemCfg.Bind(lunarString, "Visions of Heresy", true, itemConfigDescString).Value;
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
            Fruit.enabled = ItemCfg.Bind(equipmentString, "Foreign Fruit", true, itemConfigDescString).Value;
            SuperLeech.enabled = ItemCfg.Bind(equipmentString, "Super Massive Leech", true, itemConfigDescString).Value;
            VolcanicEgg.enabled = ItemCfg.Bind(equipmentString, "Volcanic Egg", true, itemConfigDescString).Value;
            Goobo.enabled = ItemCfg.Bind(equipmentString, "Goobo Jr.", true, itemConfigDescString).Value;
            Caffeinator.enabled = ItemCfg.Bind(equipmentString, "Remote Caffeinator: Trigger Pressure Plates", true, "Remote Caffeinator can be used to press the hidden buttons on Abandoned Aqueduct.").Value;
        }

        private static void ConfigFireSelect()
        {
            FireSelect.scrollSelection = SurvivorCfg.Bind(fireSelectString, "Use Scrollwheel", true, "Mouse Scroll Wheel changes firemode");

            FireSelect.swapButton = SurvivorCfg.Bind(fireSelectString, "Next Firemode", KeyboardShortcut.Empty, "Button to swap to the next firemode.");

            FireSelect.prevButton = SurvivorCfg.Bind(fireSelectString, "Previous Firemode", KeyboardShortcut.Empty, "Button to swap to the previous firemode.");

            CaptainFireModes.enabled = SurvivorCfg.Bind(fireSelectString, "Captain: Enable Fire Select", false, "Enable firemode selection for Captain's shotgun (requires primary changes to be enabled).");
            CaptainFireModes.defaultButton = SurvivorCfg.Bind(fireSelectString, "Captain: Swap to Default", KeyboardShortcut.Empty, "Button to swap to the Default firemode.");
            CaptainFireModes.autoButton = SurvivorCfg.Bind(fireSelectString, "Captain: Swap to Auto", KeyboardShortcut.Empty, "Button to swap to the Auto firemode.");
            CaptainFireModes.chargeButton = SurvivorCfg.Bind(fireSelectString, "Captain: Swap to Charged", KeyboardShortcut.Empty, "Button to swap to the Charged firemode.");

            BanditFireModes.enabled = SurvivorCfg.Bind(fireSelectString, "Bandit: Enable Fire Select", false, "Enable firemode selection for Bandit's primaries (requires primary changes to be enabled).");
            BanditFireModes.defaultButton = SurvivorCfg.Bind(fireSelectString, "Bandit: Swap to Default", KeyboardShortcut.Empty, "Button to swap to the Default firemode.");
            BanditFireModes.spamButton = SurvivorCfg.Bind(fireSelectString, "Bandit: Swap to Spam", KeyboardShortcut.Empty, "Button to swap to the Spam firemode.");
        }

        private static void ConfigSurvivors()
        {
            SurvivorCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Survivors.cfg"), true);
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
            ToolbotCore.scrapICBM = SurvivorCfg.Bind(toolbotString, "Scrap Launcher ICBM", true, "Scrap Launcher benefits from Pocket ICBM if Scrap Launcher Changes are enabled.").Value;

            ToolbotCore.sawPhysics = SurvivorCfg.Bind(toolbotString, "Power Saw Physics Changes", true, "Makes Power Saw physics more consistent.").Value;

            ToolbotCore.sawHitbox = SurvivorCfg.Bind(toolbotString, "Power Saw Hitbox Changes", true, "Increases Power Saw hitbox size by 50%.").Value;
            ToolbotCore.sawBarrierOnHit = SurvivorCfg.Bind(toolbotString, "Power Saw Barrier On Hit", true, "Power Saw gives barrier-on-hit.").Value;
            ToolbotCore.enableSecondarySkillChanges = SurvivorCfg.Bind(toolbotString, "Blast Canister Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enableRetoolChanges = SurvivorCfg.Bind(toolbotString, "Retool Changes", true, "Enable changes to this skill.").Value;
            ToolbotCore.enablePowerModeChanges = SurvivorCfg.Bind(toolbotString, "Power Mode Changes", true, "Enable changes to this skill.").Value;

            EngiCore.enabled = SurvivorCfg.Bind(engiString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            PressureMines.enabled = SurvivorCfg.Bind(engiString, "Pressure Mine Changes", true, "Pressure Mines only detonate when fully armed.").Value;
            EngiCore.harpoonRangeTweak = SurvivorCfg.Bind(engiString, "Thermal Harpoon Changes", true, "Increases Thermal Harpoon targeting range.").Value;
            TurretChanges.turretChanges = SurvivorCfg.Bind(engiString, "Stationary Turret Changes", true, "Enable changes to Stationary Turrets.").Value;
            TurretChanges.mobileTurretChanges = SurvivorCfg.Bind(engiString, "Mobile Turret Changes", true, "Enable changes to Mobile Turrets.").Value;

            MageCore.enabled = SurvivorCfg.Bind(mageString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            MageCore.m1AttackSpeed = SurvivorCfg.Bind(mageString, "Primary Attack Speed Scaling", true, "Artificer's primary reload scales with attack speed instead of cooldown.").Value;
            MageCore.modifyFireBolt = SurvivorCfg.Bind(mageString, "Fire Bolt Changes", true, "Enable changes to this skill.").Value;
            MageCore.modifyPlasmaBolt = SurvivorCfg.Bind(mageString, "Plasma Bolt Changes", true, "Enable changes to this skill.").Value;
            MageCore.m2RemoveNanobombGravity = SurvivorCfg.Bind(mageString, "Nanobomb - Remove Gravity", true, "Removes projectile drop from Nanobomb so it behaves like it did pre-1.0 update.").Value;

            MageCore.ionSurgeUtility = SurvivorCfg.Bind(mageString, "Ion Surge - Move to Utility Slot", true, "Moves Ion Surge to the Utility slot.").Value;
            MageCore.ionSurgeMovementScaling = SurvivorCfg.Bind(mageString, "Ion Surge - Movement Scaling", false, "(Client-Side) Ion Surge jump height scales with movement speed.");
            MageCore.ionSurgeShock = SurvivorCfg.Bind(mageString, "Ion Surge - Shock", true, "Ion Surge shocks enemies.").Value;

            MageCore.utilitySelfKnockback = SurvivorCfg.Bind(mageString, "Utility Self Knockback", true, "(Client-Side) Snapfreeze Rework and Blaze Storm apply self-knockback when used midair.");
            MageCore.iceWallRework = SurvivorCfg.Bind(mageString, "Snapfreeze Rework", true, "Snapfreeze can target midair enemies.").Value;
            Survivors.Mage.SkillTweaks.IceWallDefense.enabled = SurvivorCfg.Bind(mageString, "Snapfreeze Defense", true, "Snapfreeze blocks ranged attacks from enemies.").Value;
            MageCore.enableFireUtility = SurvivorCfg.Bind(mageString, "Blaze Storm Utility Skill", false, "Enables this custom skill.").Value;
            MageCore.flamethrowerSprintCancel = SurvivorCfg.Bind(mageString, "Flamethrower - Sprint Cancel", true, "Sprinting cancels Flamethrower.");
            MageCore.flamethrowerRangeExtend = SurvivorCfg.Bind(mageString, "Flamethrower - Increase Range", true, "Increases the range of flamethrower.").Value;
            MageCore.flamethrowerIgniteChance = SurvivorCfg.Bind(mageString, "Flamethrower - Always Ignite", true, "All hits of Flamethrower ignite.").Value;

            MageCore.enableLightningSpecial = SurvivorCfg.Bind(mageString, "Electrocute Special Skill", false, "Enables this custom skill.").Value;

            MercCore.enabled = SurvivorCfg.Bind(mercString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            MercCore.modifyStats = SurvivorCfg.Bind(mercString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;
            MercCore.evisTargetingFix = SurvivorCfg.Bind(mercString, "Eviscerate Targeting Fix", true, "Makes Eviscerate less likely to target teammates.").Value;
            //Should change the name of this to remove (Client-Side), but I don't want to mess with peoples' configs at this point.
            MercCore.m1ComboFinishTweak = SurvivorCfg.Bind(mercString, "M1 Attack Speed Tweak (Client-Side)", true, "(Client-Side) Makes the 3rd hit of Merc's M1 be unaffected by attack speed for use with combo tech.");


            TreebotCore.enabled = SurvivorCfg.Bind(treebotString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            TreebotCore.drillChanges = SurvivorCfg.Bind(treebotString, "DIRECTIVE Drill Changes", true, "Enable changes to this skill.").Value;
            TreebotCore.defaultUtilityHeal = SurvivorCfg.Bind(treebotString, "DIRECTIVE Disperse Healing", true, "DIRECTIVE Disperse heals for 5% HP per target hit.").Value;
            ModifyUtilityForce.enabled = SurvivorCfg.Bind(treebotString, "Utility - Modify Force", true, "Modifies the force of REXs Utilities.").Value;
            TreebotCore.fruitChanges = SurvivorCfg.Bind(treebotString, "DIRECTIVE Harvest Changes", true, "Enable changes to this skill.").Value;
            DropFruitOnHit.enabled = SurvivorCfg.Bind(treebotString, "DIRECTIVE Harvest - Fruit On Hit", false, "Fruiting enemies have a chance to drop fruits when hit.").Value;

            LoaderCore.enabled = SurvivorCfg.Bind(loaderString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            LoaderCore.grappleCancelsSprint = SurvivorCfg.Bind(loaderString, "Secondaries Cancel Sprint", false, "Loader's Grapple cancels sprinting.").Value;
            LoaderCore.shiftCancelsSprint = SurvivorCfg.Bind(loaderString, "Utilities Cancel Sprint", false, "Loader's Big Punches cancel sprinting.").Value;
            LoaderCore.modifyStats = SurvivorCfg.Bind(loaderString, "Modify Base Stats", true, "Enable base stat changes for this survivor.").Value;
            LoaderCore.zapFistChanges = SurvivorCfg.Bind(loaderString, "Thunder Gauntlet Changes", true, "Enable changes to this skill.").Value;
            LoaderCore.slamChanges = SurvivorCfg.Bind(loaderString, "Thunderslam Changes", true, "Enable changes to this skill.").Value;
            LoaderCore.pylonChanges = SurvivorCfg.Bind(loaderString, "M551 Pylon Changes", true, "Enable changes to this skill.").Value;

            CrocoCore.Cfg.enabled = SurvivorCfg.Bind(crocoString, "0. Use Separate Config", false, "Generate a separate config file for more in-depth tuning. Overwrites ALL settings for this survivor.").Value;
            CrocoCore.enabled = SurvivorCfg.Bind(crocoString, "Enable Changes", true, "Enable changes to this survivor. Skill options unavailable due to all the changes being too interlinked.").Value;
            CrocoCore.gameplayRework = SurvivorCfg.Bind(crocoString, "Gameplay Rework", true, "A full rework of Acrid's skills.").Value;
            BiggerMeleeHitbox.enabled = SurvivorCfg.Bind(crocoString, "Extend Melee Hitbox", true, "Extends Acrid's melee hitbox so he can hit Vagrants while standing on top of them.").Value;
            BlightStack.enabled = SurvivorCfg.Bind(crocoString, "Blight Duration Reset", true, "Blight stacks like Bleed.").Value;
            RemovePoisonDamageCap.enabled = SurvivorCfg.Bind(crocoString, "Remove Poison Damage Cap", true, "Poison no longer has a hidden damage cap.").Value;
            BiggerLeapHitbox.enabled = SurvivorCfg.Bind(crocoString, "Extend Leap Collision Box", true, "Acrid's Shift skills have a larger collision hitbox. Damage radius remains the same.").Value;
            ShiftAirControl.enabled = SurvivorCfg.Bind(crocoString, "Leap Air Control", true, "Acrid's Shift skills gain increased air control at high move speeds.").Value;

            if (CrocoCore.Cfg.enabled)
            {
                GenerateCrocoConfig();
            }

            CaptainCore.enabled = SurvivorCfg.Bind(captainString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            CaptainOrbitalHiddenRealms.enabled = SurvivorCfg.Bind(captainString, "Hidden Realm Orbital Skills", true, "Allow Orbital skills in Hiden Realms.").Value;
            Microbots.deletionRestrictions = SurvivorCfg.Bind(captainString, "Defensive Microbots Nerf", true, "Defensive Microbots no longer deletes stationary projectiles like gas clouds and Void Reaver mortars.").Value;
            Microbots.droneScaling = SurvivorCfg.Bind(captainString, "Defensive Microbots Drone Scaling", true, "Defensive Microbots scale with drone count instead of attack speed.").Value;
            CaptainCore.enablePrimarySkillChanges = SurvivorCfg.Bind(captainString, "Enable Primary Skill Changes", true, "Enable primary skill changes for this survivor.").Value;
            CaptainCore.modifyTaser = SurvivorCfg.Bind(captainString, "Power Taser Changes", true, "Enable changes to this skill.").Value;
            CaptainCore.nukeAmmopackNerf = SurvivorCfg.Bind(captainString, "Diablo Strike Ammopack Nerf", true, "Ammopacks only restore half of Diablo Strike's charge. Intended for use with Beacon: Resupply changes.").Value;
            CaptainCore.nukeProc = SurvivorCfg.Bind(captainString, "Diablo Strike Proc Coeficient", true, "Increases Diablo Strike's proc coefficient.").Value;

            BeaconRework.healCooldown = SurvivorCfg.Bind(captainString, "Beacon: Healing - Enable Cooldown", true, "Allow this beacon to be re-used on a cooldown.").Value;

            BeaconRework.hackCooldown = SurvivorCfg.Bind(captainString, "Beacon: Hack - Enable Cooldown", true, "Allow this beacon to be re-used on a cooldown.").Value;
            BeaconRework.hackChanges = SurvivorCfg.Bind(captainString, "Beacon: Hack - Enable Changes", true, "Enable changes to the effect of this beacon.").Value;
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
            BanditSpecialGracePeriod.enabled = SurvivorCfg.Bind(banditString, "Special Grace Period", true, "Special On-kill effects can trigger if an enemy dies shortly after being hit.").Value;
            BanditSpecialGracePeriod.durationLocalUser = SurvivorCfg.Bind(banditString, "Special Grace Period Duration (Host)", 0.5f, "Length in seconds of Special Grace Period.").Value;
            BanditSpecialGracePeriod.durationOnline = SurvivorCfg.Bind(banditString, "Special Grace Period Duration (Client)", 1f, "Length in seconds of Special Grace Period for Online Clients.").Value;

            //Not sure if the Special even works if you disable this.
            if (!BanditSpecialGracePeriod.enabled)
            {
                BanditSpecialGracePeriod.enabled = true;
                BanditSpecialGracePeriod.durationOnline = 0f;
                BanditSpecialGracePeriod.durationLocalUser = 0f;
            }

            DesperadoRework.enabled = SurvivorCfg.Bind(banditString, "Desperado Persist", false, "Desperado stacks are weaker but last between stages.").Value;
            DesperadoRework.damagePerBuff = SurvivorCfg.Bind(banditString, "Desperado Persist - Damage Multiplier", 0.01f, "Revolver damage multiplier per Desperado stack if Desperado Persist is enabled.").Value;

            BackstabRework.enabled = SurvivorCfg.Bind(banditString, "Backstab Changes", true, "Backstabs minicrit for 50% bonus damage and crit chance is converted to crit damage.").Value;
            BuffHemorrhage.ignoreArmor = SurvivorCfg.Bind(banditString, "Hemmorrhage - Ignore Armor", true, "Hemmorrhage damage ignores positive armor.").Value;
            BuffHemorrhage.enableProcs = SurvivorCfg.Bind(banditString, "Hemmorrhage - Enable Procs", true, "Hemmorrhage damage has a low proc coefficient.").Value;

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
            RailgunnerCore.slowFieldChanges = SurvivorCfg.Bind(railgunnerString, "Polar Field Device Changes", true, "Enable changes to this skill.").Value;

            VoidFiendCore.enabled = SurvivorCfg.Bind(voidFiendString, "Enable Changes", true, "Enable changes to this survivor.").Value;
            VoidFiendCore.fasterCorruptTransition = SurvivorCfg.Bind(voidFiendString, "Faster Corrupt Transition", true, "Speed up the corruption transform animation.").Value;
            VoidFiendCore.corruptOnKill = SurvivorCfg.Bind(voidFiendString, "Corruption on Kill", true, "Gain Corruption on kill. Lowers passive Corruption gain and Corrupted form duration.").Value;
            VoidFiendCore.corruptMeterTweaks = SurvivorCfg.Bind(voidFiendString, "Corruption Meter Tweaks", true, "Faster decay, slower passive buildup. Corrupted Suppress can be used as long as you have the HP for it. Meant to be used with Corrupt on Kill.").Value;
            VoidFiendCore.noCorruptCrit = SurvivorCfg.Bind(voidFiendString, "No Corruption on Crit", true, "Disables Corruption gain on crit.").Value;
            VoidFiendCore.noCorruptHeal = SurvivorCfg.Bind(voidFiendString, "No Corruption loss on Heal", true, "Disables Corruption loss on heal.").Value;
            VoidFiendCore.removeCorruptArmor = SurvivorCfg.Bind(voidFiendString, "No Corrupt Mode Bonus Armor", true, "Disables bonus armor while Corrupted.").Value;
            VoidFiendCore.secondaryMultitask = SurvivorCfg.Bind(voidFiendString, "Secondary Multitasking", true, "Drown and Suppress can be fired while charging Flood.").Value;
            VoidFiendCore.modifyCorruptCrush = SurvivorCfg.Bind(voidFiendString, "Corrupted Suppress Changes", true, "Enable changes to this skill.").Value;
            UtilityFallImmune.enabled = SurvivorCfg.Bind(voidFiendString, "Trespass Changes", true, "Enable changes to this skill.").Value;
            UtilityMoveSpeedScaling.disableScaling = SurvivorCfg.Bind(voidFiendString, "Trespass - Disable Move Speed Scaling", false, "(Client-Side) Prevents Trespass from scaling with move speed.");
            VoidFiendCore.removePrimarySpread = SurvivorCfg.Bind(voidFiendString, "Remove Primary Spread", true, "Remove random spread from Drown.").Value;

            ConfigFireSelect();
        }

        private static void ConfigMonsters()
        {
            MonsterCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Monsters.cfg"), true);

            AiTargetFinding.enabled = MonsterCfg.Bind(monsterGeneralString, "AI Targetfinding Tweaks", true, "Enables fullvision and disables LoS checks for AI.").Value;
            EnemiesCore.infernoCompat = MonsterCfg.Bind("Compatibility", "Inferno Compatibility", true, "Disable certain changes that may conflict with Inferno.").Value;

            MonsterFallDamage.enabled = MonsterCfg.Bind(monsterGeneralString, "Lethal Fall Damage", true, "Monsters can die from fall damage.").Value;

            MithrixCore.enabled = MonsterCfg.Bind(monsterMithrixString, "Enable Changes", true, "Enable changes to this monster.").Value;
            MithrixFallImmune.enabled = MonsterCfg.Bind(monsterMithrixString, "Fall Damage Immunity", true, "Mithrix does not take fall damage.").Value;
            MithrixTargetPrioritization.enabled = MonsterCfg.Bind(monsterMithrixString, "Prioritize Players", true, "This monster always tries to prioritize targeting players when possible.").Value;

            //this doesn't seem to actually help much
            //SprintBashAntiTrimp.enabled = MonsterCfg.Bind(monsterMithrixString, "Sprint Bash Anti Trimp", true, "Prevents Mithrix from launching himself into the sky when using his sprint bash on the ramps.").Value;
            SprintBashAntiTrimp.enabled = false;

            VoidlingCore.enabled = MonsterCfg.Bind(monsterVoidlingString, "Enable Changes", true, "Enable changes to this monster.").Value;
            VoidlingStats.modifyHP = MonsterCfg.Bind(monsterVoidlingString, "Reduce HP", true, "Reduces Voidling HP.").Value;
            VoidlingTargetPrioritization.enabled = MonsterCfg.Bind(monsterVoidlingString, "Prioritize Players", true, "This monster always tries to prioritize targeting players when possible.").Value;
            VoidlingFogDamage.enabled = MonsterCfg.Bind(monsterVoidlingString, "Planeterium Void Fog Changes", true, "Makes Planeterium Void Fog behave like the Void Fields.").Value;

            Beetle.enabled = MonsterCfg.Bind(monsterString, "Beetle", true, "Enable changes to this monster.").Value;
            Jellyfish.enabled = MonsterCfg.Bind(monsterString, "Jellyfish", true, "Enable changes to this monster.").Value;
            Imp.enabled = MonsterCfg.Bind(monsterString, "Imp", true, "Enable changes to this monster.").Value;
            HermitCrab.enabled = MonsterCfg.Bind(monsterString, "Hermit Crab", true, "Enable changes to this monster.").Value;
            Lemurian.enabled = MonsterCfg.Bind(monsterString, "Lemurian", true, "Enable changes to this monster.").Value;

            Golem.enabled = MonsterCfg.Bind(monsterString, "Stone Golem", true, "Enable changes to this monster.").Value;
            Mushrum.enabled = MonsterCfg.Bind(monsterString, "Mini Mushrum", true, "Enable changes to this monster.").Value;
            Bison.enabled = MonsterCfg.Bind(monsterString, "Bighorn Bison", true, "Enable changes to this monster.").Value;

            Bronzong.enabled = MonsterCfg.Bind(monsterString, "Brass Contraption", true, "Enable changes to this monster.").Value;
            GreaterWisp.enabled = MonsterCfg.Bind(monsterString, "Greater Wisp", true, "Enable changes to this monster.").Value;

            Parent.enabled = MonsterCfg.Bind(monsterString, "Parent", true, "Enable changes to this monster.").Value;

            LunarExploder.enabled = MonsterCfg.Bind(monsterString, "Lunar Exploder", true, "Enable changes to this monster.").Value;
            LunarGolem.enabled = MonsterCfg.Bind(monsterString, "Lunar Golem", true, "Enable changes to this monster.").Value;
            LunarWisp.enabled = MonsterCfg.Bind(monsterString, "Lunar Wisp", true, "Enable changes to this monster.").Value;
            LunarWisp.reduceSpawnrate = MonsterCfg.Bind(monsterString, "Lunar Wisp - Reduce Spawnrate", true, "Increases the director credit cost of this monster.").Value;
            LunarWisp.removeHitscan = MonsterCfg.Bind(monsterString, "Lunar Wisp - Remove Hitscan", true, "Changes the Lunar Wisp attack from hitscan to projectile.").Value;
            LunarWisp.enableFalloff = MonsterCfg.Bind(monsterString, "Lunar Wisp - Bullet Falloff", true, "Adds damage falloff to Lunar Wisps bullets. Disabled if Remove Hitscan is enabled.").Value;
            LunarWisp.disableProjectileOnKill = MonsterCfg.Bind(monsterString, "Lunar Wisp - Disable Projectile On-Kill Effects", true, "Prevents on-kill effects from proccing when this monster's projectiles are shot down.").Value;

            Titan.enabled = MonsterCfg.Bind(monsterString, "Stone Titan and Aurelionite", true, "Enable changes to this monster.").Value;
            BeetleQueen.enabled = MonsterCfg.Bind(monsterString, "Beetle Queen", true, "Enable changes to this monster.").Value;
            Vagrant.enabled = MonsterCfg.Bind(monsterString, "Wandering Vagrant", true, "Enable changes to this monster.").Value;
            Vagrant.disableProjectileOnKill = MonsterCfg.Bind(monsterString, "Wandering Vagrant - Disable Projectile On-Kill Effects", true, "Prevents on-kill effects from proccing when this monster's projectiles are shot down.").Value;
            Gravekeeper.disableProjectileOnKill = MonsterCfg.Bind(monsterString, "Grovetender - Disable Projectile On-Kill Effects", true, "Prevents on-kill effects from proccing when this monster's projectiles are shot down.").Value;
            SCU.enabled = MonsterCfg.Bind(monsterString, "Solus Control Unit", true, "Enable changes to this monster.").Value;
            Worm.enabled = MonsterCfg.Bind(monsterString, "Magma/Overloading Worm", true, "Enable changes to this monster.").Value;

            Aurelionite.modifyStats = MonsterCfg.Bind(monsterString, "Aurelionite", true, "Modify stats.").Value;
            AWU.enabled = MonsterCfg.Bind(monsterString, "Alloy Worship Unit", true, "Enable changes to this monster.").Value;

            Grandparent.enabled = MonsterCfg.Bind(monsterString, "Grandparent", true, "Enable changes to this monster.").Value;

            BlindPest.enabled = MonsterCfg.Bind(monsterString, "Blind Pest", true, "Enable changes to this monster.").Value;
            XiConstruct.enabled = MonsterCfg.Bind(monsterString, "Xi Construct", true, "Enable changes to this monster.").Value;

            VoidInfestor.enabled = MonsterCfg.Bind(monsterString, "Void Infestor", true, "Enable changes to this monster.").Value;
            VoidInfestor.noVoidAllies = MonsterCfg.Bind(monsterString, "Void Infestor - No Ally Infestation", true, "Void Infestors can't possess allies.").Value;

            VoidReaver.enabled = MonsterCfg.Bind(monsterString, "Void Reaver", true, "Enable changes to this monster.").Value;
        }

        private static void ConfigSpawnpools()
        {
            SpawnpoolCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Spawnpools.cfg"), true);

            TitanicPlains.enabled = SpawnpoolCfg.Bind("Stage 1", "Titanic Plains", true, "Enable spawnpool changes on this stage.").Value;
            DistantRoost.enabled = SpawnpoolCfg.Bind("Stage 1", "Distant Roost", true, "Enable spawnpool changes on this stage.").Value;
            SnowyForest.enabled = SpawnpoolCfg.Bind("Stage 1", "Siphoned Forest", true, "Enable spawnpool changes on this stage.").Value;

            GooLake.enabled = SpawnpoolCfg.Bind("Stage 2", "Abandoned Aqueduct", true, "Enable spawnpool changes on this stage.").Value;

            StadiaJungle.enabled = SpawnpoolCfg.Bind("Stage 4", "Stadia Jungle", true, "Enable spawnpool changes on this stage.").Value;
            SirensCall.enabled = SpawnpoolCfg.Bind("Stage 4", "Sirens Call", true, "Enable spawnpool changes on this stage.").Value;

            SkyMeadow.enabled = SpawnpoolCfg.Bind("Stage 5", "Sky Meadow", true, "Enable spawnpool changes on this stage.").Value;
        }

        private static void GenerateCrocoConfig()
        {
            SurvivorCrocoCfg = new ConfigFile(System.IO.Path.Combine(ConfigFolderPath, $"RiskyMod_Survivors_Acrid.cfg"), true);
            CrocoCore.enabled = SurvivorCrocoCfg.Bind("00. General", "Enable Changes", true, "Enable changes to this survivor.").Value;
            CrocoCore.gameplayRework = SurvivorCrocoCfg.Bind("00. General", "Gameplay Rework", true, "A full rework of Acrid's skills. Every option outside of General/Stats requires this to be enabled.").Value;
            BiggerMeleeHitbox.enabled = SurvivorCrocoCfg.Bind("00. General", "Extend Melee Hitbox", true, "Extends Acrid's melee hitbox so he can hit Vagrants while standing on top of them.").Value;
            BlightStack.enabled = SurvivorCrocoCfg.Bind("00. General", "Blight Duration Reset", true, "Blight stacks like Bleed.").Value;
            RemovePoisonDamageCap.enabled = SurvivorCrocoCfg.Bind("00. General", "Remove Poison Damage Cap", true, "Poison no longer has a hidden damage cap.").Value;
            BiggerLeapHitbox.enabled = SurvivorCrocoCfg.Bind("00. General", "Extend Leap Collision Box", true, "Acrid's Shift skills have a larger collision hitbox. Damage radius remains the same.").Value;
            ShiftAirControl.enabled = SurvivorCrocoCfg.Bind("00. General", "Leap Air Control", true, "Acrid's Shift skills gain increased air control at high move speeds.").Value;

            CrocoCore.Cfg.Stats.enabled = SurvivorCrocoCfg.Bind("01. Stats", "0 - Enable Stat Changes", true, "Overwrite Acrid's stats with the values in the config.").Value;
            CrocoCore.Cfg.Stats.health = SurvivorCrocoCfg.Bind("01. Stats", "Health", 160f, "Base health.").Value;
            CrocoCore.Cfg.Stats.damage = SurvivorCrocoCfg.Bind("01. Stats", "Damage", 12f, "Base damage. Vanilla is 15").Value;
            CrocoCore.Cfg.Stats.regen = SurvivorCrocoCfg.Bind("01. Stats", "Regen", 2.5f, "Base health regeneration. Affected by difficulty.").Value;
            CrocoCore.Cfg.Stats.armor = SurvivorCrocoCfg.Bind("01. Stats", "Armor", 20f, "Base armor.").Value;

            CrocoCore.Cfg.Regenerative.healFraction = SurvivorCrocoCfg.Bind("02. Regenerative", "Regenerative Heal Fraction", 0.1f, "How much Regenerative heals. Affected by difficulty.").Value;
            CrocoCore.Cfg.Regenerative.healDuration = SurvivorCrocoCfg.Bind("02. Regenerative", "Regenerative Heal Duration", 3f, "How long it takes for Regenerative to heal its full amount.").Value;

            CrocoCore.Cfg.Passives.baseDoTDuration = SurvivorCrocoCfg.Bind("03. Passives", "Base DoT Duration", 6f, "How long Poison and Blight last for.").Value;
            CrocoCore.Cfg.Passives.virulentDurationMult = SurvivorCrocoCfg.Bind("03. Passives", "Virulent Duration Multiplier", 1.8f, "How much to multiply DoT duration by when Virulent is selected.").Value;
            CrocoCore.Cfg.Passives.contagionSpreadRange = SurvivorCrocoCfg.Bind("03. Passives", "Contagion Spread Range", 30f, "How far Contagion can spread.").Value;

            CrocoCore.Cfg.Skills.ViciousWounds.baseDuration = SurvivorCrocoCfg.Bind("04. Primary - Vicious Wounds", "Base Duration", 1.2f, "Time it takes for each slash. Vanilla is 1.5").Value;
            CrocoCore.Cfg.Skills.ViciousWounds.damageCoefficient = SurvivorCrocoCfg.Bind("04. Primary - Vicious Wounds", "Damage Coefficient", 2f, "Skill damage.").Value;
            CrocoCore.Cfg.Skills.ViciousWounds.finisherDamageCoefficient = SurvivorCrocoCfg.Bind("04. Primary - Vicious Wounds", "Finisher Damage Coefficient", 4f, "Damage of the 3rd combo hit.").Value;

            CrocoCore.Cfg.Skills.Neurotoxin.damageCoefficient = SurvivorCrocoCfg.Bind("05. Secondary - Neurotoxin", "Damage Coefficient", 2.4f, "Skill damage.").Value;
            CrocoCore.Cfg.Skills.Neurotoxin.cooldown = SurvivorCrocoCfg.Bind("05. Secondary - Neurotoxin", "Cooldown", 2f, "Skill cooldown.").Value;

            CrocoCore.Cfg.Skills.Bite.damageCoefficient = SurvivorCrocoCfg.Bind("06. Secondary - Ravenous Bite", "Damage Coefficient", 3.6f, "Skill damage. Vanilla is 3.2").Value;
            CrocoCore.Cfg.Skills.Bite.cooldown = SurvivorCrocoCfg.Bind("06. Secondary - Ravenous Bite", "Cooldown", 2f, "Skill cooldown.").Value;
            CrocoCore.Cfg.Skills.Bite.healFractionOnKill = SurvivorCrocoCfg.Bind("06. Secondary - Ravenous Bite", "Heal on Kill Fraction", 0.08f, "How much HP to heal when killing with this skill.").Value;

            CrocoCore.Cfg.Skills.CausticLeap.cooldown = SurvivorCrocoCfg.Bind("07. Utility - Caustic Leap", "Cooldown", 6f, "Skill cooldown.").Value;
            CrocoCore.Cfg.Skills.CausticLeap.damageCoefficient = SurvivorCrocoCfg.Bind("07. Utility - Caustic Leap", "Damage Coefficient", 3.2f, "Skill damage.").Value;
            CrocoCore.Cfg.Skills.CausticLeap.acidProcCoefficient = SurvivorCrocoCfg.Bind("07. Utility - Caustic Leap", "Acid Proc Coefficient", 0.5f, "Affects the chance and power of item effects triggered by the acid puddle. Vanilla is 0.1").Value;

            CrocoCore.Cfg.Skills.FrenziedLeap.cooldown = SurvivorCrocoCfg.Bind("08. Utility - Frenzied Leap", "Cooldown", 6f, "Skill cooldown.").Value;
            CrocoCore.Cfg.Skills.FrenziedLeap.cooldownReduction = SurvivorCrocoCfg.Bind("08. Utility - Frenzied Leap", "Cooldown Reduction", 1f, "Amount of cooldown to refund per enemy hit.").Value;
            CrocoCore.Cfg.Skills.FrenziedLeap.damageCoefficient = SurvivorCrocoCfg.Bind("08. Utility - Frenzied Leap", "Damage Coefficient", 5.5f, "Skill damage.").Value;

            CrocoCore.Cfg.Skills.Epidemic.cooldown = SurvivorCrocoCfg.Bind("09. Special - Epidemic", "Cooldown", 10f, "Skill cooldown.").Value;
            CrocoCore.Cfg.Skills.Epidemic.baseTickCount = SurvivorCrocoCfg.Bind("09. Special - Epidemic", "Tick Count", 7, "Number of ticks that Epidemic hits for. Does not include the initial hit that applies it.").Value;
            CrocoCore.Cfg.Skills.Epidemic.damageCoefficient = SurvivorCrocoCfg.Bind("09. Special - Epidemic", "Damage Coefficient", 1f, "How much damage each tick of Epidemic deals.").Value;
            CrocoCore.Cfg.Skills.Epidemic.procCoefficient = SurvivorCrocoCfg.Bind("09. Special - Epidemic", "Proc Coefficient", 0.5f, "Affects the chance and power of item effects triggered by this skill.").Value;
            CrocoCore.Cfg.Skills.Epidemic.spreadRange = SurvivorCrocoCfg.Bind("09. Special - Epidemic", "Spread Range", 30f, "How far Epidemic can spread on each bounce.").Value;
        }
    }
}
