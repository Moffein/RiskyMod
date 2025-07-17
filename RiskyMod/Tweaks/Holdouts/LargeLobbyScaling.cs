using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.Tweaks.Holdouts
{
    public class LargeLobbyScaling
    {
        public static bool enabled = true;

        public static float increasePerPlayer = 0.125f;
        public static int maxIncreases = 8;

        public LargeLobbyScaling()
        {
            if (!enabled) return;
            IL.RoR2.HoldoutZoneController.DoUpdate += HoldoutZoneController_DoUpdate;
        }

        private void HoldoutZoneController_DoUpdate(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchStloc(3)))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, HoldoutZoneController, float>>((radius, self) =>
                {
                    if (self.chargingTeam == TeamIndex.Player)
                    {
                        int players = 0;
                        foreach (PlayerCharacterMasterController pc in PlayerCharacterMasterController.instances)
                        {
                            if (pc.isConnected && pc.master && pc.master.teamIndex == TeamIndex.Player)
                            {
                                players++;
                            }
                        }

                        if (players > 4)
                        {
                            radius *= 1f + Mathf.Min(maxIncreases, players - 4) * increasePerPlayer;
                        }
                    }
                    return radius;
                });
            }
            else
            {
                Debug.LogError("RiskyMod: LargeLobbyScaling IL hook failed.");
            }
        }
    }
}
