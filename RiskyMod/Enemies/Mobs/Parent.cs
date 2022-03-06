using MonoMod.Cil;
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
            LowerCreditCost();
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

        private void LowerCreditCost()
        {
            CharacterSpawnCard csc = LegacyResourcesAPI.Load<CharacterSpawnCard>("spawncards/characterspawncards/cscparent");
            //Debug.Log("\n\n\n\n\n\nCost: " + csc.directorCreditCost);
            csc.directorCreditCost = 75;    //65 to be proportional to parent, 75 for Elder Lemurian
        }
    }
}
