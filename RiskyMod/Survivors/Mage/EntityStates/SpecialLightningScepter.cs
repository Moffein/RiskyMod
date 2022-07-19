using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.RiskyMod.Mage
{
    public class SpecialLightningScepter : SpecialLightning
    {
        public override void LoadStats()
        {
            base.LoadStats();
            loadBounceDistance = 30f;
            loadTotalDamageCoefficient = 20f;
            loadBounceCount = 2;
        }
    }
}
