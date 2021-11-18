using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Moon
{
    public class PillarsDropItems
    {
        public static bool enabled = true;
        private static Vector3 rewardPositionOffset = new Vector3(0f, 3f, 0f);

        public static float whiteChance = 50f;
        public static float greenChance = 40f;
        public static float redChance = 10f;
        public static BossGroup teleBossGroup = Resources.Load<BossGroup>("prefabs/networkedobjects/teleporters/Teleporter1");

        public PillarsDropItems()
        {
            if (!enabled) return;

            On.RoR2.MoonBatteryMissionController.OnBatteryCharged += (orig, self, holdoutZone) =>
            {
                orig(self, holdoutZone);
                if (NetworkServer.active)
                {
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

                            PickupIndex pearlIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex);
                            PickupIndex shinyPearlIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex);

                            bool pearlAvailable = pearlIndex != PickupIndex.none && Run.instance.IsItemAvailable(RoR2Content.Items.Pearl.itemIndex);
                            bool shinyPearlAvailable = shinyPearlIndex != PickupIndex.none && Run.instance.IsItemAvailable(RoR2Content.Items.ShinyPearl.itemIndex);

                            int k = 0;
                            while (k < num)
                            {
                                bool overwritePickup = false;
                                PickupIndex pickupOverwrite = PickupIndex.none;
                                if ((pearlAvailable || shinyPearlAvailable) && teleBossGroup.bossDropChance > 0f && Run.instance.treasureRng.RangeFloat(0f, 100f) <= teleBossGroup.bossDropChance)
                                {
                                    if (pearlAvailable)
                                    {
                                        pickupOverwrite = pearlIndex;
                                        if (shinyPearlAvailable && Run.instance.treasureRng.RangeFloat(0f, 100f) <= 20f)
                                        {
                                            pickupOverwrite = shinyPearlIndex;
                                        }
                                    }
                                    else
                                    {
                                        pickupOverwrite = shinyPearlIndex;
                                    }
                                }
                                PickupDropletController.CreatePickupDroplet(overwritePickup ? pickupOverwrite : pickupIndex, holdoutZone.transform.position + rewardPositionOffset, vector);
                                k++;
                                vector = rotation * vector;
                            }
                        }
                    }
                }
            };
        }

        //Yellow Chance is handled after selecting item
        private static PickupIndex SelectItem()
        {
            List<PickupIndex> list;
            Xoroshiro128Plus bossRewardRng = Run.instance.bossRewardRng;
            PickupIndex selectedPickup = PickupIndex.none;

            float total = whiteChance + greenChance + redChance;

            if (bossRewardRng.RangeFloat(0f, total) <= whiteChance)//drop white
            {
                list = Run.instance.availableTier1DropList;
            }
            else
            {
                total -= whiteChance;
                if (bossRewardRng.RangeFloat(0f, total) <= greenChance)//drop green
                {
                    list = Run.instance.availableTier2DropList;
                }
                else
                {
                    total -= greenChance;
                    list = Run.instance.availableTier3DropList;
                }
            }
            if (list.Count > 0)
            {
                selectedPickup = bossRewardRng.NextElementUniform<PickupIndex>(list);
            }
            return selectedPickup;
        }
    }
}
