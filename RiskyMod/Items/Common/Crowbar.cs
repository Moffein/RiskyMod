using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.Common
{
    public class Crowbar
    {
        public static bool enabled = true;
        public static DamageAPI.ModdedDamageType CrowbarDamage;
        public static CrowbarManager crowbarManager;
        public static float damageCoefficient = 0.35f;

        public static ItemDef itemDef = RoR2Content.Items.Crowbar;

        public Crowbar()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, itemDef);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, itemDef);

            CrowbarDamage = DamageAPI.ReserveDamageType();
            //Remove vanilla effect
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Crowbar")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "FireRing")
                    );
                c.GotoNext(
                     x => x.MatchLdcR4(4f)
                    );
                c.Index++;
                c.Emit(OpCodes.Ldarg_1);//damageinfo
                c.EmitDelegate<Func<float, DamageInfo, float>>((ringThreshold, damageInfo) =>
                {
                    if (DamageAPI.HasModdedDamageType(damageInfo, CrowbarDamage))
                    {
                        if (damageInfo.attacker)
                        {
                            CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                            if (attackerBody)
                            {
                                Inventory inv = attackerBody.inventory;
                                if (inv)
                                {
                                    int crowbarCount = inv.GetItemCount(RoR2Content.Items.Crowbar);
                                    if (crowbarCount > 0)
                                    {
                                        ringThreshold *= GetCrowbarMult(crowbarCount);  //Scale up ring activation threshold to match the crowbar damage bonus.
                                    }
                                }
                            }
                        }
                    }
                    return ringThreshold;
                });
            };

            //Effect handled in OnCharacterDeath (for removal)

            On.RoR2.Run.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    Crowbar.crowbarManager = self.gameObject.GetComponent<CrowbarManager>();
                    if (!Crowbar.crowbarManager)
                    {
                        Crowbar.crowbarManager = self.gameObject.AddComponent<CrowbarManager>();
                    }
                }
            };

            On.RoR2.Stage.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active) Crowbar.crowbarManager.ClearList();
            };

            TakeDamage.ModifyInitialDamageInventoryActions += CrowbarDamageBoost;
        }

        //This is used in multiple places, so it is a static method to make sure calculations are consistent.
        public static float GetCrowbarMult(int crowbarCount)
        {
            return 1f + damageCoefficient * crowbarCount;
        }

        private static void CrowbarDamageBoost(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody, Inventory attackerInventory)
        {
            int crowbarCount = attackerInventory.GetItemCount(RoR2Content.Items.Crowbar);
            if (crowbarCount > 0)
            {
                if (self.body != attackerBody
                    && damageInfo.procCoefficient > 0f
                    && !damageInfo.HasModdedDamageType(Crowbar.CrowbarDamage))
                {
                    float damageCoefficient = damageInfo.damage / attackerBody.damage;
                    if (damageCoefficient >= 4f)
                    {
                        if (Crowbar.crowbarManager.CanApplyCrowbar(self, attackerBody))
                        {
                            damageInfo.damage *= GetCrowbarMult(crowbarCount);
                            EffectManager.SimpleImpactEffect(HealthComponent.AssetReferences.crowbarImpactEffectPrefab, damageInfo.position, -damageInfo.force, true);
                            damageInfo.AddModdedDamageType(Crowbar.CrowbarDamage);
                        }
                    }
                }
            }
        }
    }

    public class CrowbarManager : MonoBehaviour
    {
        private class CrowbarTarget
        {
            public HealthComponent victim;
            public HashSet<CharacterBody> attackers;
            public CrowbarTarget(HealthComponent vict, CharacterBody attacker = null)
            {
                victim = vict;
                attackers = new HashSet<CharacterBody>();
                if (attacker)
                {
                    attackers.Add(attacker);
                }
            }
        }
        private List<CrowbarTarget> targetList;

        public void FixedUpdate()
        {
            if (targetList.Count > 0)
            {
                List<CrowbarTarget> toRemove = new List<CrowbarTarget>();
                foreach (CrowbarTarget ct in targetList)
                {
                    if (ct.victim.combinedHealth >= ct.victim.fullCombinedHealth)
                    {
                        toRemove.Add(ct);
                    }
                }
                foreach(CrowbarTarget ct in toRemove)
                {
                    targetList.Remove(ct);
                }
            }
        }

        public void Awake()
        {
            targetList = new List<CrowbarTarget>();
        }

        public void Remove(HealthComponent victim)
        {
            CrowbarTarget toRemove = null;
            foreach (CrowbarTarget ct in targetList)
            {
                if (ct.victim == victim)
                {
                    toRemove = ct;
                    break;
                }
            }

            targetList.Remove(toRemove);
        }

        public void ClearList()
        {
            targetList.Clear();
        }

        public bool CanApplyCrowbar(HealthComponent victim, CharacterBody attackerBody)
        {
            bool applyCrowbar = true;

            bool foundVictim = false;
            foreach (CrowbarTarget ct in targetList)
            {
                if (ct.victim == victim)
                {
                    foundVictim = true;
                    if (ct.attackers.Contains(attackerBody))
                    {
                        applyCrowbar = false;
                    }
                    else
                    {
                        ct.attackers.Add(attackerBody);
                    }
                    break;
                }
            }

            if (!foundVictim)
            {
                targetList.Add(new CrowbarTarget(victim, attackerBody));
            }

            return applyCrowbar;
        }
    }
}
