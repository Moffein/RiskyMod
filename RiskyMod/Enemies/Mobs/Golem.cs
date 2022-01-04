using MonoMod.Cil;
using RoR2;
using UnityEngine;
using System;

namespace RiskyMod.Enemies.Mobs
{
    public class Golem
    {
        public static bool enabled = true;
        public Golem()
        {
            if (!enabled) return;
            //GameObject golemObject = Resources.Load<GameObject>("prefabs/characterbodies/golembody");
            RemoveClapFalloff();
        }

        private void RemoveClapFalloff()
        {
            //SneedUtils.SneedUtils.DumpEntityStateConfig("EntityStates.GolemMonster.ClapState");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.GolemMonster.ClapState", "damageCoefficient", "2.3");   //orig is 3
            IL.EntityStates.GolemMonster.ClapState.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>(blastAttack =>
                {
                    blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                    return blastAttack;
                });
            };
        }
    }
}
