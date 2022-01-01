using System;
using MonoMod.Cil;
using RoR2;

namespace RiskyMod.Enemies.Mobs
{
    public class Jellyfish
    {
        public static bool enabled = true;
        public Jellyfish()
        {
            if (!enabled) return;
            ChangeFalloff();
        }

        private void ChangeFalloff()
        {
            IL.EntityStates.JellyfishMonster.JellyNova.Detonate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>(blastAttack =>
                {
                    blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                    return blastAttack;
                });
            };
        }
    }
}
