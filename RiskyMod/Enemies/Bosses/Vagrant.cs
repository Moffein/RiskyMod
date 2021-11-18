using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies.Bosses
{
    public class Vagrant
    {
        public static bool enabled = true;
        public Vagrant()
        {
            if (!enabled) return;

            IL.EntityStates.VagrantMonster.ChargeMegaNova.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdfld<EntityStates.BaseState>("attackSpeedStat")
                    );
                c.Index++;
                c.EmitDelegate<Func<float, float>>((attackSpeed) =>
                {
                    return 1f;
                });
            };
        }
    }
}
