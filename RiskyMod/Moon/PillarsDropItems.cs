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
        public static float lunarChance = 0f;

        public static float pearlOverwriteChance = 15f;

        public PillarsDropItems()
        {
            if (!enabled) return;

            On.RoR2.MoonBatteryMissionController.OnBatteryCharged += (orig, self, holdoutZone) =>
            {
                orig(self, holdoutZone);
                if (NetworkServer.active)
                {
                    PickupIndex pickupIndex = SelectItem();
                    ItemTier tier = PickupCatalog.GetPickupDef(pickupIndex).itemTier;
                    if (pickupIndex != PickupIndex.none)
                    {
                        string pillarName = "Pillar";

                        PurchaseInteraction pu = holdoutZone.gameObject.GetComponent<PurchaseInteraction>();
                        if (pu)
                        {
                            pillarName = pu.GetDisplayName();
                        }

                        PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);

                        int participatingPlayerCount = Run.instance.participatingPlayerCount;
                        if (participatingPlayerCount != 0 && holdoutZone.transform)
                        {
                            int num = participatingPlayerCount;

                            PickupDropTable dropTable;
                            bool itemShareActive = false;
                            switch (tier)
                            {
                                case ItemTier.Tier2:
                                    if (SoftDependencies.ShareSuiteUncommon)
                                    {
                                        num = 1;
                                        itemShareActive = true;
                                    }
                                    dropTable = RiskyMod.tier2Drops;
                                    break;
                                case ItemTier.Tier3:
                                    if (SoftDependencies.ShareSuiteLegendary)
                                    {
                                        num = 1;
                                        itemShareActive = true;
                                    }
                                    dropTable = RiskyMod.tier3Drops;
                                    break;
                                case ItemTier.Lunar:
                                    if (SoftDependencies.ShareSuiteLunar)
                                    {
                                        num = 1;
                                        itemShareActive = true;
                                    }
                                    dropTable = RiskyMod.lunarDrops;
                                    break;
                                default:
                                    if (SoftDependencies.ShareSuiteCommon)
                                    {
                                        num = 1;
                                        itemShareActive = true;
                                    }
                                    dropTable = RiskyMod.tier1Drops;
                                    break;
                            }

                            float angle = 360f / (float)num;
                            Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);


                            //Roll the rng anyways so that it performs the same with/without the config option. This code is a mess.
                            PickupPickerController.Option[] options = PickupPickerController.GenerateOptionsFromDropTable(3, dropTable, Run.instance.bossRewardRng);
                            if (options.Length > 0 && options[0].pickupIndex != PickupIndex.none) pickupDef = PickupCatalog.GetPickupDef(options[0].pickupIndex);

                            if ((pickupDef != null && pickupDef.pickupIndex != PickupIndex.none))
                            {
                                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                                {
                                    baseToken = "MOON_PILLAR_COMPLETE_RISKYMOD",
                                    paramTokens = new string[]
                                    {
                                pillarName,
                                "<color=#"+ColorUtility.ToHtmlStringRGB(pickupDef.baseColor)+">"+Language.GetStringFormatted(pickupDef.nameToken) + "</color>"
                                    }
                                });
                            }

                            int k = 0;
                            while (k < num)
                            {
                                PickupIndex pickupOverwrite = PickupIndex.none;
                                bool overwritePickup = false;
                                if (tier != ItemTier.Tier3 && !itemShareActive && !SoftDependencies.ShareSuiteBoss)
                                {
                                    float pearlChance = pearlOverwriteChance;
                                    float total = pearlChance;
                                    if (Run.instance.bossRewardRng.RangeFloat(0f, 100f) < pearlChance)
                                    {
                                        pickupOverwrite = SelectPearl();
                                    }

                                    overwritePickup = !(pickupOverwrite == PickupIndex.none);
                                }

                                if (!overwritePickup)
                                {
                                    if (!SoftDependencies.IsPotentialArtifactActive())
                                    {
                                        PickupDropletController.CreatePickupDroplet(pickupDef.pickupIndex, holdoutZone.transform.position + rewardPositionOffset, vector);
                                    }
                                    else
                                    {
                                        PickupDropletController.CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
                                        {
                                            pickupIndex = PickupCatalog.FindPickupIndex(tier),
                                            pickerOptions = options,
                                            rotation = Quaternion.identity,
                                            prefabOverride = RiskyMod.potentialPrefab
                                        }, holdoutZone.transform.position + rewardPositionOffset, vector);
                                    }
                                }
                                else
                                {
                                    PickupDropletController.CreatePickupDroplet(pickupOverwrite, holdoutZone.transform.position + rewardPositionOffset, vector);
                                }
                                k++;
                                vector = rotation * vector;
                            }
                        }
                    }
                }
            };
        }

        private static PickupIndex SelectPearl()
        {
            PickupIndex pearlIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.Pearl.itemIndex);
            PickupIndex shinyPearlIndex = PickupCatalog.FindPickupIndex(RoR2Content.Items.ShinyPearl.itemIndex);
            bool pearlAvailable = pearlIndex != PickupIndex.none && Run.instance.IsItemAvailable(RoR2Content.Items.Pearl.itemIndex);
            bool shinyPearlAvailable = shinyPearlIndex != PickupIndex.none && Run.instance.IsItemAvailable(RoR2Content.Items.ShinyPearl.itemIndex);

            PickupIndex toReturn = PickupIndex.none;
            if (pearlAvailable && shinyPearlAvailable)
            {
                toReturn = pearlIndex;
                if (Run.instance.bossRewardRng.RangeFloat(0f, 100f) <= 20f)
                {
                    toReturn = shinyPearlIndex;
                }
            }
            else
            {
                if (pearlAvailable)
                {
                    toReturn = pearlIndex;
                }
                else if (shinyPearlAvailable)
                {
                    toReturn = shinyPearlIndex;
                }
            }
            return toReturn;
        }

        //Yellow Chance is handled after selecting item
        private static PickupIndex SelectItem()
        {
            List<PickupIndex> list;
            Xoroshiro128Plus bossRewardRng = Run.instance.bossRewardRng;
            PickupIndex selectedPickup = PickupIndex.none;

            float total = whiteChance + greenChance + redChance + lunarChance;

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
                    if ((bossRewardRng.RangeFloat(0f, total) <= redChance))
                    {
                        list = Run.instance.availableTier3DropList;
                    }
                    else
                    {
                        list = Run.instance.availableLunarCombinedDropList;
                    }
                    
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
