using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Tweaks.Artifact
{
    public class VengeanceVoidTeam
    {
        public static bool enabled = true;

        public VengeanceVoidTeam()
        {
            if (!enabled) return;

            IL.RoR2.Artifacts.DoppelgangerInvasionManager.CreateDoppelganger += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                    ))
                {
                    c.EmitDelegate<Func<DirectorSpawnRequest, DirectorSpawnRequest>>(spawnRequest =>
                    {
                        spawnRequest.teamIndexOverride = new TeamIndex?(TeamIndex.Void);
                        return spawnRequest;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: VengeanceVoidTeam IL Hook failed");
                }
            };
        }
    }
}
