using RoR2;
using UnityEngine;
using R2API;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.Equipment
{
    public class VolcanicEgg
    {
        public static bool enabled = true;
        public VolcanicEgg()
        {
            if (!enabled) return;
            On.RoR2.EquipmentCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, RoR2Content.Equipment.FireBallDash);
            };

            GameObject fireball = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/FireBallDash/FireballVehicle.prefab").WaitForCompletion();
            FireballVehicle fv = fireball.GetComponent<FireballVehicle>();
            fv.blastRadius = 14f;
            fv.blastDamageCoefficient = 12f;
            fv.overlapDamageCoefficient = 6f;
            fv.blastDamageType = DamageType.IgniteOnHit | DamageType.Stun1s;
        }
    }
}
