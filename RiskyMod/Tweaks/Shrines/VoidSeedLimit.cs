using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Tweaks
{
    public class VoidSeedLimit
    {
        public static int seedLimit = 1;
        public static bool enabled = true;

        private static int seedsSpawned = 0;

        public VoidSeedLimit()
        {
            if (!enabled) return;

            On.RoR2.Stage.Start += (orig, self) =>
            {
                seedsSpawned = 0;
                orig(self);
            };

            IL.RoR2.SceneDirector.PopulateScene += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                    x => x.MatchCallvirt<DirectorCard>("IsAvailable")
                    );
                c.Emit(OpCodes.Ldloc_2);    //Director card
                c.EmitDelegate<Func<bool, DirectorCard, bool>>((isAvailable, card) =>
                {
                    if (isAvailable && card.spawnCard && card.spawnCard.name == "iscVoidCamp")
                    {
                        if (seedsSpawned > seedLimit)
                        {
                            return false;
                        }
                        else
                        {
                            seedsSpawned++;
                            return true;
                        }
                    }
                    return isAvailable;
                });
            };
        }
    }
}
