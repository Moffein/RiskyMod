using RoR2;
using UnityEngine;

namespace RiskyMod.Enemies.Mobs.Lunar
{
    public class LunarGolem
    {
        public static bool enabled = true;
        public LunarGolem()
        {
            if (!enabled) return;
            EnemiesCore.DisableRegen(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/lunargolembody"));
        }
    }
}
