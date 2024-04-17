using System;
using System.Collections.Generic;
using System.Text;

namespace EntityStates.RiskyModStates.Bandit2.Revolver.Scepter
{
    class PrepRackEmUpScepter : BasePrepState
    {
        protected override EntityState GetNextState()
        {
            return new FireRackEmUpScepter();
        }
    }
}
