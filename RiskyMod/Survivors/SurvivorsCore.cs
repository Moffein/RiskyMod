using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Captain;

namespace RiskyMod.Survivors
{
    public class SurvivorsCore
    {
        public static bool enabled = true;
        public SurvivorsCore()
        {
            if (!enabled) return;
            new CaptainCore();
            new Bandit2Core();
        }
    }
}
