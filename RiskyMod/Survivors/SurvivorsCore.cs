using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Captain;
using RiskyMod.Survivors.Commando;
using RiskyMod.Survivors.Engi;
using RiskyMod.Survivors.Huntress;

namespace RiskyMod.Survivors
{
    public class SurvivorsCore
    {
        public static bool enabled = true;
        public SurvivorsCore()
        {
            if (!enabled) return;
            new SharedDamageTypes();

            new CommandoCore();
            new HuntressCore();
            new EngiCore();
            new CaptainCore();
            new Bandit2Core();
        }
    }
}
