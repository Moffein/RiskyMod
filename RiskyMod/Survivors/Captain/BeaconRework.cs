using HG;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Skills;
using System;
using System.Collections.Generic;
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
        }

        public static bool healCooldown = true;
        public static bool hackCooldown = true;
        public static bool shockCooldown = true;
        public static bool resupplyCooldown = true;

        public static bool hackChanges= true;
        public static bool shockChanges = true;
        public static bool resupplyChanges = true;

        public BeaconRework(SkillLocator sk)
        {
            Skills.BeaconResupply = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallSupplyDropEquipmentRestock.asset").WaitForCompletion();
            Skills.BeaconHacking = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallSupplyDropHacking.asset").WaitForCompletion();
            if (resupplyCooldown) AddCooldown(Skills.BeaconResupply);
            if (hackCooldown) AddCooldown(Skills.BeaconHacking);
            if (healCooldown) AddCooldown("RoR2/Base/Captain/CallSupplyDropHealing.asset");
            if (shockCooldown) AddCooldown("RoR2/Base/Captain/CallSupplyDropShocking.asset");

            sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "CAPTAIN_SPECIAL_DESCRIPTION_RISKYMOD";

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

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
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

        private void AddCooldown(string address)
        {
            SkillDef sd = Addressables.LoadAssetAsync<SkillDef>(address).WaitForCompletion();
            AddCooldown(sd);
        }

        private void AddCooldown(SkillDef sd)
        {
            sd.rechargeStock = 1;
            sd.baseRechargeInterval = 60f;
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
        }

        private void ModifyBeaconResupply(SkillLocator sk)
        {
            if (!resupplyChanges) return;
            GameObject beaconPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainSupplyDrop, EquipmentRestock.prefab").WaitForCompletion().InstantiateClone("RiskyMod_CaptainSupplyEquipmentRestock", true);
            EntityStateMachine esm = beaconPrefab.GetComponent<EntityStateMachine>();
            esm.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyMod.Captain.Beacon.BeaconEquipmentRestoreMain));
            Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.Captain.Beacon.BeaconEquipmentRestoreMain));
            Content.Content.networkedObjectPrefabs.Add(beaconPrefab);
        }
        private void ModifyBeaconHacking(SkillLocator sk)
        {
            if (!hackChanges) return;
            GameObject beaconPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainSupplyDrop, Hacking.prefab").WaitForCompletion().InstantiateClone("RiskyMod_CaptainSupplySkillRestock", true); ;//, Hacking
            EntityStateMachine esm = beaconPrefab.GetComponent<EntityStateMachine>();
            esm.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyMod.Captain.Beacon.BeaconSkillRestoreMain));
            Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.Captain.Beacon.BeaconSkillRestoreMain));

            Content.Content.networkedObjectPrefabs.Add(beaconPrefab);
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Captain.Weapon.CallSupplyDropHacking", "supplyDropPrefab", beaconPrefab);
            Skills.BeaconHacking.skillDescriptionToken = "CAPTAIN_SUPPLY_SKILL_RESTOCK_DESCRIPTION_RISKYMOD";

            //Credits to DestroyedClone for this code
            UnityEngine.Object.Destroy(beaconPrefab.GetComponent<ModelLocator>()?.modelTransform?.Find("Indicator").gameObject);
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
                skillLocator = base.GetComponent<SkillLocator>();

                Beacon1 = skillLocator.FindSkill("SupplyDrop1");
                Beacon2 = skillLocator.FindSkill("SupplyDrop2");

                Beacon1Deployables = new Queue<GameObject>();
                Beacon2Deployables = new Queue<GameObject>();
            }

            public void AddBeacon(GameObject newBeacon, GenericSkill skill)
            {
                if (!NetworkServer.active) return;  //Beacons being instantiated/deleted are server-side.
                int maxBeacons = skillLocator.special.maxStock;
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
    }
}
