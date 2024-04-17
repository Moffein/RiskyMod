using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.RiskyModStates.Bandit2.Revolver.Scepter
{
    class PrepLightsOutScepter : BasePrepState
    {
        protected override EntityState GetNextState()
        {
            return new FireLightsOutScepter();
        }
    }
}
