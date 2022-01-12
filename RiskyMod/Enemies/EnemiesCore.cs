using R2API;
using RiskyMod.Enemies.Bosses;
using RiskyMod.Enemies.Mobs;
using RiskyMod.Enemies.Mobs.Lunar;
using RiskyMod.Enemies.Spawnpools;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace RiskyMod.Enemies
{
    public class EnemiesCore
    {
        public static bool modifyEnemies = true;
        public static bool modifySpawns = true;
        public EnemiesCore()
        {
            ModifyEnemies();
            ModifySpawns();
        }

        private void ModifyEnemies()
        {
            if (!modifyEnemies) return;
            new Beetle();
            new Jellyfish();
            new Imp();
            new HermitCrab();

            new Golem();
            new Mushrum();

            new Bronzong();
            new GreaterWisp();

            new Parent();

            new LunarGolem();
            new LunarWisp();

            new BeetleQueen();
            new Vagrant();
            new Gravekeeper();
            new SCU();

            new AWU();
        }

        private void ModifySpawns()
        {
            if (!modifySpawns) return;
            SpawnCards.Init();

            new StadiaJungle();
        }
    }

    public static class SpawnCards
    {
        public static bool initialized = false;

        public static CharacterSpawnCard Beetle;
        public static CharacterSpawnCard Lemurian;

        public static CharacterSpawnCard Wisp;
        public static CharacterSpawnCard Jellyfish;

        public static CharacterSpawnCard Imp;
        public static CharacterSpawnCard Vulture;

        public static CharacterSpawnCard Golem;
        public static CharacterSpawnCard BeetleGuard;
        public static CharacterSpawnCard Mushrum;

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

            Beetle = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscbeetle");
            Lemurian = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/csclemurian");

            Wisp = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/csclesserwisp");
            Jellyfish = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscjellyfish");

            Imp = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscimp");
            Vulture = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscvulture");

            Golem = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscgolem");
            BeetleGuard = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscbeetleguard");
            Mushrum = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscminimushroom");

            Bronzong = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscbell");
            GreaterWisp = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscgreaterwisp");

            TitanBlackBeach = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/csctitanblackbeach");
            TitanDampCave = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/csctitandampcave");
            TitanGolemPlains = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/csctitangolemplains");
            TitanGooLake = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/csctitangoolake");

            Vagrant = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscvagrant");
            BeetleQueen = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscbeetlequeen");
            Dunestrider = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscclayboss");

            MagmaWorm = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscmagmaworm");
            ImpOverlord = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscimpboss");
            Grovetender = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscgravekeeper");
            RoboBall = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscroboballboss");

            Reminder = Resources.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscelectricworm");

            DirectorCards.Init();
        }
    }

    public static class DirectorCards
    {
        public static bool initialized = false;

        public static DirectorAPI.DirectorCardHolder Beetle;
        public static DirectorAPI.DirectorCardHolder Lemurian;

        public static DirectorAPI.DirectorCardHolder Wisp;
        public static DirectorAPI.DirectorCardHolder Jellyfish;

        public static DirectorAPI.DirectorCardHolder Imp;
        public static DirectorAPI.DirectorCardHolder Vulture;

        public static DirectorAPI.DirectorCardHolder Golem;
        public static DirectorAPI.DirectorCardHolder BeetleGuard;
        public static DirectorAPI.DirectorCardHolder Mushrum;

        public static DirectorAPI.DirectorCardHolder Bronzong;
        public static DirectorAPI.DirectorCardHolder GreaterWisp;

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

        public static bool logCardInfo = true;
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            Beetle = BuildDirectorCard(SpawnCards.Beetle, DirectorAPI.MonsterCategory.BasicMonsters);
            Lemurian = BuildDirectorCard(SpawnCards.Lemurian, DirectorAPI.MonsterCategory.BasicMonsters);

            Wisp = BuildDirectorCard(SpawnCards.Wisp, DirectorAPI.MonsterCategory.BasicMonsters);
            Jellyfish = BuildDirectorCard(SpawnCards.Jellyfish, DirectorAPI.MonsterCategory.BasicMonsters, 1, 0, DirectorCore.MonsterSpawnDistance.Far);

            Imp = BuildDirectorCard(SpawnCards.Imp, DirectorAPI.MonsterCategory.BasicMonsters);
            Vulture = BuildDirectorCard(SpawnCards.Vulture, DirectorAPI.MonsterCategory.BasicMonsters);

            Golem = BuildDirectorCard(SpawnCards.Golem, DirectorAPI.MonsterCategory.Minibosses);
            BeetleGuard = BuildDirectorCard(SpawnCards.BeetleGuard, DirectorAPI.MonsterCategory.Minibosses);
            Mushrum = BuildDirectorCard(SpawnCards.Mushrum, DirectorAPI.MonsterCategory.Minibosses); //These are considered basic monsters in Vanilla, but they fit all the criteria of a miniboss enemy.

            Bronzong = BuildDirectorCard(SpawnCards.Bronzong, DirectorAPI.MonsterCategory.Minibosses);  //Basic Monster on SkyMeadow
            GreaterWisp = BuildDirectorCard(SpawnCards.GreaterWisp, DirectorAPI.MonsterCategory.Minibosses);

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
                DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
                {
                    foreach (DirectorAPI.DirectorCardHolder dch in list)
                    {
                        Debug.Log("Card: " + dch.Card.spawnCard + "\nCategory: " + dch.MonsterCategory +  "\nWeight: " + dch.Card.selectionWeight + "\nDistance: " + dch.Card.spawnDistance + "\n");
                    }
                };
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
                allowAmbushSpawn = true,
                preventOverhead = false,
                minimumStageCompletions = minStages,
                spawnDistance = spawnDistance
            };
            DirectorAPI.DirectorCardHolder cardHolder = new DirectorAPI.DirectorCardHolder
            {
                Card = dc,
                MonsterCategory = monsterCategory,
                InteractableCategory = DirectorAPI.InteractableCategory.None
            };
            return cardHolder;
        }
    }
}
