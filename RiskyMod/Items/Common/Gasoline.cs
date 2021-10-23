using RoR2;
using R2API;
using UnityEngine;

namespace RiskyMod.Items.Common
{
    public class Gasoline
    {
        public static bool enabled = true;
        public Gasoline()
        {
            if (!enabled) return;

			LanguageAPI.Add("ITEM_IGNITEONKILL_DESC", "Killing an enemy <style=cIsDamage>ignites</style> all enemies within <style=cIsDamage>16m</style> for <style=cIsDamage>150%</style> base damage. Additionally, enemies <style=cIsDamage>burn</style> for <style=cIsDamage>150%</style> <style=cStack>(+75% per stack)</style> base damage.");

            On.RoR2.GlobalEventManager.ProcIgniteOnKill += (orig, damageReport, igniteOnKillCount, victimBody, attackerTeamIndex) =>
            {
				float blastRadius = victimBody.radius + 16f;
				float baseDamage = damageReport.attackerBody.damage * 1.5f;

				float burnDuration = 1f + 0.5f * igniteOnKillCount;
				float burnDamageMult = 1.5f * igniteOnKillCount / burnDuration;

				Vector3 corePosition = victimBody.corePosition;
				GlobalEventManager.igniteOnKillSphereSearch.origin = corePosition;
				GlobalEventManager.igniteOnKillSphereSearch.mask = LayerIndex.entityPrecise.mask;
				GlobalEventManager.igniteOnKillSphereSearch.radius = blastRadius;
				GlobalEventManager.igniteOnKillSphereSearch.RefreshCandidates();
				GlobalEventManager.igniteOnKillSphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(attackerTeamIndex));
				GlobalEventManager.igniteOnKillSphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
				GlobalEventManager.igniteOnKillSphereSearch.OrderCandidatesByDistance();
				GlobalEventManager.igniteOnKillSphereSearch.GetHurtBoxes(GlobalEventManager.igniteOnKillHurtBoxBuffer);
				GlobalEventManager.igniteOnKillSphereSearch.ClearCandidates();
				for (int i = 0; i < GlobalEventManager.igniteOnKillHurtBoxBuffer.Count; i++)
				{
					HurtBox hurtBox = GlobalEventManager.igniteOnKillHurtBoxBuffer[i];
					if (hurtBox.healthComponent)
					{
						DotController.InflictDot(hurtBox.healthComponent.gameObject, damageReport.attacker, DotController.DotIndex.Burn, burnDuration, burnDamageMult);
					}
				}
				GlobalEventManager.igniteOnKillHurtBoxBuffer.Clear();
				new BlastAttack
				{
					radius = blastRadius,
					baseDamage = baseDamage,
					procCoefficient = 0f,
					crit = Util.CheckRoll(damageReport.attackerBody.crit, damageReport.attackerMaster),
					damageColorIndex = DamageColorIndex.Item,
					attackerFiltering = AttackerFiltering.Default,
					falloffModel = BlastAttack.FalloffModel.None,
					attacker = damageReport.attacker,
					teamIndex = attackerTeamIndex,
					position = corePosition
				}.Fire();
				EffectManager.SpawnEffect(GlobalEventManager.CommonAssets.igniteOnKillExplosionEffectPrefab, new EffectData
				{
					origin = corePosition,
					scale = blastRadius,
					rotation = Util.QuaternionSafeLookRotation(damageReport.damageInfo.force)
				}, true);
			};
        }
    }
}
