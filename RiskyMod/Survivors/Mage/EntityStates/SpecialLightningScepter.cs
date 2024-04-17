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
            loadMaxDistance = 60f;
            loadBounceDistance = 30f;
            loadTotalDamageCoefficient = 32f;
            loadBounceCount = 2;
        }
    }
}
