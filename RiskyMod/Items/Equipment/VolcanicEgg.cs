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
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, RoR2Content.Equipment.FireBallDash);

            GameObject fireball = Resources.Load<GameObject>("prefabs/networkedobjects/FireballVehicle");
            FireballVehicle fv = fireball.GetComponent<FireballVehicle>();
            fv.blastRadius = 16f;
            fv.blastDamageCoefficient = 16f;
            fv.overlapDamageCoefficient = 8f;
            fv.blastDamageType = DamageType.IgniteOnHit | DamageType.Stun1s;
        }
    }
}
