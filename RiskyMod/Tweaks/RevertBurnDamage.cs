using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Tweaks
{
    class RevertBurnDamage
    {
		public static bool enabled = true;
		public RevertBurnDamage()
		{
			if (!enabled) return;
			IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
			{
				ILCursor c = new ILCursor(il);
				c.GotoNext(
						 x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixRed")
						);
				c.GotoNext(MoveType.After,
						 x => x.MatchStfld<InflictDotInfo>("maxStacksFromAttacker"),
						 x => x.MatchLdloc(30)
						);
				//dotinfo on stack
				c.Emit(OpCodes.Ldarg_1);//damageinfo
				c.Emit(OpCodes.Ldloc_2);//VictimBody
				c.EmitDelegate<Func<InflictDotInfo, DamageInfo, CharacterBody, InflictDotInfo>>((dotInfo, damageInfo, victimBody) =>
				{
					if (victimBody.healthComponent && victimBody.teamComponent && victimBody.teamComponent.teamIndex == TeamIndex.Player && dotInfo.totalDamage != null)
					{
						float burnPercent = damageInfo.procCoefficient * 0.2f;  //This assumes old burn was 5% hp damage per second. Might need to doublecheck
						float totalBurnDamage = burnPercent * victimBody.healthComponent.fullCombinedHealth;
						dotInfo.totalDamage = Mathf.Min((float)dotInfo.totalDamage, totalBurnDamage);
					}
					return dotInfo;
				});
			};
		}
    }
}
