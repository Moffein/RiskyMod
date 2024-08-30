using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Enemies.Mobs
{
    public class Parent
    {
        public static bool enabled = true;
        public Parent()
        {
            if (!enabled) return;
            //EnableStuns();
            SlamFalloff();
        }

        private void EnableStuns()
        {
            GameObject enemyObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/parentbody");
            SetStateOnHurt ssoh = enemyObject.GetComponent<SetStateOnHurt>();
            ssoh.canBeHitStunned = true;
            ssoh.canBeStunned = true;

            EntityStateMachine weapon = null;
            EntityStateMachine[] stateMachines = enemyObject.GetComponents<EntityStateMachine>();
            foreach (EntityStateMachine esm in stateMachines)
            {
                switch (esm.customName)
                {
                    case "Weapon":
                        weapon = esm;
                        break;
                    default:
                        break;
                }
            }
            if (weapon)
            {
                ssoh.idleStateMachine = new EntityStateMachine[] { weapon };
            }
        }

        private void SlamFalloff()
        {
            IL.EntityStates.ParentMonster.GroundSlam.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    ))
                {
                    c.EmitDelegate<Func<BlastAttack, BlastAttack>>(blastAttack =>
                    {
                        blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                        blastAttack.AddModdedDamageType(SharedDamageTypes.SweetSpotModifier);
                        return blastAttack;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Parent IL Hook failed");
                }
            };
        }
    }
}
