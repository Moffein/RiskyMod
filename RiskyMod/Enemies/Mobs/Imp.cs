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
            RemoveSlashSlow();
            AddLunge();
        }

        private void RemoveSlashSlow()
        {
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.ImpMonster.DoubleSlash", "walkSpeedPenaltyCoefficient", "1");   //0.5 is vanilla
        }

        private void AddLunge()
        {
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.ImpMonster.DoubleSlash", "selfForce", "1600");  //Clay Man is 1800
        }
    }
}
