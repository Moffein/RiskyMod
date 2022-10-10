using EntityStates;
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RiskyMod
{
	//Combined from LunarPrimaryReplacementSkill and ReloadSkillDef
	//Unused due to needing to change Interrupt Priorities.
	public class LunarPrimaryReloadSkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned(GenericSkill skillSlot)
		{
			LunarPrimaryReloadSkillDef.InstanceData instanceData = new LunarPrimaryReloadSkillDef.InstanceData();
			instanceData.skillSlot = skillSlot;
			skillSlot.characterBody.onInventoryChanged += instanceData.OnInventoryChanged;
			return instanceData;
		}

		public override void OnUnassigned(GenericSkill skillSlot)
		{
			skillSlot.characterBody.onInventoryChanged -= ((LunarPrimaryReloadSkillDef.InstanceData)skillSlot.skillInstanceData).OnInventoryChanged;
		}

		public override int GetMaxStock(GenericSkill skillSlot)
		{
			return Mathf.Max(1, skillSlot.characterBody.inventory.GetItemCount(RoR2Content.Items.LunarPrimaryReplacement)) * this.baseMaxStock;
		}

		public override void OnFixedUpdate([NotNull] GenericSkill skillSlot)
		{
			base.OnFixedUpdate(skillSlot);
			LunarPrimaryReloadSkillDef.InstanceData instanceData = (LunarPrimaryReloadSkillDef.InstanceData)skillSlot.skillInstanceData;
			instanceData.currentStock = skillSlot.stock;
			if (instanceData.currentStock < this.GetMaxStock(skillSlot))
			{
				if (skillSlot.stateMachine && !skillSlot.stateMachine.HasPendingState() && skillSlot.stateMachine.CanInterruptState(this.reloadInterruptPriority))
				{
					instanceData.graceStopwatch += Time.fixedDeltaTime;
					if (instanceData.graceStopwatch >= this.graceDuration || instanceData.currentStock == 0)
					{
						skillSlot.stateMachine.SetNextState(EntityStateCatalog.InstantiateState(this.reloadState));
						return;
					}
				}
				else
				{
					instanceData.graceStopwatch = 0f;
				}
			}
		}

		public override void OnExecute([NotNull] GenericSkill skillSlot)
		{
			base.OnExecute(skillSlot);
			((LunarPrimaryReloadSkillDef.InstanceData)skillSlot.skillInstanceData).currentStock = skillSlot.stock;
		}

		public SerializableEntityStateType reloadState;

		public InterruptPriority reloadInterruptPriority = InterruptPriority.Skill;

		public float graceDuration;

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public void OnInventoryChanged()
			{
				this.skillSlot.RecalculateValues();
			}

			public int currentStock;

			public float graceStopwatch;

			public GenericSkill skillSlot;
		}
	}
}