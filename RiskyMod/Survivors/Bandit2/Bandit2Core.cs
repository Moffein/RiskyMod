namespace RiskyMod.Survivors.Bandit2
{
    public class Bandit2Core
    {
        public static bool enabled = true;
        public Bandit2Core()
        {
            if (!enabled) return;
            new BanditSpecialGracePeriod();
        }
    }
}
