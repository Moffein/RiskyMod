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
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Thorns);

            //Remove vanilla effect
            On.RoR2.HealthComponent.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                self.itemCounts.thorns = 0;
            };

            //LanguageAPI.Add("ITEM_THORNS_DESC", "Getting hit causes you to explode in a burst of razors, dealing <style=cIsDamage>80%-420% damage</style>. Hits up to <style=cIsDamage>5</style> <style=cStack>(+2 per stack)</style> targets in a <style=cIsDamage>16m-40m</style> radius. Damage and radius increases the more damage taken.");

            TakeDamage.HandleOnPercentHpLostActions += OnHpLost;
        }

        private void OnHpLost(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost)
        {
            int thornCount = inventory.GetItemCount(RoR2Content.Items.Thorns);
            if (thornCount > 0 && !damageInfo.procChainMask.HasProc(ProcType.Thorns))
            {
                float hpLostLerp = percentHpLost / 100f;
                int targetCount = 3 + 2 * thornCount;
                bool isVengeanceClone = inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0; //In case some other mod tries to mess with HealthComponent's itemcount

                bool isCrit = self.body.RollCrit();
                float damageValue = Mathf.Lerp(0.84f, 4.2f, hpLostLerp) * self.body.damage;
                float proc = Mathf.Lerp(0.1f, 0.5f, hpLostLerp);
                float radius = Mathf.Lerp(16f, 40f, hpLostLerp);

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
