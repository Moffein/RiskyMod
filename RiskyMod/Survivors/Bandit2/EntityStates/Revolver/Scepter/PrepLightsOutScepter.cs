using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.RiskyMod.Bandit2.Revolver.Scepter
{
    class PrepLightsOutScepter : BasePrepState
    {
        protected override EntityState GetNextState()
        {
            return new FireLightsOutScepter();
        }
    }
}
