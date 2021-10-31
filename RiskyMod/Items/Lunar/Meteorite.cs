using RoR2;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;

namespace RiskyMod.Items.Lunar
{
    public class Meteorite
    {
        public static bool enabled = true;
        public Meteorite()
        {
            if (!enabled) return;

            On.RoR2.MeteorStormController.DetonateMeteor += (orig, self, meteor) =>
			{
				EffectData effectData = new EffectData
				{
					origin = ((MeteorStormController.Meteor)meteor).impactPosition
				};
				EffectManager.SpawnEffect(self.impactEffectPrefab, effectData, true);
				new BlastAttack
				{
					inflictor = self.gameObject,
					baseDamage = self.blastDamageCoefficient * self.ownerDamage,
					baseForce = self.blastForce,
					attackerFiltering = AttackerFiltering.AlwaysHit,
					crit = self.isCrit,
					falloffModel = BlastAttack.FalloffModel.SweetSpot,
					attacker = self.owner,
					bonusForce = Vector3.zero,
					damageColorIndex = DamageColorIndex.Item,
					position = ((MeteorStormController.Meteor)meteor).impactPosition,
					procChainMask = default(ProcChainMask),
					procCoefficient = 1f,
					teamIndex = TeamIndex.None,
					radius = self.blastRadius
				}.Fire();
			};
		}
    }
}
