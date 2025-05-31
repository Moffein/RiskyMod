using RoR2;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.Equipment
{
    public class Capacitor
    {
        public static bool enabled = true;
        public Capacitor()
        {
            if (!enabled) return;
            ItemsCore.ChangeEquipmentCooldown(Addressables.LoadAssetAsync<EquipmentDef>("RoR2/Base/Lightning/Lightning.asset").WaitForCompletion(), 30f);
        }
    }
}
