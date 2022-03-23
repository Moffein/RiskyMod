using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class DroneParts
    {
        public static bool enabled = true;
        public DroneParts()
        {
            if (!enabled) return;
        }
    }
}
