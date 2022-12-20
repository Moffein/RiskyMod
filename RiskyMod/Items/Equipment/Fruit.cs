using RoR2;
using System.Collections.Generic;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.Equipment
{
    public class Fruit
    {
        public static bool enabled = true;

        private static GameObject healEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FruitHealEffect");

        public static float healRadius = 20f;
        public Fruit()
        {
            if (!enabled) return;

            On.RoR2.EquipmentCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, RoR2Content.Equipment.Fruit);
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipPickups, RoR2Content.Equipment.Fruit);
            };

            On.RoR2.EquipmentSlot.FireFruit += (orig, self) =>
            {
                if (self.teamComponent)
                {
                    List<HealthComponent> hcList = new List<HealthComponent>();
                    Collider[] array = Physics.OverlapSphere(self.transform.position, Fruit.healRadius, LayerIndex.entityPrecise.mask);
                    for (int i = 0; i < array.Length; i++)
                    {
                        HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                        if (hurtBox)
                        {
                            HealthComponent healthComponent = hurtBox.healthComponent;
                            if (healthComponent && healthComponent.alive && healthComponent.body && healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex == self.teamComponent.teamIndex)
                            {
                                if (!hcList.Contains(healthComponent)) hcList.Add(healthComponent);
                            }
                        }
                    }

                    foreach (HealthComponent hc in hcList)
                    {
                        EffectData effectData = new EffectData();
                        effectData.origin = hc.body.corePosition;
                        effectData.SetNetworkedObjectReference(hc.gameObject);
                        EffectManager.SpawnEffect(Fruit.healEffectPrefab, effectData, true);
                        hc.HealFraction(0.5f, default(ProcChainMask));
                    }
                }

                return true;
            };
        }
    }
}
