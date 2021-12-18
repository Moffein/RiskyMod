using RiskyMod.Enemies.Bosses;
using RiskyMod.Enemies.Mobs.Lunar;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies
{
    public class EnemiesCore
    {
        public static bool enabled = true;
        public EnemiesCore()
        {
            if (!enabled) return;

            new LunarWisp();

            new Vagrant();
            new Gravekeeper();
        }
    }
}
