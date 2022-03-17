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
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;

            cb.baseDamage = 12f;    //15 default
            cb.levelDamage = cb.baseDamage * 0.2f;

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.FlyingVermin.Weapon.Spit", "damageCoefficient", "1");//2 default
        }
    }
}
