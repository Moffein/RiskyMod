using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies.Mithrix
{
    public class MithrixCore
    {
        public static bool enabled = true;
        public static BodyIndex brotherBodyIndex;
        public static BodyIndex brotherHurtBodyIndex;

        public MithrixCore()
        {
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                brotherBodyIndex = BodyCatalog.FindBodyIndex("BrotherBody");
                brotherHurtBodyIndex = BodyCatalog.FindBodyIndex("BrotherHurtBody");
            };

            if (!enabled) return;
            new MithrixTargetPrioritization();
            new MithrixFreezeImmune();
        }
    }
}
