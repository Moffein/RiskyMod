﻿using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil.Cil;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using RiskyMod.Allies.DroneChanges;
using R2API;
using UnityEngine.Networking;
using RiskyMod.Allies.DamageResistances;

namespace RiskyMod.Allies
{

    public class AlliesCore
    {
        public static bool enabled = true;
        public static bool beetleGlandDontRetaliate = true;
        public static bool normalizeDroneDamage = true;
        public static bool changeScaling = true;
        public static bool buffRegen = true;
        public static bool droneMeldRegen = true;

        public static bool SpikestripCompat = true;
        public static bool SS2Compat = true;
        public static bool ChenChillDroneCompat = true;
        public static bool ChenGradiusCompat = true;
        public static bool ChenQbDroneCompat = true;
        public static bool TinkersSatchelCompat = true;
        public static bool MoreDronesCompat = true;
        public static bool SandsweptCompat = true;

        private static BodyIndex SpikestripBlueLemurian = BodyIndex.None;
        private static BodyIndex SS2SecurityDrone = BodyIndex.None;
        private static BodyIndex SS2ShockDrone = BodyIndex.None;
        private static BodyIndex SS2CloneDrone = BodyIndex.None;
        private static BodyIndex SandsweptInfernoDrone = BodyIndex.None;
        private static BodyIndex ChenChillDrone = BodyIndex.None;
        private static BodyIndex ChenGradiusPsyDroneRed = BodyIndex.None;
        private static BodyIndex ChenGradiusPsyDroneGreen = BodyIndex.None;
        private static BodyIndex ChenGradiusLaserDrone1 = BodyIndex.None;
        private static BodyIndex ChenGradiusLaserDrone2 = BodyIndex.None;
        private static BodyIndex ChenQbDrone = BodyIndex.None;
        private static BodyIndex TinkerBulwarkDrone = BodyIndex.None;
        private static BodyIndex TinkerItemDrone = BodyIndex.None;
        private static BodyIndex MDSeekerDrone = BodyIndex.None;
        private static BodyIndex MDInfernoDrone = BodyIndex.None;
        private static BodyIndex MDBoosterDrone = BodyIndex.None;

        public static List<BodyIndex> StandardDroneScalingList = new List<BodyIndex>();

