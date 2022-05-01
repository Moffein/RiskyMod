using RoR2;
using System;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using UnityEngine.Networking;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace RiskyMod.Items.DLC1.Boss
{
    public class DefenseNucleus
    {
        public static bool enabled = true;
        public static bool ignoreAllyCap = true;

        private static SpawnCard MinorConstructOnKillCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC1/MajorAndMinorConstruct/cscMinorConstructOnKill.asset").WaitForCompletion();

        public DefenseNucleus()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (orig, self, slot) =>
            {
                if (slot == DeployableSlot.MinorConstructOnKill)
                {
                    return 4;
                }
                else
                {
                    return orig(self, slot);
                }
            };

            IL.RoR2.Projectile.ProjectileSpawnMaster.SpawnMaster += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                    );
                c.Emit(OpCodes.Ldarg_0);    //ProjectileSpawnMaster (self)
                c.Emit(OpCodes.Ldloc_2);    //CharacterBody
                c.EmitDelegate<Func<DirectorSpawnRequest, ProjectileSpawnMaster, CharacterBody, DirectorSpawnRequest>>((directorSpawnRequest, self, ownerBody) =>
                {
                    if (self.spawnCard == DefenseNucleus.MinorConstructOnKillCard)
                    {
                        //Master already gets nullchecked in the original method
                        if (ownerBody && ownerBody.master && ownerBody.master.inventory)
                        {
                            int stackCount = ownerBody.master.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) - 1;
                            if (stackCount > 0)
                            {
                                directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
                                {
                                    Inventory allyInv = spawnResult.spawnedInstance.GetComponent<Inventory>();

                                    if (allyInv && stackCount > 1)
                                    {
                                        allyInv.GiveItem(RoR2Content.Items.BoostDamage, 10 * stackCount);
                                        allyInv.GiveItem(RoR2Content.Items.BoostHp, 3 * stackCount);
                                    }
                                }));
                            }
                        }
                        directorSpawnRequest.ignoreTeamMemberLimit = DefenseNucleus.ignoreAllyCap;
                    }
                    return directorSpawnRequest;
                });
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.MinorConstructOnKill);
        }
    }
}
