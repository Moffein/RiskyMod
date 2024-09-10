﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class BackstabRework
    {
        public static bool enabled = true;
        public BackstabRework()
        {
            if (!enabled) return;

            //Don't actually add the crit damage item so that DropIn doesn't get messed up.
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "ConvertCritChanceToCritDamage")
                    ))
                {
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0); //CharacterBody
                    c.EmitDelegate<Func<int, CharacterBody, int>>((cdCount, self) =>
                    {
                        if (cdCount < 1 && self.bodyIndex == Bandit2Core.bodyIndex && self.bodyFlags.HasFlag(CharacterBody.BodyFlags.HasBackstabPassive))
                        {
                            cdCount = 1;
                        }
                        return cdCount;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: BackstabRework IL Hook failed");
                }
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.bodyIndex == Bandit2Core.bodyIndex && sender.bodyFlags.HasFlag(CharacterBody.BodyFlags.HasBackstabPassive))
            {
                args.critDamageMultAdd -= 0.5f;
            }
        }
    }
}
