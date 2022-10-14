using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Void
{
    public class Zoea
    {
        public static bool enabled = true;
        public static bool ignoreAllyCap = true;
        public static int maxAllyCount = 8;

        public Zoea()
        {
            if (!enabled) return;

            On.RoR2.VoidMegaCrabItemBehavior.GetMaxProjectiles += (orig, inventory) =>
            {
                return Math.Min(orig(inventory), maxAllyCount);
            };

            //Why isn't this getting capped? DNSpy shows the code as calling GetMaxProjectiles when calculating this
            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (orig, self, slot) =>
            {
                if (slot == DeployableSlot.VoidMegaCrabItem)
                {
                    //Vanilla just calls GetMaxProjectiles, but why is it different?
                    return Math.Min(self.inventory.GetItemCount(DLC1Content.Items.VoidMegaCrabItem), maxAllyCount);
                }
                else
                {
                    return orig(self, slot);
                }
            };


            On.RoR2.VoidMegaCrabItemBehavior.OnMasterSpawned += (orig, self, spawnResult) =>
            {
                orig(self, spawnResult);

                if (spawnResult.success && spawnResult.spawnedInstance)
                {
                    Inventory allyInv = spawnResult.spawnedInstance.GetComponent<Inventory>();
                    if (allyInv)
                    {
                        if (allyInv.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) allyInv.GiveItem(RoR2Content.Items.UseAmbientLevel);

                        CharacterMaster cm = spawnResult.spawnedInstance.GetComponent<CharacterMaster>();
                        if (cm && cm.teamIndex == TeamIndex.Player)
                        {
                            allyInv.GiveItem(Allies.AlliesCore.AllyMarkerItem);
                            allyInv.GiveItem(Allies.AlliesCore.AllyScalingItem);
                            allyInv.GiveItem(Allies.AlliesCore.AllyAllowVoidDeathItem);
                        }

                        if (self.body && self.body.inventory)
                        {
                            int overstack = self.stack - Zoea.maxAllyCount;
                            if (overstack > 0)
                            {
                                int targetHPBoost = 3 * overstack;
                                int targetDamageBoost = 3 * overstack;

                                allyInv.GiveItem(RoR2Content.Items.BoostHp, targetHPBoost);
                                allyInv.GiveItem(RoR2Content.Items.BoostDamage, targetDamageBoost);
                            }
                        }
                    }
                }
            };

            IL.RoR2.VoidMegaCrabItemBehavior.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                    ))
                {
                    c.EmitDelegate<Func<DirectorSpawnRequest, DirectorSpawnRequest>>((directorSpawnRequest) =>
                    {
                        directorSpawnRequest.ignoreTeamMemberLimit = Zoea.ignoreAllyCap;
                        return directorSpawnRequest;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Zoea IL Hook failed");
                }
            };
        }
    }
}
