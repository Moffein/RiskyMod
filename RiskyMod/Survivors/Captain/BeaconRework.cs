using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
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
        public BeaconRework(SkillLocator sk)
        {
            AddCooldown("RoR2/Base/Captain/CallSupplyDropEquipmentRestock.asset");
            AddCooldown("RoR2/Base/Captain/CallSupplyDropHacking.asset");
            AddCooldown("RoR2/Base/Captain/CallSupplyDropHealing.asset");
            AddCooldown("RoR2/Base/Captain/CallSupplyDropShocking.asset");

            sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "CAPTAIN_SPECIAL_DESCRIPTION_RISKYMOD";

            //Register beacons when they spawn
            CaptainCore.bodyPrefab.AddComponent<CaptainDeployableManager>();
            IL.EntityStates.Captain.Weapon.CallSupplyDropBase.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCall<NetworkServer>("Spawn")
                   );
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
            };
        }

        private void AddCooldown(string address)
        {
            SkillDef sd = Addressables.LoadAssetAsync<SkillDef>(address).WaitForCompletion();
            sd.rechargeStock = 1;
            sd.baseRechargeInterval = 30f;
            sd.baseMaxStock = 1;
            sd.beginSkillCooldownOnSkillEnd = false;

            On.RoR2.CaptainSupplyDropController.SetSkillOverride +=
                (On.RoR2.CaptainSupplyDropController.orig_SetSkillOverride orig, CaptainSupplyDropController self, ref SkillDef currentSkillDef, SkillDef newSkillDef, GenericSkill component)  =>
            {
                newSkillDef = currentSkillDef;
                orig(self, ref currentSkillDef, newSkillDef, component);
            };
        }
    }

    public class CaptainDeployableManager : MonoBehaviour
    {
        private SkillLocator skillLocator;
        private CharacterBody body;

        private GenericSkill Beacon1;
        private GenericSkill Beacon2;

        private Queue<GameObject> Beacon1Deployables;
        private Queue<GameObject> Beacon2Deployables;

        public void Awake()
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
            int maxBeacons = Math.Min(skillLocator.special.maxStock, 2);
            if (skill == Beacon1)
            {
                if(Beacon1Deployables.Count >= maxBeacons)
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
    }
}
