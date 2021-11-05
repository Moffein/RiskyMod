using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.MoonRework
{
    public class PillarsDropItems
    {
        public static bool enabled = true;
        private static Vector3 rewardPositionOffset = new Vector3(0f, 3f, 0f);

        public PillarsDropItems()
        {
            if (!enabled) return;

            On.RoR2.MoonBatteryMissionController.OnBatteryCharged += (orig, self, holdoutZone) =>
            {
                orig(self, holdoutZone);
                PickupIndex pickupIndex = SelectItem();
                if (pickupIndex != PickupIndex.none)
                {
                    int participatingPlayerCount = Run.instance.participatingPlayerCount;
                    if (participatingPlayerCount != 0 && holdoutZone.transform)
                    {
                        int num = participatingPlayerCount;
                        float angle = 360f / (float)num;
                        Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
                        int k = 0;
                        while (k < num)
                        {
                            PickupDropletController.CreatePickupDroplet(pickupIndex, holdoutZone.transform.position + rewardPositionOffset, vector);
                            k++;
                            vector = rotation * vector;
                        }
                    }
                }
            };
        }

        private static PickupIndex SelectItem()
        {
            List<PickupIndex> list;
            Xoroshiro128Plus treasureRng = Run.instance.runRNG;
            PickupIndex selectedPickup = PickupIndex.none;

            float whiteChance = 0f;
            float greenChance = 70f;
            float redChance = 15f;
            float yellowChance = 15f;

            float total = whiteChance + greenChance + redChance + yellowChance;

            if (treasureRng.RangeFloat(0f, total) <= whiteChance)//drop white
            {
                list = Run.instance.availableTier1DropList;
            }
            else
            {
                total -= whiteChance;
                if (treasureRng.RangeFloat(0f, total) <= greenChance)//drop green
                {
                    list = Run.instance.availableTier2DropList;
                }
                else
                {
                    total -= greenChance;
                    if (treasureRng.RangeFloat(0f, total) <= redChance)//drop red
                    {
                        list = Run.instance.availableTier3DropList;
                    }
                    else
                    {
                        //There's probably a better way of doing this.
                        PickupIndex pearlIndex;
                        PickupIndex shinyPearlIndex;

                        list = new List<PickupIndex>();
                        pearlIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex);
                        if (pearlIndex != PickupIndex.none && Run.instance.availableBossDropList.Contains(pearlIndex))
                        {
                            list.Add(pearlIndex);
                        }
                        shinyPearlIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex);
                        if (shinyPearlIndex != PickupIndex.none && Run.instance.availableBossDropList.Contains(shinyPearlIndex))
                        {
                            list.Add(shinyPearlIndex);
                        }
                        if (list.Count > 0)
                        {
                            if (pearlIndex != PickupIndex.none && shinyPearlIndex != PickupIndex.none)
                            {
                                //Shiny pearl is locked behind 20% random chance.
                                if (treasureRng.RangeInt(0, 5) != 0)
                                {
                                    list.Remove(shinyPearlIndex);
                                }
                            }
                        }
                        else
                        {
                            list = Run.instance.availableTier2DropList;
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                selectedPickup = treasureRng.NextElementUniform<PickupIndex>(list);
            }
            return selectedPickup;
        }
    }
}
