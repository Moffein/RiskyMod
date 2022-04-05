using EntityStates.CaptainSupplyDrop;
using RoR2;
using UnityEngine;

namespace EntityStates.RiskyMod.Captain.Beacon
{
    public class BeaconResupplyMain : BaseMainState
    {
		public override bool shouldShowEnergy
		{
			get
			{
				return true;
			}
		}

		public override string GetContextString(Interactor activator)
		{
			return Language.GetString("CAPTAIN_SUPPLY_EQUIPMENT_RESTOCK_INTERACTION_RISKYMOD");
		}

		public override Interactability GetInteractability(Interactor activator)
		{
			bool anySkillUsed = false;
			bool equipmentCanRestock = false;

			CharacterBody activatorBody = activator.GetComponent<CharacterBody>();
			if (activatorBody)
			{
				Inventory inventory = activatorBody.inventory;
				SkillLocator skills = activatorBody.skillLocator;

				if (skills)
				{
					foreach (GenericSkill skill in skills.allSkills)
					{
						if (skill.stock < skill.maxStock && skill.skillName != "SupplyDrop1" && skill.skillName != "SupplyDrop2")
						{
							anySkillUsed = true;
							break;
						}
					}
				}

				if (inventory)
				{
					equipmentCanRestock = inventory.GetEquipmentRestockableChargeCount(inventory.activeEquipmentSlot) > 0;
				}
			}

			if (!activatorBody)
			{
				return Interactability.Disabled;
			}
			if (activationCost >= this.energyComponent.energy || (!anySkillUsed && !equipmentCanRestock))
			{
				return Interactability.ConditionsNotMet;
			}

			return Interactability.Available;
		}

		public override void OnInteractionBegin(Interactor activator)
		{
			this.energyComponent.TakeEnergy(activationCost);
			/*Inventory inventory = activator.GetComponent<CharacterBody>().inventory;
			inventory.RestockEquipmentCharges(inventory.activeEquipmentSlot, 1);*/

			CharacterBody activatorBody = activator.GetComponent<CharacterBody>();
			if (activatorBody)
            {
				if (activatorBody.skillLocator)
                {
					activatorBody.skillLocator.ApplyAmmoPack();
                }
				if (activatorBody.inventory)
                {
					activatorBody.inventory.DeductActiveEquipmentCooldown(BeaconResupplyMain.equipmentCooldownPerUse);
                }
            }
		}

        public override void OnEnter()
        {
            base.OnEnter();
			this.energyComponent.chargeRate = activationCost / rechargeTimePerUse;
		}

        public static float activationCost = 100f/3f;
		public static float rechargeTimePerUse = 30f;

		public static float equipmentCooldownPerUse = 15f;
	}
}
