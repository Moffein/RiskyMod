using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.VoidLocus
{
    public class PillarsDropItems
    {
        public static bool enabled = true;
        private static Vector3 rewardPositionOffset = new Vector3(0f, 3f, 0f);

        public static float whiteChance = 50f;
        public static float greenChance = 40f;
        public static float redChance = 10f;

        public static float voidChance = 10f;

        public static bool usePotential = true;

        public PillarsDropItems()
        {
            if (!enabled) return;

            On.RoR2.HoldoutZoneController.OnDisable += (orig, self) =>
            {
                orig(self);

                if (NetworkServer.active)
                {
                    SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                    if (sd)
                    {
                        if (sd.baseSceneName.Equals("voidstage"))
                        {
                            HoldoutZoneController holdoutZone = self;
                            PickupDropTable dropTable = SelectItem();
                            PickupIndex pickupIndex = dropTable.GenerateDrop(Run.instance.bossRewardRng);
                            ItemTier tier = PickupCatalog.GetPickupDef(pickupIndex).itemTier;
                            if (pickupIndex != PickupIndex.none)
                            {
                                PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);

                                int participatingPlayerCount = Run.instance.participatingPlayerCount;
                                if (participatingPlayerCount != 0 && holdoutZone.transform)
                                {
                                    int num = participatingPlayerCount;

                                    bool itemShareActive = false;
                                    switch (tier)
                                    {
                                        case ItemTier.Tier1:
                                            if (SoftDependencies.ShareSuiteCommon)
                                            {
                                                num = 1;
                                            }
                                            break;
                                        case ItemTier.Tier2:
                                            if (SoftDependencies.ShareSuiteUncommon)
                                            {
                                                num = 1;
                                            }
                                            break;
                                        case ItemTier.Tier3:
                                            if (SoftDependencies.ShareSuiteLegendary)
                                            {
                                                num = 1;
                                            }
                                            break;
                                        case ItemTier.VoidTier1:
                                        case ItemTier.VoidTier2:
                                        case ItemTier.VoidTier3:
                                        case ItemTier.VoidBoss:
                                            if (SoftDependencies.ShareSuiteVoid)
                                            {
                                                num = 1;
                                            }
                                            break;
                                        default: break;
                                    }

                                    float angle = 360f / (float)num;
                                    Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                                    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

                                    //Roll the rng anyways so that it performs the same with/without the config option. This code is a mess.
                                    PickupPickerController.Option[] options = PickupPickerController.GenerateOptionsFromDropTable(3, dropTable, Run.instance.bossRewardRng);
                                    if (options.Length > 0 && options[0].pickupIndex != PickupIndex.none) pickupDef = PickupCatalog.GetPickupDef(options[0].pickupIndex);

                                    if (pickupDef != null && pickupDef.pickupIndex != PickupIndex.none)
                                    {
                                        Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                                        {
                                            baseToken = "VOID_SIGNAL_COMPLETE_RISKYMOD",
                                            paramTokens = new string[]
                                            {
                                                "<color=#"+ColorUtility.ToHtmlStringRGB(pickupDef.baseColor)+">"+Language.GetStringFormatted(pickupDef.nameToken) + "</color>"
                                            }
                                        });
                                    }

                                    int k = 0;
                                    while (k < num)
                                    {
                                        if (usePotential || SoftDependencies.IsPotentialArtifactActive())
                                        {
                                            PickupDropletController.CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
                                            {
                                                pickerOptions = options,
                                                prefabOverride = RiskyMod.potentialPrefab,
                                                position = holdoutZone.transform.position + rewardPositionOffset,
                                                rotation = Quaternion.identity,
                                                pickupIndex = PickupCatalog.FindPickupIndex(tier)
                                            }, vector);
                                        }
                                        else
                                        {
                                            PickupDropletController.CreatePickupDroplet(pickupIndex, holdoutZone.transform.position + rewardPositionOffset, vector);
                                        }
                                        k++;
                                        vector = rotation * vector;
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        //Yellow Chance is handled after selecting item
        private static PickupDropTable SelectItem()
        {
            Xoroshiro128Plus bossRewardRng = Run.instance.bossRewardRng;

            float total = whiteChance + greenChance + redChance + voidChance;

            if (bossRewardRng.RangeFloat(0f, total) <= whiteChance)//drop white
            {
                return RiskyMod.tier1Drops;
            }
            else
            {
                total -= whiteChance;
                if (bossRewardRng.RangeFloat(0f, total) <= greenChance)//drop green
                {
                    return RiskyMod.tier2Drops;
                }
                else
                {
                    total -= greenChance;
                    if ((bossRewardRng.RangeFloat(0f, total) <= redChance))
                    {
                        return RiskyMod.tier3Drops;
                    }
                    else
                    {
                        return RiskyMod.tierVoidDrops;
                    }
                }
            }
        }
    }
}
