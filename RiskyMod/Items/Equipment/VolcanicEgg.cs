using RoR2;
using UnityEngine;
using R2API;

namespace RiskyMod.Items.Equipment
{
    public class VolcanicEgg
    {
        public static bool enabled = true;
        public VolcanicEgg()
        {
            if (!enabled) return;
            GameObject fireball = Resources.Load<GameObject>("prefabs/networkedobjects/FireballVehicle");
            FireballVehicle fv = fireball.GetComponent<FireballVehicle>();
            fv.blastRadius = 16f;
            fv.blastDamageCoefficient = 16f;
            fv.overlapDamageCoefficient = 8f;
            fv.blastDamageType = DamageType.IgniteOnHit | DamageType.Stun1s;

            LanguageAPI.Add("EQUIPMENT_FIREBALLDASH_DESC", "Turn into a <style=cIsDamage>draconic fireball</style> for <style=cIsDamage>5</style> seconds. Deal <style=cIsDamage>800% damage</style> on impact. Detonates at the end for <style=cIsDamage>1600% damage</style> and <style=cIsDamage>stuns</style> nearby monsters.");
        }
    }
}
