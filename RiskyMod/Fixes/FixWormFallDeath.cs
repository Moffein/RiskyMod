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

            On.RoR2.MapZone.TryZoneStart += MapZone_TryZoneStart;
        }

        private void MapZone_TryZoneStart(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            if (other.gameObject)
            {
                CharacterBody body = other.GetComponent<CharacterBody>();
                if (body)
                {
                    if ((body.bodyIndex == MagmaWormIndex || body.bodyIndex == OverloadingWormIndex)
                    {
                        var teamComponent = body.teamComponent;
                        if (teamComponent)
                        {
                            if (teamComponent.teamIndex != TeamIndex.Player)
                            {
                                TeamIndex origIndex = teamComponent.teamIndex;
                                teamComponent.teamIndex = TeamIndex.Player;
                                orig(self, other);
                                teamComponent.teamIndex = origIndex;
                                return;
                            }
                        }
                    }
                }
            }
            orig(self, other);
        }
    }
}
