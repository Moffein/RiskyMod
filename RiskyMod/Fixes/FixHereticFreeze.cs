using RoR2;
using UnityEngine;

namespace RiskyMod.Fixes
{
    class FixHereticFreeze
    {
        public FixHereticFreeze()
        {
            GameObject bodyObject = Resources.Load<GameObject>("prefabs/characterbodies/hereticbody");

            EntityStateMachine body = null;
            EntityStateMachine weapon = null;
            EntityStateMachine[] stateMachines = bodyObject.GetComponents<EntityStateMachine>();
            foreach (EntityStateMachine esm in stateMachines)
            {
                if (esm.customName == "Body")
                {
                    body = esm;
                }
                else if (esm.customName == "Weapon")
                {
                    weapon = esm;
                }
            }

            SetStateOnHurt ssoh = bodyObject.GetComponent<SetStateOnHurt>();
            ssoh.canBeFrozen = true;
            ssoh.canBeHitStunned = false;
            ssoh.canBeStunned = false;
            ssoh.targetStateMachine = body;
            ssoh.idleStateMachine = new EntityStateMachine[] { weapon };
        }
    }
}
