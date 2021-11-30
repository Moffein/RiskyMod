using UnityEngine;

namespace EntityStates.RiskyMod.Bandit2.Revolver.Scepter
{
    public class FireRackEmUpScepter : FireRackEmUp
	{
		public static new int baseBulletCount = 12;

		public override int GetBulletCount()
		{
			return baseBulletCount;
		}

		public override void NextState()
		{
			this.outer.SetNextState(new FireRackEmUpScepter { shotsFired = this.shotsFired });
		}
	}
}
