namespace RiskyMod.Items.Equipment
{
    public class Chrysalis
    {
        public static bool enabled = true;
        public Chrysalis()
        {
            if (!enabled) return;
            ItemsCore.ChangeEquipmentCooldown(ItemsCore.LoadEquipmentDef("jetpack"), 45f);
        }
    }
}
