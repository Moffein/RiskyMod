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

            On.RoR2.VoidMegaCrabItemBehavior.OnMasterSpawned += (orig, self, spawnResult) =>
            {
                orig(self, spawnResult);

                if (spawnResult.success && spawnResult.spawnedInstance)
                {
                    Inventory allyInv = spawnResult.spawnedInstance.GetComponent<Inventory>();
                    if (allyInv)
                    {
                        allyInv.GiveItem(RoR2Content.Items.BoostDamage, 5);
                        if (allyInv.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) allyInv.GiveItem(RoR2Content.Items.UseAmbientLevel);

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
                c.GotoNext(
                     x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                    );
                c.EmitDelegate<Func<DirectorSpawnRequest, DirectorSpawnRequest>>((directorSpawnRequest) =>
                {
                    directorSpawnRequest.ignoreTeamMemberLimit = Zoea.ignoreAllyCap;
                    return directorSpawnRequest;
                });
            };
        }
    }
}
