using EntityStates.CaptainSupplyDrop;
using RoR2;
using UnityEngine;

namespace EntityStates.RiskyMod.Captain.Beacon
{
    public class BeaconEquipmentRestoreMain : BaseMainState
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
			return Language.GetString("CAPTAIN_SUPPLY_EQUIPMENT_RESTOCK_INTERACTION");
		}

		public override Interactability GetInteractability(Interactor activator)
		{

			CharacterBody component = activator.GetComponent<CharacterBody>();
			Inventory inventory;
			if (!component || !(inventory = component.inventory))
			{
				return Interactability.Disabled;
			}
			if (BeaconEquipmentRestoreMain.activationCost >= this.energyComponent.energy)
			{
				return Interactability.ConditionsNotMet;
			}
			if (inventory.GetEquipmentRestockableChargeCount(inventory.activeEquipmentSlot) <= 0)
			{
				return Interactability.ConditionsNotMet;
			}
			return Interactability.Available;
		}

		public override void OnInteractionBegin(Interactor activator)
		{
			this.energyComponent.TakeEnergy(BeaconEquipmentRestoreMain.activationCost);
			Inventory inventory = activator.GetComponent<CharacterBody>().inventory;
			inventory.RestockEquipmentCharges(inventory.activeEquipmentSlot, 1);
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.energyComponent.chargeRate = activationCost / rechargeTimePerUse;
			this.energyIndicator.sprite = global::RiskyMod.Content.Assets.MiscSprites.CaptainEquipmentRestockBar;
		}

		public static float activationCost = 100f / 2f;
		public static float rechargeTimePerUse = 30f;

		//public static float cooldownPerUse = 15f;
	}
}
