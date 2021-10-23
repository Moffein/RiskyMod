using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.Boss
{
    public class GenesisLoop
    {
        public static bool enabled = true;
        public GenesisLoop()
        {
            if (!enabled) return;
            IL.EntityStates.VagrantNovaItem.ChargeState.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<RoR2.CharacterBody>("get_attackSpeed")
                    );
                c.Index++;
                c.EmitDelegate<Func<float,float>>((attackSpeed) =>
                {
                    return 1f;
                });
            };
        }
    }
}
