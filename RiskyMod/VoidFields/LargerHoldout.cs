using RoR2;
using UnityEngine;

namespace RiskyMod.VoidFields
{
    public class LargerHoldout
    {
        public static bool enabled = true;
        public LargerHoldout()
        {
            if (!enabled) return;

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Missions.Arena.NullWard.NullWardBaseState", "wardRadiusOn", "30");  //15 in vanilla 
        }
    }
}
