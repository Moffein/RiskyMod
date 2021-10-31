namespace RiskyMod.Items.Equipment
{
    public class Capacitor
    {
        public static bool enabled = true;
        public Capacitor()
        {
            if (!enabled) return;
            ItemsCore.ChangeEquipmentCooldown(ItemsCore.LoadEquipmentDef("lightning"), 30f);
        }
    }
}
