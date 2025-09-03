using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Captain;
using RiskyMod.Survivors.Commando;
using RiskyMod.Survivors.Croco;
using RiskyMod.Survivors.Croco;
using RiskyMod.Survivors.DLC1.VoidFiend;
using RiskyMod.Survivors.Engi;
using RiskyMod.Survivors.Huntress;
using RiskyMod.Survivors.Loader;
using RiskyMod.Survivors.Mage;
using RiskyMod.Survivors.Merc;
using RiskyMod.Survivors.DLC2.Seeker;
using RiskyMod.Survivors.Toolbot;
using RiskyMod.Survivors.Treebot;
using RiskyMod.Survivors.DLC2.FalseSon;

namespace RiskyMod.Survivors
{
    public class SurvivorsCore
    {
        public static bool enabled = true;
        public SurvivorsCore()
        {
            if (!enabled) return;

            new Bandit2Core();
            new CaptainCore();
            new CommandoCore();
            new CrocoCore();
            new EngiCore();
            new HuntressCore();
            new ToolbotCore();
            new TreebotCore();
            new LoaderCore();
            new MageCore();
            new MercCore();
            new VoidFiendCore();
            new SeekerCore();
            new FalseSonCore();
        }
    }
}
