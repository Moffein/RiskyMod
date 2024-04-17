using EntityStates;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Captain
{
    public class BeaconRework
    {
        public static class Skills
        {
            public static SkillDef BeaconResupply;
            public static SkillDef BeaconHacking;
            public static SkillDef BeaconRecharge;
        }

        public static bool healCooldown = true;
        public static bool hackCooldown = true;
        public static bool shockCooldown = true;
        public static bool resupplyCooldown = true;

        public static bool hackDisable = false;
        public static bool hackChanges= true;
        public static bool shockChanges = true;
        public static bool resupplyChanges = true;

        public static SkillFamily BeaconFamily1 = LegacyResourcesAPI.Load<SkillFamily>("skilldefs/captainbody/CaptainSupplyDrop1SkillFamily");
        public static SkillFamily BeaconFamily2 = LegacyResourcesAPI.Load<SkillFamily>("skilldefs/captainbody/CaptainSupplyDrop2SkillFamily");

        public BeaconRework(SkillLocator sk)
        {
            Skills.BeaconResupply = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallSupplyDropEquipmentRestock.asset").WaitForCompletion();
            Skills.BeaconHacking = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallSupplyDropHacking.asset").WaitForCompletion();
            if (resupplyCooldown) AddCooldown(Skills.BeaconResupply, 60f);
            if (hackCooldown) AddCooldown(Skills.BeaconHacking, 60f);   //Does this need a longer cooldown?
            if (healCooldown) AddCooldown("RoR2/Base/Captain/CallSupplyDropHealing.asset", 60f);
            if (shockCooldown) AddCooldown("RoR2/Base/Captain/CallSupplyDropShocking.asset", 60f);

            SkillDef captainSpecialGeneric = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/PrepSupplyDrop.asset").WaitForCompletion();
            captainSpecialGeneric.skillDescriptionToken = "CAPTAIN_SPECIAL_DESCRIPTION_RISKYMOD";

            //Disable Vanilla Deployable handling
            IL.EntityStates.Captain.Weapon.CallSupplyDropBase.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                   x => x.MatchCallvirt<CharacterBody>("get_master")
                  ))
                {
                    c.EmitDelegate<Func<CharacterMaster, CharacterMaster>>(characterMaster =>
                    {
                        return null;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: BeaconRework Deployable IL Hook failed");
                }
            };

            //Register beacons when they spawn
            CaptainCore.bodyPrefab.AddComponent<CaptainDeployableManager>();
            IL.EntityStates.Captain.Weapon.CallSupplyDropBase.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                    x => x.MatchCall<NetworkServer>("Spawn")
                   ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<GameObject, EntityStates.Captain.Weapon.CallSupplyDropBase, GameObject>>((beacon, self) =>
                    {
                        CaptainDeployableManager cdm = self.gameObject.GetComponent<CaptainDeployableManager>();
                        if (cdm)
                        {
                            cdm.AddBeacon(beacon, self.activatorSkillSlot);
                        }
                        return beacon;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: BeaconRework IL Hook failed");
                }
            };

            SharedHooks.RecalculateStats.HandleRecalculateStatsActions += (self) =>
            {
                if (self.bodyIndex == CaptainCore.CaptainIndex)
                {
                    if (self.skillLocator.special.bonusStockFromBody > 0)
                    {
                        int leftStocks = Mathf.CeilToInt(self.skillLocator.special.bonusStockFromBody / 2f);
                        int rightStocks = self.skillLocator.special.bonusStockFromBody - leftStocks;

                        self.skillLocator.FindSkill("SupplyDrop1").SetBonusStockFromBody(leftStocks);
                        self.skillLocator.FindSkill("SupplyDrop2").SetBonusStockFromBody(rightStocks);
                    }
                }
            };
            ModifyBeacons(sk);
        }

        private void AddCooldown(string address, float cooldown)
        {
            SkillDef sd = Addressables.LoadAssetAsync<SkillDef>(address).WaitForCompletion();
            AddCooldown(sd, cooldown);
        }

        private void AddCooldown(SkillDef sd, float cooldown)
        {
            sd.rechargeStock = 1;
            sd.baseRechargeInterval = cooldown;
            sd.baseMaxStock = 1;
            sd.beginSkillCooldownOnSkillEnd = false;

            On.RoR2.CaptainSupplyDropController.SetSkillOverride +=
                (On.RoR2.CaptainSupplyDropController.orig_SetSkillOverride orig, CaptainSupplyDropController self, ref SkillDef currentSkillDef, SkillDef newSkillDef, GenericSkill component) =>
                {
                    newSkillDef = currentSkillDef;
                    orig(self, ref currentSkillDef, newSkillDef, component);
                };
        }

        private void ModifyBeacons(SkillLocator sk)
        {
            ModifyBeaconResupply(sk);
            ModifyBeaconHacking(sk);
            ModifyBeaconShocking(sk);
        }

        private void ModifyBeaconShocking(SkillLocator sk)
        {
            if (!shockChanges) return;
            //Debug.Log("Shock Radius: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.CaptainSupplyDrop.ShockZoneMainState", "shockRadius"));//10, same as healing
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.CaptainSupplyDrop.ShockZoneMainState", "shockRadius", "15");

            IL.EntityStates.CaptainSupplyDrop.ShockZoneMainState.Shock += (il) =>
            {
                 ILCursor c = new ILCursor(il);
                 if (c.TryGotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    ))
                 {
                    c.EmitDelegate<Func<BlastAttack, BlastAttack>>(blastAttack =>
                    {
                        blastAttack.AddModdedDamageType(SharedDamageTypes.Slow50For5s);
                        return blastAttack;
                    });
                 }
                 else
                 {
                     UnityEngine.Debug.LogError("RiskyMod: BeaconRework Shock IL Hook failed");
                 }
            };

            /*Transform indicatorTransform = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainSupplyDrop, Shocking.prefab").WaitForCompletion().GetComponent<ModelLocator>()?.modelTransform?.Find("AreaIndicatorCenter");
            if (indicatorTransform)
            {
                indicatorTransform.gameObject.SetActive(true);
                Debug.Log("Shock scale: " + indicatorTransform.localScale);
                indicatorTransform.localScale = new Vector3(1.5f * indicatorTransform.localScale.x, indicatorTransform.localScale.y, 1.5f * indicatorTransform.localScale.z);
            }*/
        }

        private void ModifyBeaconResupply(SkillLocator sk)
        {
            if (!resupplyChanges) return;
            GameObject beaconPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainSupplyDrop, EquipmentRestock.prefab").WaitForCompletion().InstantiateClone("RiskyMod_CaptainSupplyEquipmentRestock", true);
            EntityStateMachine esm = beaconPrefab.GetComponent<EntityStateMachine>();
            esm.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyModStates.Captain.Beacon.BeaconSkillRestoreMain));
            Content.Content.entityStates.Add(typeof(EntityStates.RiskyModStates.Captain.Beacon.BeaconSkillRestoreMain));
            Content.Content.networkedObjectPrefabs.Add(beaconPrefab);//Seems like InstantiateClone auto does this for some reason. TODO: Address, though it seems harmless.

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Captain/EntityStates.Captain.Weapon.CallSupplyDropEquipmentRestock.asset", "supplyDropPrefab", beaconPrefab);

            SkillDef resupplySkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallSupplyDropEquipmentRestock.asset").WaitForCompletion();
            resupplySkillDef.skillDescriptionToken = "CAPTAIN_SUPPLY_SKILL_RESTOCK_DESCRIPTION_RISKYMOD";
        }

        private void ModifyBeaconHacking(SkillLocator sk)
        {
            if (hackDisable)
            {
                BeaconFamily1.variants = BeaconFamily1.variants.Where(v => v.skillDef.activationState.stateType != typeof(EntityStates.Captain.Weapon.CallSupplyDropHacking)).ToArray();
                BeaconFamily2.variants = BeaconFamily2.variants.Where(v => v.skillDef.activationState.stateType != typeof(EntityStates.Captain.Weapon.CallSupplyDropHacking)).ToArray();
            }
            else if (hackChanges)
            {
                SkillDef hackSkillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallSupplyDropHacking.asset").WaitForCompletion();
                hackSkillDef.skillDescriptionToken = "CAPTAIN_SUPPLY_PRICE_REDUCTION_DESCRIPTION_RISKYMOD";

                //Prevent already-hacked interactables from being re-hacked.
                On.EntityStates.CaptainSupplyDrop.HackingMainState.PurchaseInteractionIsValidTarget += (orig, purchaseInteraction) =>
                {
                    bool flag = orig(purchaseInteraction) && !purchaseInteraction.GetComponent<HackMarker>();
                    return flag;
                };

                //Change price and mark the interactable once hacking is complete.
                IL.EntityStates.CaptainSupplyDrop.UnlockTargetState.OnEnter += (il) =>
                {
                    bool error = true;
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(
                        x => x.MatchCall<PurchaseInteraction>("set_Networkcost")
                       ))
                    {
                        c.Emit(OpCodes.Ldarg_0);
                        c.EmitDelegate<Func<int, EntityStates.CaptainSupplyDrop.UnlockTargetState, int>>((newCost, self) =>
                        {
                            newCost = self.target.cost;
                            HackMarker hm = self.target.gameObject.GetComponent<HackMarker>();
                            if (!hm)
                            {
                                hm = self.target.gameObject.AddComponent<HackMarker>();
                                hm.purchaseInteraction = self.target;
                                hm.owner = self.gameObject;
                                hm.newCost = Math.Max(1, Mathf.CeilToInt(self.target.cost * HackMarker.costMult));
                                newCost = hm.newCost;
                            }
                            return newCost;
                        });
                        if (c.TryGotoNext(
                            x => x.MatchCallvirt<Interactor>("AttemptInteraction")
                           ))
                        {
                            c.Emit(OpCodes.Ldarg_0);
                            c.EmitDelegate<Func<GameObject, EntityStates.CaptainSupplyDrop.UnlockTargetState, GameObject>>((gameObject, self) =>
                            {
                                if (self.target.cost > 0)
                                {
                                    return null;
                                }
                                return gameObject;
                            });
                            error = false;
                        }
                    }

                    if (error)
                    {
                        UnityEngine.Debug.LogError("RiskyMod: BeaconRework Hack UnlockTargetState IL Hook failed");
                    }
                };
            }
        }

        public class CaptainDeployableManager : MonoBehaviour
        {
            public SkillLocator skillLocator;
            public CharacterBody body;

            public GenericSkill Beacon1;
            public GenericSkill Beacon2;

            public Queue<GameObject> Beacon1Deployables;
            public Queue<GameObject> Beacon2Deployables;

            public static bool allowLysateStack = false;

            private void Awake()
            {
                body = base.GetComponent<CharacterBody>();

                Beacon1Deployables = new Queue<GameObject>();
                Beacon2Deployables = new Queue<GameObject>();
            }

            private void Start()
            {
                skillLocator = base.GetComponent<SkillLocator>();

                Beacon1 = skillLocator.FindSkill("SupplyDrop1");
                Beacon2 = skillLocator.FindSkill("SupplyDrop2");
            }

            public void AddBeacon(GameObject newBeacon, GenericSkill skill)
            {
                if (!NetworkServer.active) return;  //Beacons being instantiated/deleted are server-side.

                //Bad way to get stock bonus, but Vanilla does this for Engi Turrets in the Deployable Mangaer code.
                int inventoryStocks = 0;
                if (body && body.inventory) inventoryStocks = body.inventory.GetItemCount(DLC1Content.Items.EquipmentMagazineVoid);

                int maxBeacons = 1 + inventoryStocks;
                if (!allowLysateStack && maxBeacons >= 2) maxBeacons = 2;
                if (skill == Beacon1)
                {
                    if (Beacon1Deployables.Count >= maxBeacons)
                    {
                        GameObject toRemove = Beacon1Deployables.Dequeue();
                        UnityEngine.Object.Destroy(toRemove);
                    }

                    Beacon1Deployables.Enqueue(newBeacon);
                }
                else if (skill == Beacon2)
                {
                    if (Beacon2Deployables.Count >= maxBeacons)
                    {
                        GameObject toRemove = Beacon2Deployables.Dequeue();
                        UnityEngine.Object.Destroy(toRemove);
                    }
                    Beacon2Deployables.Enqueue(newBeacon);
                }
            }

            private void OnDestroy()
            {
                while (Beacon1Deployables.Count > 0)
                {
                    UnityEngine.Object.Destroy(Beacon1Deployables.Dequeue());
                }
                while (Beacon2Deployables.Count > 0)
                {
                    UnityEngine.Object.Destroy(Beacon2Deployables.Dequeue());
                }
            }
        }

        public class HackMarker : MonoBehaviour
        {
            public static float costMult = 0.5f;

            public int newCost;
            public GameObject owner;
            public PurchaseInteraction purchaseInteraction;

            public void FixedUpdate()
            {
                //Undo price changes if the hack beacon is removed.
                if (!owner)
                {
                    if (purchaseInteraction)
                    {
                        purchaseInteraction.cost = Mathf.FloorToInt(purchaseInteraction.cost / costMult);
                    }
                    Destroy(this);
                }
            }
        }
    }
}
