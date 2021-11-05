namespace RiskyMod.Items.Uncommon
{
    public class WarHorn
    {
        public static bool enabled = true;
        public WarHorn()
        {
            if (!enabled) return;
            //Code handled in SharedHooks.GetStatCoefficients
        }
    }
}
