using Moonstorm.Starstorm2.Modules;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Tweaks
{
    public class ShrineCombatItems
    {
        public static bool enabled = true;
        public static float whiteChance = 79.2f;
        public static float greenChance = 19.8f;
        public static float redChance = 0.99f;
        private static Vector3 rewardPositionOffset = new Vector3(0f, 6f, 0f);
        public ShrineCombatItems()
        {
            if (!enabled) return;
            On.RoR2.ShrineCombatBehavior.Awake += (orig, self) =>
            {
                orig(self);
                
                //bad way to do this
                if (self.combatDirector)
                {
                    if (self.combatDirector.expRewardCoefficient == 0.2f)
                    {
                        self.combatDirector.expRewardCoefficient = 0.067f;
                    }
                    else if (self.combatDirector.expRewardCoefficient == 0.067f)
                    {
                        self.combatDirector.expRewardCoefficient = 0.19f;
                    }
                }
            };
            On.RoR2.ShrineCombatBehavior.OnDefeatedServer += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    ItemTier tier = ItemTier.NoTier;
                    PickupIndex pickupIndex = SelectItem();
                    PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                    if (pickupIndex != PickupIndex.none)
                    {
                        int participatingPlayerCount = Run.instance.participatingPlayerCount;
                        if (participatingPlayerCount != 0 && self.transform)
                        {
                            int num = participatingPlayerCount;

                            PickupDropTable dropTable;
                            tier = pickupDef.itemTier;
                            switch (tier)
                            {
                                case ItemTier.Tier2:
                                    dropTable = RiskyMod.tier2Drops;
                                    if (SoftDependencies.ShareSuiteUncommon) num = 1;
                                    break;
                                case ItemTier.Tier3:
                                    dropTable = RiskyMod.tier3Drops;
                                    if (SoftDependencies.ShareSuiteLegendary) num = 1;
                                    break;
                                default:
                                    dropTable = RiskyMod.tier1Drops;
                                    if (SoftDependencies.ShareSuiteCommon) num = 1;
                                    break;
                            }

                            float angle = 360f / (float)num;
                            Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

                            //Roll the rng anyways so that it performs the same with/without the config option. This code is a mess.
                            PickupPickerController.Option[] options = PickupPickerController.GenerateOptionsFromDropTable(3, dropTable, Run.instance.bossRewardRng);
                            if (options.Length > 0 && options[0].pickupIndex != PickupIndex.none)
                            {
                                pickupDef = PickupCatalog.GetPickupDef(options[0].pickupIndex);
                                pickupIndex = pickupDef.pickupIndex;
                            }

                            int k = 0;
                            while (k < num)
                            {
                                if (!SoftDependencies.IsPotentialArtifactActive())
                                {
                                    PickupDropletController.CreatePickupDroplet(pickupDef.pickupIndex, self.transform.position + rewardPositionOffset, vector);
                                }
                                else
                                {
                                    PickupDropletController.CreatePickupDroplet(new GenericPickupController.CreatePickupInfo
                                    {
                                        pickerOptions = options,
                                        prefabOverride = RiskyMod.potentialPrefab,
                                        position = self.transform.position + rewardPositionOffset,
                                        rotation = Quaternion.identity,
                                        pickupIndex = PickupCatalog.FindPickupIndex(tier)
                                    }, vector);
                                }
                                k++;
                                vector = rotation * vector;
                            }

                            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                            {
                                baseToken = "SHRINE_COMBAT_END_TRIAL_RISKYMOD",
                                paramTokens = new string[]
                                {
                                    "<color=#"+ColorUtility.ToHtmlStringRGB(pickupDef.baseColor)+">"+Language.GetStringFormatted(pickupDef.nameToken) + "</color>"
                                }
                            });
                        }
                    }
                }
            };
        }

        private static PickupIndex SelectItem()
        {
            List<PickupIndex> list;
            Xoroshiro128Plus treasureRng = Run.instance.treasureRng;
            PickupIndex selectedPickup = PickupIndex.none;

            float total = whiteChance + greenChance + redChance;

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
                    list = Run.instance.availableTier3DropList;
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
