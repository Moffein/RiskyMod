using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace RiskyMod.Items.Uncommon
{
    public class Razorwire
    {
        public static bool enabled = true;
        public Razorwire()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove vanilla effect
            On.RoR2.HealthComponent.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                self.itemCounts.thorns = 0;
            };

            TakeDamage.OnPercentHpLostActions += OnHpLost;
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Thorns);
        }

        private void OnHpLost(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost)
        {
            int thornCount = inventory.GetItemCount(RoR2Content.Items.Thorns);
            if (thornCount > 0 && !damageInfo.procChainMask.HasProc(ProcType.Thorns))
            {
                float hpLostLerp = percentHpLost / 10f; //Was /100f, Lerp should be clamped.
                int targetCount = 1 + 2 * thornCount;
                bool isVengeanceClone = inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0; //In case some other mod tries to mess with HealthComponent's itemcount

                bool isCrit = self.body.RollCrit();
                int stack = thornCount - 1;
                float damageValue = (1.6f + 0.4f * stack) * self.body.damage;    //Mathf.Lerp(0.84f + 0.18f * stack, 4.2f + 0.9f * stack, hpLostLerp)
                float proc = Mathf.Lerp(0.1f, 0.5f, hpLostLerp);
                float radius = 25f; //Mathf.Lerp(20f, 50f, hpLostLerp);

                TeamIndex teamIndex2 = self.body.teamComponent.teamIndex;
                HurtBox[] hurtBoxes = new SphereSearch
                {
                    origin = damageInfo.position,
                    radius = radius,
                    mask = LayerIndex.entityPrecise.mask,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal
                }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamIndex2)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes();
                for (int k = 0; k < Mathf.Min(targetCount, hurtBoxes.Length); k++)
                {
                    LightningOrb lightningOrb = new LightningOrb();
                    lightningOrb.attacker = self.gameObject;
                    lightningOrb.bouncedObjects = null;
                    lightningOrb.bouncesRemaining = 0;
                    lightningOrb.damageCoefficientPerBounce = 1f;
                    lightningOrb.damageColorIndex = DamageColorIndex.Item;
                    lightningOrb.damageValue = damageValue;
                    lightningOrb.isCrit = isCrit;
                    lightningOrb.lightningType = LightningOrb.LightningType.RazorWire;
                    lightningOrb.origin = damageInfo.position;
                    lightningOrb.procChainMask = default(ProcChainMask);
                    lightningOrb.procChainMask.AddProc(ProcType.Thorns);
                    lightningOrb.procCoefficient = isVengeanceClone ? 0f : (RiskyMod.disableProcChains ? proc : 0.5f);
                    lightningOrb.range = 0f;
                    lightningOrb.teamIndex = teamIndex2;
                    lightningOrb.target = hurtBoxes[k];
                    OrbManager.instance.AddOrb(lightningOrb);
                }
            }
        }
    }
}
