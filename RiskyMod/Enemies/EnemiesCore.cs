using R2API;
using RiskyMod.Enemies.Bosses;
using RiskyMod.Enemies.DLC1;
using RiskyMod.Enemies.DLC1.Voidling;
using RiskyMod.Enemies.DLC2;
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
        public static bool spawnpoolDLCReplacementFix = true;
        public EnemiesCore()
        {
            On.RoR2.CharacterAI.BaseAI.UpdateTargets += PrioritizePlayers.AttemptTargetPlayer;
            new MithrixCore();
            new VoidlingCore();
            ModifyEnemies();
            ModifySpawns();
        }

        private void ModifyEnemies()
        {
            if (!modifyEnemies) return;
            new Jellyfish();
            new Imp();
            new HermitCrab();
            new Lemurian();

            new Mushrum();
            new Bison();

            new GreaterWisp();

            new Parent();

            new VoidReaver();

            new LunarWisp();
            new LunarExploder();

            new BeetleQueen();
            new Vagrant();
            new Gravekeeper();
            new SCU();
            new Worm();
            new Titan();

            new AWU();
            new Aurelionite();

            new VoidInfestor();
            new BlindPest();
            new XiConstruct();

            new Child();
            new Scorchling();
        }

        private void ModifySpawns()
        {
            if (!modifySpawns) return;
            SpawnCards.Init();

            new TitanicPlains();
            new DistantRoost();
            new SnowyForest();
            new Lakes();

            new LakesNight();
            new VillageNight();

            new Wetland();
            new GooLake();

            new FrozenWall();

            new DampCaveSimple();
            new SirensCall();
            new StadiaJungle();

            new SkyMeadow();
            new HelminthRoost();
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

        public static CharacterSpawnCard Golem, GolemNature;
        public static CharacterSpawnCard BeetleGuard;
        public static CharacterSpawnCard Mushrum;
        public static CharacterSpawnCard Bison;
        public static CharacterSpawnCard LemurianBruiser;
        public static CharacterSpawnCard ClayApothecary;
        public static CharacterSpawnCard Gup, Geep;

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

            Beetle = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Beetle/cscBeetle.asset").WaitForCompletion();
            Lemurian = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Lemurian/cscLemurian.asset").WaitForCompletion();

            Wisp = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Wisp/cscLesserWisp.asset").WaitForCompletion();
            Jellyfish = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Jellyfish/cscJellyfish.asset").WaitForCompletion();

            Imp = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Imp/cscImp.asset").WaitForCompletion();
            Vulture = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Vulture/cscVulture.asset").WaitForCompletion();

            Golem = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Golem/cscGolem.asset").WaitForCompletion();
            GolemNature = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Golem/cscGolemNature.asset").WaitForCompletion();
            BeetleGuard = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/BeetleGuard/cscBeetleGuard.asset").WaitForCompletion();
            Mushrum = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/MiniMushroom/cscMiniMushroom.asset").WaitForCompletion();
            Bison = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Bison/cscBison.asset").WaitForCompletion();
            LemurianBruiser = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/LemurianBruiser/cscLemurianBruiser.asset").WaitForCompletion();

            Bronzong = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Bell/cscBell.asset").WaitForCompletion();
            GreaterWisp = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/GreaterWisp/cscGreaterWisp.asset").WaitForCompletion();
            TitanBlackBeach = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanBlackBeach.asset").WaitForCompletion();
            TitanDampCave = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanDampCave.asset").WaitForCompletion();
            TitanGolemPlains = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanGolemPlains.asset").WaitForCompletion();
            TitanGooLake = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Titan/cscTitanGooLake.asset").WaitForCompletion();

            Vagrant = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Vagrant/cscVagrant.asset").WaitForCompletion();
            BeetleQueen = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/BeetleQueen/cscBeetleQueen.asset").WaitForCompletion();
            Dunestrider = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/ClayBoss/cscClayBoss.asset").WaitForCompletion();

            MagmaWorm = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/MagmaWorm/cscMagmaWorm.asset").WaitForCompletion();
            ImpOverlord = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/ImpBoss/cscImpBoss.asset").WaitForCompletion();
            Grovetender = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/Gravekeeper/cscGravekeeper.asset").WaitForCompletion();
            RoboBall = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/RoboBallBoss/cscRoboBallBoss.asset").WaitForCompletion();

            Reminder = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/Base/ElectricWorm/cscElectricWorm.asset").WaitForCompletion();

            BlindVerminSnowy = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/Vermin/cscVerminSnowy.asset").WaitForCompletion();
            BlindPestSnowy = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/FlyingVermin/cscFlyingVerminSnowy.asset").WaitForCompletion();
            ClayApothecary = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/ClayGrenadier/cscClayGrenadier.asset").WaitForCompletion();
            Gup = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/Gup/cscGupBody.asset").WaitForCompletion();
            Geep = Addressables.LoadAssetAsync<CharacterSpawnCard>("RoR2/DLC1/Gup/cscGeepBody.asset").WaitForCompletion();

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
        public static DirectorCard GolemNature;
        public static DirectorCard BeetleGuard;
        public static DirectorCard Mushrum;
        public static DirectorCard ClayApothecary;
        public static DirectorCard Bison;
        public static DirectorCard BisonLoop;
        public static DirectorCard GupLoop;
        public static DirectorCard Geep;

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
        public static DirectorCard XiConstructLoop;

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
            GolemNature = BuildDirectorCard(SpawnCards.GolemNature);
            BeetleGuard = BuildDirectorCard(SpawnCards.BeetleGuard);
            Mushrum = BuildDirectorCard(SpawnCards.Mushrum); //These are considered basic monsters in Vanilla, but they fit all the criteria of a miniboss enemy.
            ClayApothecary = BuildDirectorCard(SpawnCards.ClayApothecary);
            Bison = BuildDirectorCard(SpawnCards.Bison);
            BisonLoop = BuildDirectorCard(SpawnCards.Bison, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);
            GupLoop = BuildDirectorCard(SpawnCards.Gup, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);
            Geep = BuildDirectorCard(SpawnCards.Geep);

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

            XiConstructLoop = BuildDirectorCard(SpawnCards.XiConstruct, 1, 5, DirectorCore.MonsterSpawnDistance.Standard);
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
                spawnDistance = spawnDistance,
                forbiddenUnlockableDef = null,
                requiredUnlockableDef = null
            };
            return dc;
        }
    }
}
