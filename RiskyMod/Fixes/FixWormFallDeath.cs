using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Fixes
{
    public class FixWormFallDeath
    {
        public static BodyIndex MagmaWormIndex;
        public static BodyIndex OverloadingWormIndex;

        public FixWormFallDeath()
        {
            //Get Worm BodyIndex
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                MagmaWormIndex = BodyCatalog.FindBodyIndex("MagmaWormBody");
                OverloadingWormIndex = BodyCatalog.FindBodyIndex("ElectricWormBody");
            };

            IL.RoR2.MapZone.TryZoneStart += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdcI4(0),
                     x => x.MatchStloc(3),
                     x => x.MatchLdloc(0)
                    ))
                {
                    c.Index++;
                    c.Emit(OpCodes.Ldloc, 0);   //CharacterBody
                    c.EmitDelegate<Func<bool, CharacterBody, bool>>((flag, body) =>
                    {
                        return flag || (body.bodyIndex == MagmaWormIndex || body.bodyIndex == OverloadingWormIndex);
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: FixWormFallDeath IL Hook failed");
                }
            };
        }
    }
}
