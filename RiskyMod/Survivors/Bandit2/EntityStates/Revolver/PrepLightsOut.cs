namespace EntityStates.RiskyMod.Bandit2.Revolver
{
    public class PrepLightsOut : BasePrepState
    {
        protected override EntityState GetNextState()
        {
            return new FireLightsOut();
        }
    }
}
