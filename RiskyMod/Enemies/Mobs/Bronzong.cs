using RoR2;
using UnityEngine;

namespace RiskyMod.Enemies.Mobs
{
    class Bronzong
    {
        public static bool enabled = true;
        public Bronzong()
        {
            if (!enabled) return;
            FixStun();
        }

        private void FixStun()
        {
            GameObject enemyObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/bellbody");

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

            SetStateOnHurt ssoh = enemyObject.GetComponent<SetStateOnHurt>();
            ssoh.idleStateMachine = new EntityStateMachine[] { weapon };
        }
    }
}
