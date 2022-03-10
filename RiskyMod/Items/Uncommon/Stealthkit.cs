using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.Uncommon
{
    public class Stealthkit
    {
        public static bool enabled = true;
        public static GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ProcStealthkit");
        public Stealthkit()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Disable vanilla behavior
            On.RoR2.Items.PhasingBodyBehavior.Start += (orig, self) =>
            {
                UnityEngine.Object.Destroy(self);
                return;
            };

            TakeDamage.HandleOnPercentHpLostActions += OnHpLost;
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Phasing);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Phasing);
        }

        private void OnHpLost(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost)
        {
            //Basing this off of https://riskofrain2.fandom.com/wiki/Old_War_Stealthkit Version History
            if (!self.body.HasBuff(RoR2Content.Buffs.Cloak))
            {
                int stealthkitCount = inventory.GetItemCount(RoR2Content.Items.Phasing);
                if (stealthkitCount > 0)
                {
                    if (percentHpLost > 0f)
                    {
                        if (Util.CheckRoll(percentHpLost, self.body.master))
                        {
                            float buffDuration = 1.5f + stealthkitCount * 1.5f;
                            self.body.AddTimedBuff(RoR2Content.Buffs.Cloak, buffDuration);
                            self.body.AddTimedBuff(RoR2Content.Buffs.CloakSpeed, buffDuration);
                            EffectManager.SpawnEffect(Stealthkit.effectPrefab, new EffectData
                            {
                                origin = self.body.transform.position,
                                rotation = Quaternion.identity
                            }, true);
                        }
                    }
                }
            }
        }
    }
}
