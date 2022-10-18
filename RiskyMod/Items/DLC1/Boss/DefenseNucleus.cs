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
        public static bool inheritEliteAffix = true;
        public static bool removeAllyScaling = true;

        public static SpawnCard MinorConstructOnKillCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC1/MajorAndMinorConstruct/cscMinorConstructOnKill.asset").WaitForCompletion();
        public static GameObject SpawnEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/OmniExplosionVFXMinorConstruct.prefab").WaitForCompletion();

        public DefenseNucleus()
        {
            if (!enabled) return;

            if (removeAllyScaling)
            {
                ItemsCore.ModifyItemDefActions += ModifyItem;
                On.RoR2.CharacterMaster.GetDeployableSameSlotLimit += (orig, self, slot) =>
                {
                    if (slot == DeployableSlot.MinorConstructOnKill)
                    {
                        return (self.inventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill) > 0) ? 4 : 0;
                    }
                    else
                    {
                        return orig(self, slot);
                    }
                };
            }

            //Rewrite code to directly spawn the Construct if Elite Affix needs to be inherited since the info is lost when firing a projectile
            if (inheritEliteAffix)
            {
                //Disable Vanilla Effect
                IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if(c.TryGotoNext(
                         x => x.MatchLdsfld(typeof(DLC1Content.Items), "MinorConstructOnKill")
                        ))
                    {
                        c.Remove();
                        c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("RiskyMod: DefenseNucleus OnCharacterDeath IL Hook failed");
                    }
                };

                SharedHooks.OnCharacterDeath.OnCharacterDeathInventoryActions += SpawnEliteConstruct;
            }
            else
            {
                IL.RoR2.Projectile.ProjectileSpawnMaster.SpawnMaster += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if(c.TryGotoNext(
                         x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                        ))
                    {
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
                                            if (spawnResult.success)
                                            {
                                                Inventory allyInv = spawnResult.spawnedInstance.GetComponent<Inventory>();

                                                if (allyInv && stackCount > 0)
                                                {
                                                    if (allyInv.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) allyInv.GiveItem(RoR2Content.Items.UseAmbientLevel);

                                                    if (ownerBody.master.teamIndex == TeamIndex.Player)
                                                    {
                                                        allyInv.GiveItem(Allies.AllyItems.AllyMarkerItem);
                                                        allyInv.GiveItem(Allies.AllyItems.AllyScalingItem);
                                                        allyInv.GiveItem(Allies.AllyItems.AllyAllowVoidDeathItem);
                                                        allyInv.GiveItem(Allies.AllyItems.AllyAllowOverheatDeathItem);
                                                        allyInv.GiveItem(Allies.AllyItems.AllyResistAoEItem);
                                                    }
                                                    allyInv.GiveItem(RoR2Content.Items.HealthDecay, 40);

                                                    if (removeAllyScaling)
                                                    {
                                                        allyInv.GiveItem(RoR2Content.Items.BoostDamage, 15 * stackCount);
                                                        allyInv.GiveItem(RoR2Content.Items.BoostHp, 15 * stackCount);
                                                    }
                                                }
                                            }
                                        }));
                                    }
                                }
                                directorSpawnRequest.ignoreTeamMemberLimit = DefenseNucleus.ignoreAllyCap;
                            }
                            return directorSpawnRequest;
                        });
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("RiskyMod: DefenseNucleus SpawnMaster IL Hook failed");
                    }
                };
            }
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.MinorConstructOnKill);
        }

        private static void SpawnEliteConstruct(GlobalEventManager self, DamageReport damageReport, CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody)
        {
            int itemCount = attackerInventory.GetItemCount(DLC1Content.Items.MinorConstructOnKill);
            if (itemCount > 0 && attackerBody.teamComponent && attackerBody.master && victimBody.isElite)
            {
                if (attackerBody.master.IsDeployableSlotAvailable(DeployableSlot.MinorConstructOnKill))
                {
                    EffectManager.SimpleEffect(DefenseNucleus.SpawnEffectPrefab, victimBody.corePosition, Quaternion.identity, true);
                    SpawnCard spawnCard = DefenseNucleus.MinorConstructOnKillCard;
                    DirectorPlacementRule placementRule = new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Approximate,//Set to approximate since NearestNode causes overlapping
                        minDistance = 5f,
                        maxDistance = 25f,
                        position = victimBody.corePosition
                    };
                    DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, placementRule, RoR2Application.rng);
                    directorSpawnRequest.teamIndexOverride = attackerBody.teamComponent.teamIndex;
                    directorSpawnRequest.summonerBodyObject = attackerBody.gameObject;
                    directorSpawnRequest.ignoreTeamMemberLimit = DefenseNucleus.ignoreAllyCap;
                    directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
                    {
                        if (spawnResult.success)
                        {
                            Inventory allyInv = spawnResult.spawnedInstance.GetComponent<Inventory>();
                            if (allyInv)
                            {
                                CharacterMaster master = spawnResult.spawnedInstance.GetComponent<CharacterMaster>();
                                if (allyInv.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) allyInv.GiveItem(RoR2Content.Items.UseAmbientLevel);
                                if (master && master.teamIndex == TeamIndex.Player)
                                {
                                    allyInv.GiveItem(Allies.AllyItems.AllyMarkerItem);
                                    allyInv.GiveItem(Allies.AllyItems.AllyScalingItem);
                                    allyInv.GiveItem(Allies.AllyItems.AllyAllowVoidDeathItem);
                                    allyInv.GiveItem(Allies.AllyItems.AllyAllowOverheatDeathItem);
                                    allyInv.GiveItem(Allies.AllyItems.AllyResistAoEItem);
                                }
                                allyInv.GiveItem(RoR2Content.Items.HealthDecay, 40);

                                if (removeAllyScaling)
                                {
                                    int stackCount = itemCount - 1;
                                    if (stackCount > 0)
                                    {
                                        //20 on first stack because it includes your initial 100% base damage/HP
                                        allyInv.GiveItem(RoR2Content.Items.BoostDamage, 20 + 15 * stackCount);
                                        allyInv.GiveItem(RoR2Content.Items.BoostHp, 20 + 15 * stackCount);
                                    }
                                }

                                if (victimBody.inventory)
                                {
                                    EquipmentDef ed = EquipmentCatalog.GetEquipmentDef(victimBody.inventory.currentEquipmentIndex);
                                    if (ed && ed.passiveBuffDef && ed.passiveBuffDef.isElite)
                                    {
                                        allyInv.SetEquipmentIndex(victimBody.inventory.currentEquipmentIndex);
                                    }
                                }
                            }

                            Deployable deployable = spawnResult.spawnedInstance.AddComponent<Deployable>();
                            attackerBody.master.AddDeployable(deployable, DeployableSlot.MinorConstructOnKill);
                        }
                    }));
                    DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                }
            }
        }
    }
}
