using RoR2;
using RoR2.Projectile;
using UnityEngine;
using R2API;

namespace Risky_ItemTweaks.Items.Legendary
{
    public class LaserTurbine
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled || Risky_ItemTweaks.disableProcChains) return;
            EntityStateConfiguration esc = Resources.Load<EntityStateConfiguration>("entitystateconfigurations/EntityStates.LaserTurbine.FireMainBeamState");

            for (int i = 0; i < esc.serializedFieldsCollection.serializedFields.Length; i++)
            {
                if (esc.serializedFieldsCollection.serializedFields[i].fieldName == "mainBeamProcCoefficient")
                {
                    esc.serializedFieldsCollection.serializedFields[i].fieldValue.stringValue = "0.4";
                }
                else if (esc.serializedFieldsCollection.serializedFields[i].fieldName == "secondBombPrefab")
                {
                    GameObject secondBombPrefab = Resources.Load<GameObject>("prefabs/projectiles/LaserTurbineBomb").InstantiateClone("RiskyItemTweaks_LaserTurbineBomb", true);
                    ProjectileImpactExplosion pie = secondBombPrefab.GetComponent<ProjectileImpactExplosion>();
                    pie.blastProcCoefficient = 0.4f;
                    ProjectileAPI.Add(secondBombPrefab);
                    esc.serializedFieldsCollection.serializedFields[i].fieldValue.objectValue = secondBombPrefab;
                }
            }
        }
    }
}
