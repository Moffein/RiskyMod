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
            loadBounceDistance = 25f;
            loadTotalDamageCoefficient = 18f;
            loadBounceCount = 2;
            loadBounceDistance = 20f;
        }
    }
}
