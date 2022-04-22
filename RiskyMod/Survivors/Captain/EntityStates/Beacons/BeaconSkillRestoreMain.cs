using EntityStates.CaptainSupplyDrop;
using RoR2;
using UnityEngine;

namespace EntityStates.RiskyMod.Captain.Beacon
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
			bool anySkillUsed = false;

			CharacterBody activatorBody = activator.GetComponent<CharacterBody>();
			if (activatorBody)
			{
				if (activatorBody.hasAuthority)
				{
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

					if (activationCost >= this.energyComponent.energy || !anySkillUsed)
					{
						return Interactability.ConditionsNotMet;
					}
				}
			}
			else
			{
				return Interactability.Disabled;
			}

			return Interactability.Available;
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
            }
		}

        public override void OnEnter()
        {
            base.OnEnter();
			this.energyComponent.chargeRate = activationCost / rechargeTimePerUse;
		}

        public static float activationCost = 100f/3f;
		public static float rechargeTimePerUse = 20f;
	}
}
