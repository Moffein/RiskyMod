using RoR2.Projectile;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Items;

namespace RiskyMod.Items.DLC4.Hallowed
{
    public class VileCenser
    {
        public static bool enabled;
        public VileCenser()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            if (RiskyMod.disableProcChains)
            {
                GameObject vileCenserProjectile = Addressables.LoadAssetAsync<GameObject>("be1bfb3232189094d97eaa64a83b7ac5").WaitForCompletion();
                ProjectileController pc = vileCenserProjectile.GetComponent<ProjectileController>();
                pc.procCoefficient = 0.5f;
            }

            On.RoR2.Items.VileCenserDamageController.GetSphereRadius += VileCenserDamageController_GetSphereRadius;
        }

        private float VileCenserDamageController_GetSphereRadius(On.RoR2.Items.VileCenserDamageController.orig_GetSphereRadius orig, RoR2.Items.VileCenserDamageController self, RoR2.Items.VileCenserDamageItem damageItem)
        {
            return 20f;
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC4Content.Items.VileCenser);
        }
    }
}
