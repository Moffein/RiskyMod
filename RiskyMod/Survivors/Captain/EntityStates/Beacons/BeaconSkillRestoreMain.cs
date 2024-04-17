using EntityStates.CaptainSupplyDrop;
using RoR2;
using UnityEngine;

namespace EntityStates.RiskyModStates.Captain.Beacon
{
    public class BeaconSkillRestoreMain : BaseMainState
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
			return Language.GetString("CAPTAIN_SUPPLY_SKILL_RESTOCK_INTERACTION_RISKYMOD");
		}

		//Runs on both Client/Server, but Server is in charge of who actually gets to interact.
		public override Interactability GetInteractability(Interactor activator)
		{
			if (this.energyComponent.energy >= BeaconSkillRestoreMain.activationCost)
			{
				return Interactability.Available;
			}
			else
            {
				return Interactability.Disabled;
            }
		}

		public override void OnInteractionBegin(Interactor activator)
		{
			this.energyComponent.TakeEnergy(activationCost);

			CharacterBody activatorBody = activator.GetComponent<CharacterBody>();
			if (activatorBody)
            {
				if (activatorBody.skillLocator)
                {
					activatorBody.skillLocator.ApplyAmmoPack();
				}

				if (activatorBody.inventory)
                {
					activatorBody.inventory.DeductActiveEquipmentCooldown(BeaconSkillRestoreMain.equipmentRechargeAmount);
                }
            }
		}

        public override void OnEnter()
        {
            base.OnEnter();
			this.energyComponent.chargeRate = activationCost / rechargeTimePerUse;
		}

        public static float activationCost = 100f/3f;
		public static float rechargeTimePerUse = 20f;
		public static float equipmentRechargeAmount = 20f;
	}
}