        public AlliesCore()
        {
            AllyItems.Init();

            if (!enabled) return;
            if (AlliesCore.normalizeDroneDamage)
            {
                //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireTurret", "damageCoefficient", "0.25");   //Shared with Gunner Drones, but those use a dedicated component to handle attacking now

                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireGatling", "damageCoefficient", "0.45");   //Damage 18 -> 12, coef 0.3 -> 0.45
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.HealBeam", "healCoefficient", "1.7");   //Damage 10 -> 12, coef 2 -> 1.6667
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireMegaTurret", "damageCoefficient", "2.2");   //Damage 14 -> 12, coef 2.2 -> 2.5667 mult by 0.8
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireTwinRocket", "damageCoefficient", "4.6667");   //Damage 14 -> 12, coef 4 -> 4.6667

                //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireMissileBarrage", "damageCoefficient", "1.7");   //Damage 14 -> 12, coef 1 -> 1.166666667

                NormalizeDroneDamage(AllyPrefabs.GunnerDrone);
                NormalizeDroneDamage(AllyPrefabs.HealDrone);
                //NormalizeDroneDamage(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/backupdronebody"));
                NormalizeDroneDamage(AllyPrefabs.MissileDrone);
                NormalizeDroneDamage(AllyPrefabs.EquipmentDrone);
                NormalizeDroneDamage(AllyPrefabs.EmergencyDrone);
                //NormalizeDroneDamage(AllyPrefabs.IncineratorDrone);
                NormalizeDroneDamage(AllyPrefabs.MegaDrone);
                NormalizeDroneDamage(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/dronecommanderbody"));
                NormalizeDroneDamage(AllyPrefabs.GunnerTurret);
                //NormalizeDroneDamage(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/RoboBallGreenBuddyBody"));
                //NormalizeDroneDamage(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/RoboBallRedBuddyBody"));
            }

            new AllyScaling();

            new ModifyBulletAttacks();

            new SuperAttackResist();
            new DotZoneResist();
            new EliteDamageModifiers();
            new NoVoidDamage();
            new AntiSplat();

            TweakDrones();
            new CheaperRepairs();

            RoR2Application.onLoad += OnLoad;

            RoR2.CharacterMaster.onStartGlobal += ExternalModCompat;
        }

        private void OnLoad()
        {
            if (SpikestripCompat) SpikestripBlueLemurian = BodyCatalog.FindBodyIndex("BlueLemurianBody");
            if (SS2Compat)
            {
                SS2SecurityDrone = BodyCatalog.FindBodyIndex("DroidDroneBody");
                SS2CloneDrone = BodyCatalog.FindBodyIndex("CloneDroneBody");
                SS2ShockDrone = BodyCatalog.FindBodyIndex("ShockDroneBody");

                StandardDroneScalingList.Add(SS2CloneDrone);
                StandardDroneScalingList.Add(SS2ShockDrone);
            }
            if (SandsweptCompat)
            {
                SandsweptInfernoDrone = BodyCatalog.FindBodyIndex("InfernoDroneBody");
                StandardDroneScalingList.Add(SandsweptInfernoDrone);
            }
            if (ChenChillDroneCompat)
            {
                ChenChillDrone = BodyCatalog.FindBodyIndex("ChillDroneBody");
                StandardDroneScalingList.Add(ChenChillDrone);
            }
            if (ChenQbDroneCompat)
            {
                ChenQbDrone = BodyCatalog.FindBodyIndex("QbDroneBody");
                StandardDroneScalingList.Add(ChenQbDrone);
            }

            if (ChenGradiusCompat)
            {
                ChenGradiusPsyDroneGreen = BodyCatalog.FindBodyIndex("PsyDroneGreenBody");
                ChenGradiusPsyDroneRed = BodyCatalog.FindBodyIndex("PsyDroneRedBody");
                ChenGradiusLaserDrone1 = BodyCatalog.FindBodyIndex("LaserDrone1Body");
                ChenGradiusLaserDrone2 = BodyCatalog.FindBodyIndex("LaserDrone2Body");

                StandardDroneScalingList.Add(ChenGradiusPsyDroneGreen);
                StandardDroneScalingList.Add(ChenGradiusPsyDroneRed);
                StandardDroneScalingList.Add(ChenGradiusLaserDrone1);
                StandardDroneScalingList.Add(ChenGradiusLaserDrone2);
            }
            if (TinkersSatchelCompat)
            {
                TinkerItemDrone = BodyCatalog.FindBodyIndex("ItemDroneBody");
                TinkerBulwarkDrone = BodyCatalog.FindBodyIndex("BulwarkDroneBody");

                StandardDroneScalingList.Add(TinkerItemDrone);
                StandardDroneScalingList.Add(TinkerBulwarkDrone);
            }
            if (MoreDronesCompat)
            {
                MDSeekerDrone = BodyCatalog.FindBodyIndex("ShredderDrone");
                MDBoosterDrone = BodyCatalog.FindBodyIndex("BoosterDroneBody");
                MDInfernoDrone = BodyCatalog.FindBodyIndex("HellDroneBody");

                StandardDroneScalingList.Add(MDSeekerDrone);
                StandardDroneScalingList.Add(MDBoosterDrone);
                StandardDroneScalingList.Add(MDInfernoDrone);
            }
        }

        private void ExternalModCompat(CharacterMaster master)
        {
            if (NetworkServer.active && master.inventory && master.aiComponents.Length > 0 && master.teamIndex == TeamIndex.Player)
            {
                CharacterBody masterBody = master.GetBody();
                if (!masterBody || masterBody.bodyIndex == BodyIndex.None) return;
                if (StandardDroneScalingList.Contains(masterBody.bodyIndex))
                {
                    master.inventory.GiveItem(AllyItems.AllyMarkerItem);
                    master.inventory.GiveItem(AllyItems.AllyScalingItem);
                    master.inventory.GiveItem(AllyItems.AllyRegenItem, 40);
                }
                else if (masterBody.bodyIndex == SS2SecurityDrone)
                {
                    master.inventory.GiveItem(AllyItems.AllyMarkerItem);
                    master.inventory.GiveItem(AllyItems.AllyScalingItem);
                    master.inventory.GiveItem(AllyItems.AllyAllowOverheatDeathItem);
                    master.inventory.GiveItem(AllyItems.AllyAllowVoidDeathItem);
                }
                else if (masterBody.bodyIndex == SpikestripBlueLemurian)
                {
                    master.inventory.GiveItem(AllyItems.AllyMarkerItem);
                    master.inventory.GiveItem(AllyItems.AllyScalingItem);
                    master.inventory.GiveItem(AllyItems.AllyRegenItem, 40);
                    master.inventory.GiveItem(AllyItems.AllyAllowOverheatDeathItem);
                    master.inventory.GiveItem(AllyItems.AllyAllowVoidDeathItem);
                }
            }
        }

        private void TweakDrones()
        {
            new GunnerTurret();
            new MegaDrone();
            new IncineratorDrone();
            new MissileDrone();
            new GunnerDrone();
            new HealDrone();
            new DroneCommander();
            new EmergencyDrone();
        }

        public static void NormalizeDroneDamage(GameObject bodyObject)
        {
            if (bodyObject)
            {
                CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
                if (cb)
                {
                    cb.baseDamage = 12f;
                    cb.levelDamage = cb.baseDamage * 0.2f;
                }
            }
        }
    }
}
