using RiskyMod.Enemies.Bosses;
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
            new Vagrant();
            new Gravekeeper();
        }
    }
}
