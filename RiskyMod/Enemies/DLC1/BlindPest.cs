using RoR2;
using UnityEngine;

namespace RiskyMod.Enemies.DLC1
{
    public class BlindPest
    {
        public static bool enabled = true;
        public BlindPest()
        {
            if (!enabled) return;

            //GameObject pestObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/FlyingVerminBody");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.FlyingVermin.Weapon.Spit", "damageCoefficient", "1");
        }
    }
}
