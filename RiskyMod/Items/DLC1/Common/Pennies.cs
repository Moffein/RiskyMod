using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Common
{
    public class Pennies
    {
        public static bool disableInBazaar = true;

        public Pennies()
        {
            if (disableInBazaar)
            {
                IL.RoR2.HealthComponent.TakeDamage += (il) =>
                {
                    bool error = true;
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(MoveType.After, x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "goldOnHurt")))
                    {
                        c.EmitDelegate<Func<int, int>>(origItemCount =>
                        {
                            return RiskyMod.inBazaar ? 0 : origItemCount;
                        });
                        error = false;
                    }

                    if (error)
                    {
                        Debug.LogError("RiskyMod: Pennies DisableInBazaar IL Hook failed");
                    }
                };
            }
        }
    }
}
