using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API.Utils;
using RoR2;
using System;
using System.Runtime.CompilerServices;

namespace RiskyMod.Fixes
{
    public class FixPlayercount
    {
        public static bool enabled = true;
        public static bool standalonePluginLoaded = false;
        public static bool MultitudesLoaded = false;
        public FixPlayercount()
        {
            if (!enabled) return;
            //Based on https://github.com/wildbook/R2Mods/blob/master/Multitudes/Multitudes.cs
            var getParticipatingPlayerCount = new Hook(typeof(Run).GetMethodCached("get_participatingPlayerCount"),
                typeof(FixPlayercount).GetMethodCached(nameof(GetParticipatingPlayerCountHook)));
        }
        private static int GetParticipatingPlayerCountHook(Run self)
        {
            return GetConnectedPlayers();
        }

        public static int GetConnectedPlayers()
        {
            int players = 0;
            foreach (PlayerCharacterMasterController pc in PlayerCharacterMasterController.instances)
            {
                if (pc.isConnected)
                {
                    players++;
                }
            }
            if (MultitudesLoaded)
            {
                players = ApplyMultitudes(players);
            }
            return players;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static int ApplyMultitudes(int origPlayerCount)
        {
            return origPlayerCount * Multitudes.Multitudes.Multiplier;
        }
    }
}
