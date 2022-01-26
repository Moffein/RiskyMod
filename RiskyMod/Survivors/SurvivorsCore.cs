using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Captain;
using RiskyMod.Survivors.Commando;
using RiskyMod.Survivors.Croco;
using RiskyMod.Survivors.Engi;
using RiskyMod.Survivors.Huntress;
using RiskyMod.Survivors.Loader;
using RiskyMod.Survivors.Mage;
using RiskyMod.Survivors.Merc;
using RiskyMod.Survivors.Toolbot;
using RiskyMod.Survivors.Treebot;

namespace RiskyMod.Survivors
{
    public class SurvivorsCore
    {
        public static bool enabled = true;
        public SurvivorsCore()
        {
            new SharedDamageTypes();
            if (!enabled) return;

            new Bandit2Core();
            new CaptainCore();
            new CommandoCore();
            new EngiCore();
            new HuntressCore();
            new ToolbotCore();
            new TreebotCore();
            new CrocoCore();
            new LoaderCore();
            new MageCore();
            new MercCore();
        }
    }
}
