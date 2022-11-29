using R2API.Networking;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.RiskyMod.Commando.Scepter
{
    public class CookGrenadeScepter : CookGrenade
    {
        public static new float selfBlastRadius = 21f;
        public static new float selfForce = 6000f;

        public override void LoadStats()
        {
            radiusInternal = selfBlastRadius;
            selfForceInternal = selfForce;
        }

        public override void SwapToThrowGrenade()
        {
            this.grenadeThrown = true;
            this.outer.SetNextState(new ThrowGrenadeScepter { fuseTime = base.fixedAge });
        }
    }
}
