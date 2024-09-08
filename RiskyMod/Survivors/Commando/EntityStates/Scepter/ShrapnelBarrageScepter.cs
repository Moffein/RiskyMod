using RoR2;
using UnityEngine;
using R2API;
using RiskyMod.Survivors.Commando;

namespace EntityStates.RiskyMod.Commando.Scepter
{
	public class ShrapnelBarrageScepter : ShrapnelBarrage
	{
        public override void LoadStats()
        {
			internalBaseBulletCount = 12;
			internalBaseDurationBetweenShots = 0.04f;
        }

        protected override float DamageCoefficient => 1.5f;
	}
}
