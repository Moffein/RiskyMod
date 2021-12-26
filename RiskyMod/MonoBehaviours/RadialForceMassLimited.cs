using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using HG;
using System.Collections.Generic;

namespace RiskyMod.MonoBehaviours
{
    //Mostly copied from vanilla RadialForce
    public class RadialForceMassLimited : MonoBehaviour
    {
		private TeamFilter teamFilter;


		public void Awake()
		{
			this.teamFilter = base.GetComponent<TeamFilter>();
			this.sphereSearch = new SphereSearch();
		}


		public void FixedUpdate()
		{
			List<HurtBox> list = CollectionPool<HurtBox, List<HurtBox>>.RentCollection();
			this.SearchForTargets(list);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				this.ApplyPullToHurtBox(list[i]);
				i++;
			}
			if (this.tetherVfxOrigin)
			{
				List<Transform> list2 = CollectionPool<Transform, List<Transform>>.RentCollection();
				int j = 0;
				int count2 = list.Count;
				while (j < count2)
				{
					HurtBox hurtBox = list[j];
					if (hurtBox)
					{
						Transform item = hurtBox.transform;
						HealthComponent healthComponent = hurtBox.healthComponent;
						if (healthComponent)
						{
							Transform coreTransform = healthComponent.body.coreTransform;
							if (coreTransform)
							{
								item = coreTransform;
							}
						}
						list2.Add(item);
					}
					j++;
				}
				this.tetherVfxOrigin.SetTetheredTransforms(list2);
				CollectionPool<Transform, List<Transform>>.ReturnCollection(list2);
			}
			CollectionPool<HurtBox, List<HurtBox>>.ReturnCollection(list);
		}

		public void SearchForTargets(List<HurtBox> dest)
		{
			this.sphereSearch.mask = LayerIndex.entityPrecise.mask;
			this.sphereSearch.origin = this.transform.position;
			this.sphereSearch.radius = this.radius;
			this.sphereSearch.queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;
			this.sphereSearch.RefreshCandidates();
			this.sphereSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(this.teamFilter.teamIndex));
			this.sphereSearch.OrderCandidatesByDistance();
			this.sphereSearch.FilterCandidatesByDistinctHurtBoxEntities();
			this.sphereSearch.GetHurtBoxes(dest);
			this.sphereSearch.ClearCandidates();
		}


		public void ApplyPullToHurtBox(HurtBox hurtBox)
		{
			if (!(hurtBox && hurtBox.healthComponent && hurtBox.healthComponent.body && hurtBox.healthComponent.body.rigidbody)
				|| hurtBox.healthComponent.body.rigidbody.mass > maxMass)
			{
				return;
			}
			HealthComponent healthComponent = hurtBox.healthComponent;
			if (healthComponent && NetworkServer.active)
			{
				CharacterMotor characterMotor = healthComponent.body.characterMotor;
				Vector3 a = hurtBox.transform.position - this.transform.position;
				float num = 1f - Mathf.Clamp(a.magnitude / this.radius, 0f, 1f - this.forceCoefficientAtEdge);
				a = a.normalized * this.forceMagnitude * (1f - num);
				Vector3 velocity;
				float mass;
				if (characterMotor)
				{
					velocity = characterMotor.velocity;
					mass = characterMotor.mass;
				}
				else
				{
					Rigidbody rigidbody = healthComponent.body.rigidbody;
					velocity = rigidbody.velocity;
					mass = rigidbody.mass;
				}
				velocity.y += Physics.gravity.y * Time.fixedDeltaTime;
				healthComponent.TakeDamageForce(a - velocity * (this.damping * mass * num), true, false);
			}
		}

		public float maxMass;
		public float radius;
		public float damping = 0.2f;
		public float forceMagnitude;
		public float forceCoefficientAtEdge = 0.5f;
		public TetherVfxOrigin tetherVfxOrigin;

		private SphereSearch sphereSearch;
	}
}
