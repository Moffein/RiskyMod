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

            GameObject pestObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/FlyingVerminBody");
            CharacterBody cb = pestObject.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 60f;
            cb.levelMaxHealth = 18f;

            cb.baseDamage = 15f;    //15 default
            cb.levelDamage = 3f;

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.FlyingVermin.Weapon.Spit", "damageCoefficient", "1");//2 default
        }
    }
}
