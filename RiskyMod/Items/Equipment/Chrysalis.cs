using RoR2;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.Equipment
{
    public class Chrysalis
    {
        public static bool enabled = true;
        public Chrysalis()
        {
            if (!enabled) return;
            ItemsCore.ChangeEquipmentCooldown(Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Jetpack/Jetpack.asset").WaitForCompletion(), 45f);
        }
    }
}
