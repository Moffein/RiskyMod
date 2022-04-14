using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.VoidFields
{
    public class VoidFieldsCore
    {
        public static bool enabled = true;
        public VoidFieldsCore()
        {
            if (!enabled) return;

            new ModifyHoldout();
        }
    }
}
