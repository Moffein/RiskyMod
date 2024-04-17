using UnityEngine;

namespace EntityStates.RiskyModStates.Huntress
{
	public class BeginArrowRainScepter : EntityStates.Huntress.BaseBeginArrowBarrage
	{
		private void LoadStats()
        {
			this.basePrepDuration = 0.25f;
			this.blinkDuration = 0.3f;
			this.jumpCoefficient = 3f;
			this.blinkVector = Vector3.up;
        }

		public override void OnEnter()
        {
			LoadStats();
			base.OnEnter();
        }

		public override EntityState InstantiateNextState()
		{
			return new ArrowRainScepter();
		}
	}
}
