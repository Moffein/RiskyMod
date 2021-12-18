using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace RiskyMod.Items.Legendary
{
    public class LaserTurbine
    {
        public static bool enabled = true;
        public LaserTurbine()
        {
            if (!enabled) return;
            if (RiskyMod.disableProcChains)
            {
                EntityStateConfiguration esc = Resources.Load<EntityStateConfiguration>("entitystateconfigurations/EntityStates.LaserTurbine.FireMainBeamState");

                for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
                {
                    if (esc.serializedFieldsCollection.serializedFields[i].fieldName == "mainBeamProcCoefficient")
                    {
                        esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue = "0.5";
                    }
                    else if (esc.serializedFieldsCollection.serializedFields[i].fieldName == "secondBombPrefab")
                    {
                        GameObject secondBombPrefab = Resources.Load<GameObject>("prefabs/projectiles/LaserTurbineBomb").InstantiateClone("RiskyMod_LaserTurbineBomb", true);
                        ProjectileImpactExplosion pie = secondBombPrefab.GetComponent<ProjectileImpactExplosion>();
                        pie.blastProcCoefficient = 0.5f;
                        ProjectileAPI.Add(secondBombPrefab);
                        esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue = secondBombPrefab;
                    }
                }
            }

            AssistManager.HandleAssistInventoryActions += OnKillEffect;
        }

        private void OnKillEffect(CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody, CharacterBody killerBody)
        {
            if (attackerBody != killerBody) //Vanilla behavior is left functional to prevent GetComponent call.
            {
                int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.LaserTurbine);
                if (itemCount > 0)
                {
                    attackerBody.AddTimedBuff(RoR2Content.Buffs.LaserTurbineKillCharge, EntityStates.LaserTurbine.RechargeState.killChargeDuration, EntityStates.LaserTurbine.RechargeState.killChargesRequired);
                }
            }
        }
    }
}
