using RoR2;
using UnityEngine;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace RiskyMod.Fixes
{
    public class FixGhostMinonSpawns
    {
        public static bool enabled = true;
        public FixGhostMinonSpawns()
        {
            IL.EntityStates.RoboBallBoss.Weapon.DeployMinions.SummonMinion += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<CharacterBody>("get_inventory")
                     );
                c.Index -= 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<Inventory, EntityStates.RoboBallBoss.Weapon.DeployMinions, Inventory>>((minionInventory, self) =>
                {
                    if (self.characterBody && self.characterBody.inventory)
                    {
                        if (self.characterBody.inventory.GetItemCount(RoR2Content.Items.Ghost) > 0)
                        {
                            minionInventory.GiveItem(RoR2Content.Items.Ghost);

                            minionInventory.GiveItem(RoR2Content.Items.BoostDamage, self.characterBody.inventory.GetItemCount(RoR2Content.Items.BoostDamage));
                        }
                        int decayCount = self.characterBody.inventory.GetItemCount(RoR2Content.Items.HealthDecay);
                        if (decayCount > 0)
                        {
                            minionInventory.GiveItem(RoR2Content.Items.HealthDecay, decayCount);
                        }
                    }
                    return minionInventory;
                });
            };
        }
    }
}
