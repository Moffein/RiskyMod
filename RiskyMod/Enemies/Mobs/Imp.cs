using UnityEngine;
using RoR2.CharacterAI;
using RoR2;

namespace RiskyMod.Enemies.Mobs
{
    public class Imp
    {
        public static bool enabled = true;
        public Imp()
        {
            if (!enabled) return;
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Imp/EntityStates.ImpMonster.DoubleSlash.asset", "walkSpeedPenaltyCoefficient", "1");   //0.5 is vanilla
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Imp/EntityStates.ImpMonster.DoubleSlash.asset", "selfForce", "1600");  //Clay Man is 1800
        }
    }
}
