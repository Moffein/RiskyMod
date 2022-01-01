using RiskyMod.Enemies.Bosses;
using RiskyMod.Enemies.Mobs;
using RiskyMod.Enemies.Mobs.Lunar;

namespace RiskyMod.Enemies
{
    public class EnemiesCore
    {
        public static bool enabled = true;
        public EnemiesCore()
        {
            if (!enabled) return;
            new Beetle();
            new Jellyfish();
            new Imp();
            new HermitCrab();

            new Golem();
            new Mushrum();

            new Bronzong();
            new GreaterWisp();

            new Parent();

            new LunarWisp();

            new BeetleQueen();
            new Vagrant();
            new Gravekeeper();
        }
    }
}
