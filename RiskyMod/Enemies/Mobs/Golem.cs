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
            //GameObject golemObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/golembody");
            RemoveClapFalloff();
        }

        private void RemoveClapFalloff()
        {
            //the hitbox is pretty big, so falloff is needed
            IL.EntityStates.GolemMonster.ClapState.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    ))
                {
                    c.EmitDelegate<Func<BlastAttack, BlastAttack>>(blastAttack =>
                    {
                        blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                        return blastAttack;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Golem IL Hook failed");
                }
            };
        }
    }
}
