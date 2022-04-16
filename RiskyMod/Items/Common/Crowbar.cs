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
        public static float damageCoefficient = 0.45f;


        public Crowbar()
        {
            if (!enabled) return;

            ItemsCore.ModifyItemDefActions += ModifyItem;

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
                if (NetworkServer.active)
                {
                    if (Crowbar.crowbarManager)
                    {
                        Crowbar.crowbarManager.ClearList();
                    }
                }
            };

            TakeDamage.ModifyInitialDamageInventoryActions += CrowbarDamageBoost;
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Crowbar);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Crowbar);
        }

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
                    && damageInfo.damage > 0f
                    && !damageInfo.HasModdedDamageType(Crowbar.CrowbarDamage))
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
