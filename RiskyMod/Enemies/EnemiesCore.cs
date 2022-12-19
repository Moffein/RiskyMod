using R2API;
using RiskyMod.Enemies.Bosses;
using RiskyMod.Enemies.DLC1;
using RiskyMod.Enemies.Mithrix;
using RiskyMod.Enemies.Mobs;
using RiskyMod.Enemies.Mobs.Lunar;
using RiskyMod.Enemies.Spawnpools;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies
{
    public class EnemiesCore
    {
        public static bool modifyEnemies = true;
        public static bool modifySpawns = true;
        public static bool infernoCompat = true;
        public static void DisableRegen(GameObject enemyObject)
        {
            CharacterBody cb = enemyObject.GetComponent<CharacterBody>();
            cb.baseRegen = 0f;
            cb.levelRegen = 0f;
        }

        public EnemiesCore()
        {
            new MonsterFallDamage();
            new AiTargetFinding();
            new PrioritizePlayers();
            Mithrix();
            ModifyEnemies();
            ModifySpawns();
        }

        private void Mithrix()
        {
            new MithrixFallImmune();
            new MithrixTargetPrioritization();
            new ImproveRampAI();
        }

        private void ModifyEnemies()
        {
            if (!modifyEnemies) return;
            new Beetle();
            new Jellyfish();
            new Imp();
            new HermitCrab();
            new Lemurian();

            new Golem();
            new Mushrum();
            new Bison();

            new Bronzong();
            new GreaterWisp();

            new Parent();

            new VoidReaver();

            new LunarGolem();
            new LunarWisp();

            new BeetleQueen();
            new Vagrant();
            new Gravekeeper();
            new SCU();
            new Grandparent();
            new Worm();
            new Titan();

            new AWU();
            new Aurelionite();

            new VoidInfestor();
            new BlindPest();
            new XiConstruct();
        }

        private void ModifySpawns()
        {
            if (!modifySpawns) return;
            SpawnCards.Init();

            new TitanicPlains();
            new DistantRoost();
            new SnowyForest();

            new GooLake();
            new Wetland();

            new RallypointDelta();

            //new SirensCall();
            new StadiaJungle();

            new SkyMeadow();
        }
    }

    public static class SpawnCards
    {
        public static bool initialized = false;

        public static CharacterSpawnCard AlphaConstruct;

        public static CharacterSpawnCard Beetle;
        public static CharacterSpawnCard Lemurian;

        public static CharacterSpawnCard Wisp;
        public static CharacterSpawnCard Jellyfish;
        public static CharacterSpawnCard BlindPestSnowy;
        public static CharacterSpawnCard BlindVerminSnowy;

        public static CharacterSpawnCard Imp;
        public static CharacterSpawnCard Vulture;

        public static CharacterSpawnCard Golem;
        public static CharacterSpawnCard BeetleGuard;
        public static CharacterSpawnCard Mushrum;
        public static CharacterSpawnCard Bison;
        public static CharacterSpawnCard ClayApothecary;

        public static CharacterSpawnCard Bronzong;
        public static CharacterSpawnCard GreaterWisp;

        public static CharacterSpawnCard TitanBlackBeach;
        public static CharacterSpawnCard TitanDampCave;
        public static CharacterSpawnCard TitanGolemPlains;
        public static CharacterSpawnCard TitanGooLake;

        public static CharacterSpawnCard Vagrant;
        public static CharacterSpawnCard BeetleQueen;
        public static CharacterSpawnCard Dunestrider;

        public static CharacterSpawnCard MagmaWorm;
        public static CharacterSpawnCard ImpOverlord;
        public static CharacterSpawnCard Grovetender;
        public static CharacterSpawnCard RoboBall;

        public static CharacterSpawnCard Reminder;

        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            AlphaConstruct = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/MajorAndMinorConstruct/cscMinorConstruct.asset").WaitForCompletion();

            Beetle = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscbeetle");
            Lemurian = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/csclemurian");

            Wisp = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/csclesserwisp");
            Jellyfish = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscjellyfish");

            Imp = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscimp");
            Vulture = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscvulture");

            Golem = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Golem/cscGolem.asset").WaitForCompletion();
            BeetleGuard = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscbeetleguard");
            Mushrum = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscminimushroom");
            Bison = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Bison/cscBison.asset").WaitForCompletion();

            Bronzong = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscbell");
            GreaterWisp = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscgreaterwisp");

            TitanBlackBeach = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/csctitanblackbeach");
            TitanDampCave = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/csctitandampcave");
            TitanGolemPlains = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/csctitangolemplains");
            TitanGooLake = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/csctitangoolake");

            Vagrant = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscvagrant");
            BeetleQueen = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscbeetlequeen");
            Dunestrider = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscclayboss");

            MagmaWorm = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscmagmaworm");
            ImpOverlord = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscimpboss");
            Grovetender = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscgravekeeper");
            RoboBall = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscroboballboss");

            Reminder = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscelectricworm");

            BlindVerminSnowy = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/Vermin/cscVerminSnowy.asset").WaitForCompletion();
            BlindPestSnowy = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/FlyingVermin/cscFlyingVerminSnowy.asset").WaitForCompletion();
            ClayApothecary = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/ClayGrenadier/cscClayGrenadier.asset").WaitForCompletion();

            DirectorCards.Init();
        }
    }

    public static class DirectorCards
    {
        public static bool initialized = false;

        public static DirectorAPI.DirectorCardHolder AlphaConstructLoop;

        public static DirectorAPI.DirectorCardHolder Beetle;
        public static DirectorAPI.DirectorCardHolder Lemurian;

        public static DirectorAPI.DirectorCardHolder Wisp;
        public static DirectorAPI.DirectorCardHolder Jellyfish;
        public static DirectorAPI.DirectorCardHolder BlindPestSnowy;
        public static DirectorAPI.DirectorCardHolder BlindVerminSnowy;

        public static DirectorAPI.DirectorCardHolder Imp;
        public static DirectorAPI.DirectorCardHolder Vulture;

        public static DirectorAPI.DirectorCardHolder Golem;
        public static DirectorAPI.DirectorCardHolder GolemBasic;
        public static DirectorAPI.DirectorCardHolder BeetleGuard;
        public static DirectorAPI.DirectorCardHolder Mushrum;
        public static DirectorAPI.DirectorCardHolder ClayApothecary;
        public static DirectorAPI.DirectorCardHolder Bison;
        public static DirectorAPI.DirectorCardHolder BisonLoop;

        public static DirectorAPI.DirectorCardHolder Bronzong;
        public static DirectorAPI.DirectorCardHolder GreaterWisp;
        public static DirectorAPI.DirectorCardHolder GreaterWispBasic;

        public static DirectorAPI.DirectorCardHolder TitanBlackBeach;
        public static DirectorAPI.DirectorCardHolder TitanDampCave;
        public static DirectorAPI.DirectorCardHolder TitanGolemPlains;
        public static DirectorAPI.DirectorCardHolder TitanGooLake;

        public static DirectorAPI.DirectorCardHolder Vagrant;
        public static DirectorAPI.DirectorCardHolder BeetleQueen;
        public static DirectorAPI.DirectorCardHolder Dunestrider;

        public static DirectorAPI.DirectorCardHolder MagmaWorm;
        public static DirectorAPI.DirectorCardHolder MagmaWormLoop;
        public static DirectorAPI.DirectorCardHolder ImpOverlord;
        public static DirectorAPI.DirectorCardHolder Grovetender;
        public static DirectorAPI.DirectorCardHolder RoboBall;

        public static DirectorAPI.DirectorCardHolder Reminder;
        public static DirectorAPI.DirectorCardHolder ReminderLoop;

        public static DirectorAPI.DirectorCardHolder LunarGolemSkyMeadow;
        public static DirectorAPI.DirectorCardHolder LunarGolemSkyMeadowBasic;

        public static bool logCardInfo = false;
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            AlphaConstructLoop = BuildDirectorCard(SpawnCards.AlphaConstruct, DirectorAPI.MonsterCategory.BasicMonsters, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);

            Beetle = BuildDirectorCard(SpawnCards.Beetle, DirectorAPI.MonsterCategory.BasicMonsters);
            Lemurian = BuildDirectorCard(SpawnCards.Lemurian, DirectorAPI.MonsterCategory.BasicMonsters);

            Wisp = BuildDirectorCard(SpawnCards.Wisp, DirectorAPI.MonsterCategory.BasicMonsters);
            Jellyfish = BuildDirectorCard(SpawnCards.Jellyfish, DirectorAPI.MonsterCategory.BasicMonsters, 1, 0, DirectorCore.MonsterSpawnDistance.Far);
            BlindPestSnowy = BuildDirectorCard(SpawnCards.BlindPestSnowy, DirectorAPI.MonsterCategory.BasicMonsters);
            BlindVerminSnowy = BuildDirectorCard(SpawnCards.BlindVerminSnowy, DirectorAPI.MonsterCategory.BasicMonsters);

            Imp = BuildDirectorCard(SpawnCards.Imp, DirectorAPI.MonsterCategory.BasicMonsters);
            Vulture = BuildDirectorCard(SpawnCards.Vulture, DirectorAPI.MonsterCategory.BasicMonsters);

            Golem = BuildDirectorCard(SpawnCards.Golem, DirectorAPI.MonsterCategory.Minibosses);
            GolemBasic = BuildDirectorCard(SpawnCards.Golem, DirectorAPI.MonsterCategory.BasicMonsters);
            BeetleGuard = BuildDirectorCard(SpawnCards.BeetleGuard, DirectorAPI.MonsterCategory.Minibosses);
            Mushrum = BuildDirectorCard(SpawnCards.Mushrum, DirectorAPI.MonsterCategory.Minibosses); //These are considered basic monsters in Vanilla, but they fit all the criteria of a miniboss enemy.
            ClayApothecary = BuildDirectorCard(SpawnCards.ClayApothecary, DirectorAPI.MonsterCategory.Minibosses);
            Bison = BuildDirectorCard(SpawnCards.Bison, DirectorAPI.MonsterCategory.Minibosses);
            BisonLoop = BuildDirectorCard(SpawnCards.Bison, DirectorAPI.MonsterCategory.Minibosses, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);

            Bronzong = BuildDirectorCard(SpawnCards.Bronzong, DirectorAPI.MonsterCategory.Minibosses);  //Basic Monster on SkyMeadow
            GreaterWisp = BuildDirectorCard(SpawnCards.GreaterWisp, DirectorAPI.MonsterCategory.Minibosses);
            GreaterWispBasic = BuildDirectorCard(SpawnCards.GreaterWisp, DirectorAPI.MonsterCategory.BasicMonsters);

            TitanBlackBeach = BuildDirectorCard(SpawnCards.TitanBlackBeach, DirectorAPI.MonsterCategory.Champions);
            TitanDampCave = BuildDirectorCard(SpawnCards.TitanDampCave, DirectorAPI.MonsterCategory.Champions);
            TitanGolemPlains = BuildDirectorCard(SpawnCards.TitanGolemPlains, DirectorAPI.MonsterCategory.Champions);
            TitanGooLake = BuildDirectorCard(SpawnCards.TitanGooLake, DirectorAPI.MonsterCategory.Champions);

            Vagrant = BuildDirectorCard(SpawnCards.Vagrant, DirectorAPI.MonsterCategory.Champions);
            BeetleQueen = BuildDirectorCard(SpawnCards.BeetleQueen, DirectorAPI.MonsterCategory.Champions);
            Dunestrider = BuildDirectorCard(SpawnCards.Dunestrider, DirectorAPI.MonsterCategory.Champions);

            ImpOverlord = BuildDirectorCard(SpawnCards.ImpOverlord, DirectorAPI.MonsterCategory.Champions);
            Grovetender = BuildDirectorCard(SpawnCards.Grovetender, DirectorAPI.MonsterCategory.Champions);
            RoboBall = BuildDirectorCard(SpawnCards.RoboBall, DirectorAPI.MonsterCategory.Champions);
            MagmaWorm = BuildDirectorCard(SpawnCards.MagmaWorm, DirectorAPI.MonsterCategory.Champions);
            MagmaWormLoop = BuildDirectorCard(SpawnCards.MagmaWorm, DirectorAPI.MonsterCategory.Champions, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);

            Reminder = BuildDirectorCard(SpawnCards.Reminder, DirectorAPI.MonsterCategory.Champions);
            ReminderLoop = BuildDirectorCard(SpawnCards.Reminder, DirectorAPI.MonsterCategory.Champions, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);

            if (logCardInfo)
            {
                //Todo: fix for r2api update
                /*DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
                {
                    foreach (DirectorAPI.DirectorCardHolder dch in list)
                    {
                        Debug.Log("Card: " + dch.Card.spawnCard
                            + "\nCategory: " + dch.MonsterCategory
                            +  "\nWeight: " + dch.Card.selectionWeight
                            + "\nDistance: " + dch.Card.spawnDistance
                            //+ "\nMin Stages: " + dch.Card.minimumStageCompletions
                            + "\n");
                    }
                };*/
            }
        }

        public static DirectorAPI.DirectorCardHolder BuildDirectorCard(CharacterSpawnCard spawnCard, DirectorAPI.MonsterCategory monsterCategory)
        {
            return BuildDirectorCard(spawnCard, monsterCategory, 1, 0, DirectorCore.MonsterSpawnDistance.Standard);
        }

        public static DirectorAPI.DirectorCardHolder BuildDirectorCard(CharacterSpawnCard spawnCard, DirectorAPI.MonsterCategory monsterCategory, int weight, int minStages, DirectorCore.MonsterSpawnDistance spawnDistance)
        {
            DirectorCard dc = new DirectorCard
            {
                spawnCard = spawnCard,
                selectionWeight = weight,
                preventOverhead = false,
                minimumStageCompletions = minStages,
                spawnDistance = spawnDistance
            };
            DirectorAPI.DirectorCardHolder cardHolder = new DirectorAPI.DirectorCardHolder
            {
                Card = dc,
                MonsterCategory = monsterCategory
            };

            return cardHolder;
        }
    }
}
