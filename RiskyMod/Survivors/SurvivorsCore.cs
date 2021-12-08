using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Captain;
using RiskyMod.Survivors.Commando;
using RiskyMod.Survivors.Engi;
using RiskyMod.Survivors.Huntress;
using RiskyMod.Survivors.Toolbot;
using RiskyMod.Survivors.Treebot;

namespace RiskyMod.Survivors
{
    public class SurvivorsCore
    {
        public static bool enabled = true;
        public SurvivorsCore()
        {
            if (!enabled) return;
            new CommandoCore();
            new HuntressCore();
            new ToolbotCore();
            new EngiCore();
            new TreebotCore();
            new CaptainCore();
            new Bandit2Core();
        }
    }
}
