using R2API;
using RiskyMod.Enemies.Bosses;
using RiskyMod.Enemies.DLC1;
using RiskyMod.Enemies.DLC1.Voidling;
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
            On.RoR2.CharacterAI.BaseAI.UpdateTargets += PrioritizePlayers.AttemptTargetPlayer;
            new MithrixCore();
            new VoidlingCore();
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

            new SirensCall();
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
        public static CharacterSpawnCard XiConstruct;

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

            XiConstruct = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/MajorAndMinorConstruct/cscMegaConstruct.asset").WaitForCompletion();

            DirectorCards.Init();
        }
    }

    public static class DirectorCards
    {
        public static bool initialized = false;

        public static DirectorCard AlphaConstructLoop;

        public static DirectorCard Beetle;
        public static DirectorCard Lemurian;

        public static DirectorCard Wisp;
        public static DirectorCard Jellyfish;
        public static DirectorCard BlindPestSnowy;
        public static DirectorCard BlindVerminSnowy;

        public static DirectorCard Imp;
        public static DirectorCard Vulture;

        public static DirectorCard Golem;
        public static DirectorCard BeetleGuard;
        public static DirectorCard Mushrum;
        public static DirectorCard ClayApothecary;
        public static DirectorCard Bison;
        public static DirectorCard BisonLoop;

        public static DirectorCard Bronzong;
        public static DirectorCard GreaterWisp;

        public static DirectorCard TitanBlackBeach;
        public static DirectorCard TitanDampCave;
        public static DirectorCard TitanGolemPlains;
        public static DirectorCard TitanGooLake;

        public static DirectorCard Vagrant;
        public static DirectorCard BeetleQueen;
        public static DirectorCard Dunestrider;

        public static DirectorCard MagmaWorm;
        public static DirectorCard MagmaWormLoop;
        public static DirectorCard ImpOverlord;
        public static DirectorCard Grovetender;
        public static DirectorCard RoboBall;

        public static DirectorCard Reminder;
        public static DirectorCard ReminderLoop;

        public static DirectorCard LunarGolemSkyMeadow;
        public static DirectorCard LunarGolemSkyMeadowBasic;

        public static bool logCardInfo = false;
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            AlphaConstructLoop = BuildDirectorCard(SpawnCards.AlphaConstruct,  1, 5, DirectorCore.MonsterSpawnDistance.Standard);

            Beetle = BuildDirectorCard(SpawnCards.Beetle);
            Lemurian = BuildDirectorCard(SpawnCards.Lemurian);

            Wisp = BuildDirectorCard(SpawnCards.Wisp);
            Jellyfish = BuildDirectorCard(SpawnCards.Jellyfish, 1, 0, DirectorCore.MonsterSpawnDistance.Far);
            BlindPestSnowy = BuildDirectorCard(SpawnCards.BlindPestSnowy);
            BlindVerminSnowy = BuildDirectorCard(SpawnCards.BlindVerminSnowy);

            Imp = BuildDirectorCard(SpawnCards.Imp);
            Vulture = BuildDirectorCard(SpawnCards.Vulture);

            Golem = BuildDirectorCard(SpawnCards.Golem);
            BeetleGuard = BuildDirectorCard(SpawnCards.BeetleGuard);
            Mushrum = BuildDirectorCard(SpawnCards.Mushrum); //These are considered basic monsters in Vanilla, but they fit all the criteria of a miniboss enemy.
            ClayApothecary = BuildDirectorCard(SpawnCards.ClayApothecary);
            Bison = BuildDirectorCard(SpawnCards.Bison);
            BisonLoop = BuildDirectorCard(SpawnCards.Bison, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);

            Bronzong = BuildDirectorCard(SpawnCards.Bronzong);  //Basic Monster on SkyMeadow
            GreaterWisp = BuildDirectorCard(SpawnCards.GreaterWisp);

            TitanBlackBeach = BuildDirectorCard(SpawnCards.TitanBlackBeach);
            TitanDampCave = BuildDirectorCard(SpawnCards.TitanDampCave);
            TitanGolemPlains = BuildDirectorCard(SpawnCards.TitanGolemPlains);
            TitanGooLake = BuildDirectorCard(SpawnCards.TitanGooLake);

            Vagrant = BuildDirectorCard(SpawnCards.Vagrant);
            BeetleQueen = BuildDirectorCard(SpawnCards.BeetleQueen);
            Dunestrider = BuildDirectorCard(SpawnCards.Dunestrider);

            ImpOverlord = BuildDirectorCard(SpawnCards.ImpOverlord);
            Grovetender = BuildDirectorCard(SpawnCards.Grovetender);
            RoboBall = BuildDirectorCard(SpawnCards.RoboBall);
            MagmaWorm = BuildDirectorCard(SpawnCards.MagmaWorm);
            MagmaWormLoop = BuildDirectorCard(SpawnCards.MagmaWorm, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);

            Reminder = BuildDirectorCard(SpawnCards.Reminder);
            ReminderLoop = BuildDirectorCard(SpawnCards.Reminder, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);
        }

        public static DirectorCard BuildDirectorCard(CharacterSpawnCard spawnCard)
        {
            return BuildDirectorCard(spawnCard, 1, 0, DirectorCore.MonsterSpawnDistance.Standard);
        }

        public static DirectorCard BuildDirectorCard(CharacterSpawnCard spawnCard, int weight, int minStages, DirectorCore.MonsterSpawnDistance spawnDistance)
        {
            DirectorCard dc = new DirectorCard
            {
                spawnCard = spawnCard,
                selectionWeight = weight,
                preventOverhead = false,
                minimumStageCompletions = minStages,
                spawnDistance = spawnDistance
            };
            return dc;
        }
    }
}
