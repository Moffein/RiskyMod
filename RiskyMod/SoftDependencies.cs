using RoR2;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace RiskyMod
{
    public class SoftDependencies
    {
        public static bool ClassicItemsScepterLoaded = false;
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

        public static bool artifactPotentialLoaded = false;
        public static bool IsPotentialArtifactActive()
        {
            bool isActive = false;
            if (artifactPotentialLoaded) isActive = IsPotentialArtifactActiveInternal();
            return isActive;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool IsPotentialArtifactActiveInternal()
        {
            return RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(ArtifactOfPotential.PotentialArtifact.Potential);
        }
    }
}
