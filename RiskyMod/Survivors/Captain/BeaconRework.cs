using MonoMod.Cil;
using RoR2;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Captain
{
    public class BeaconRework
    {
        public BeaconRework(SkillLocator sk)
        {
            GenericSkill beacon1;
            GenericSkill beacon2;

            //Debug.Log("Modifying Beacons");
            GenericSkill[] allSkills = CaptainCore.bodyPrefab.GetComponents<GenericSkill>();    //sk.allSkills nullrefs
            foreach(GenericSkill s in allSkills)
            {
                //Debug.Log(s.skillName);
                if (s.skillName == "SupplyDrop1")
                {
                    beacon1 = s;
                }
                else if (s.skillName == "SupplyDrop2")
                {
                    beacon2 = s;
                }

                AddCooldown("RoR2/Base/Captain/CallSupplyDropEquipmentRestock.asset");
                AddCooldown("RoR2/Base/Captain/CallSupplyDropHacking.asset");
                AddCooldown("RoR2/Base/Captain/CallSupplyDropHealing.asset");
                AddCooldown("RoR2/Base/Captain/CallSupplyDropShocking.asset");
            }
        }

        private void AddCooldown(string address)
        {
            SkillDef sd = Addressables.LoadAssetAsync<SkillDef>(address).WaitForCompletion();
            sd.rechargeStock = 1;
            sd.baseRechargeInterval = 30f;
            sd.baseMaxStock = 1;
            sd.beginSkillCooldownOnSkillEnd = false;

            On.RoR2.CaptainSupplyDropController.SetSkillOverride += CaptainSupplyDropController_SetSkillOverride;
        }

        private void CaptainSupplyDropController_SetSkillOverride(On.RoR2.CaptainSupplyDropController.orig_SetSkillOverride orig, CaptainSupplyDropController self, ref SkillDef currentSkillDef, SkillDef newSkillDef, GenericSkill component)
        {
            newSkillDef = currentSkillDef;
            orig(self, ref currentSkillDef, newSkillDef, component);
        }
    }
}
