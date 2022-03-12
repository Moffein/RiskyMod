using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.VoidLocus
{
    public class RemoveFog
    {
        public static bool enabled = true;
        public RemoveFog()
        {
            if (!enabled) return;
            On.RoR2.VoidStageMissionController.RequestFog += (orig, self, zone) =>
            {
                return null;
            };
        }
    }
}
