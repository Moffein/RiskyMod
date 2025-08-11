using RiskyFixes.Fixes;
using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace RiskyMod
{
    public class SoftDependencies
    {
        public static bool LinearDamageLoaded = false;
        public static bool RiskOfOptionsLoaded = false;

        public static bool ScepterPluginLoaded = false;

        public static bool AIBlacklistLoaded = false;
        public static bool AIBlacklistUseVanillaBlacklist = true;

        public static bool ZetTweaksLoaded = false;
        public static bool RtAutoSprintLoaded = false;

        public static bool QueensGlandBuffLoaded = false;

        public static bool ShareSuiteLoaded = false;
        public static bool ShareSuiteCommon = false;
        public static bool ShareSuiteUncommon = false;
        public static bool ShareSuiteLegendary = false;
        public static bool ShareSuiteBoss = false;
        public static bool ShareSuiteLunar = false;
        public static bool ShareSuiteVoid = false;

        public static bool SpikestripGrooveSalad = false;
        public static bool SpikestripHeyImNoob = false;
        public static bool SpikestripPlasmaCore = false;

        public static bool InfernoPluginLoaded = false;

        public static bool KingKombatArenaLoaded = false;
        public static bool KingKombatArenaActive = false;

        public static bool ArtifactOfPotentialLoaded = false;

        public static bool TeleporterTurretsLoaded = false;
        public static bool SS2OLoaded = false;

        public static bool MoreStatsLoaded = false;
        public static bool EclipseRevampedLoaded = false;

        public static bool IsPotentialArtifactActive()
        {
            bool isActive = false;
            if (ArtifactOfPotentialLoaded) isActive = IsPotentialArtifactActiveInternal();
            return isActive;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool IsPotentialArtifactActiveInternal()
        {
            return RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfPotential.PotentialArtifact.Potential);
        }

        public static bool SS2_CheckDroneMarker(GameObject gameObject)
        {
            if (SS2OLoaded) return SS2_CheckDroneMarkerInternal(gameObject);
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool SS2_CheckDroneMarkerInternal(GameObject gameObject)
        {
            return gameObject.GetComponent<Moonstorm.Starstorm2.Interactables.DroneTable.RefabricatorHardDeathToken>() != null;
        }

        internal static bool EclipseRevamped_CheckEclipse2Config()
        {
            if (EclipseRevampedLoaded) return EclipseRevamped_CheckEclipse2ConfigInternal();
            return false;
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool EclipseRevamped_CheckEclipse2ConfigInternal()
        {
            return EclipseRevamped.Main.shouldChangeE2.Value;
        }
    }
}
