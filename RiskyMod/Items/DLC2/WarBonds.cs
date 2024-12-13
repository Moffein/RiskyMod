using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC2
{
    public class WarBonds
    {
        public static bool enabled = true;

        private static bool modifiedProc = false;

        public WarBonds()
        {
            if (!enabled) return;

            if (RiskyMod.disableProcChains)
            {
                On.RoR2.GoldOnStageStartBehaviour.FireMissile += GoldOnStageStartBehaviour_FireMissile;
            }
        }

        private void GoldOnStageStartBehaviour_FireMissile(On.RoR2.GoldOnStageStartBehaviour.orig_FireMissile orig, GoldOnStageStartBehaviour self)
        {
            ModifyProc();
            orig(self);
        }

        //This is loaded assync, which means that it can fail even if put in RoR2Application.OnLoadFinished, and accessing the IL for that looks like a pain.
        private void ModifyProc()
        {
            if (modifiedProc) return;
            modifiedProc = true;
            ProjectileController pc = GoldOnStageStartBehaviour.missilePrefab.GetComponent<ProjectileController>();
            pc.procCoefficient = 0f;
        }
    }
}
