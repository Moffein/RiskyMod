using RoR2;
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
                if (self.combatDirector)
                {
                    self.combatDirector.expRewardCoefficient *= 0.3335f;  //0.2 -> 0.067
                }
            };
            On.RoR2.ShrineCombatBehavior.OnDefeatedServer += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    PickupIndex pickupIndex = SelectItem();
                    if (pickupIndex != PickupIndex.none)
                    {
                        int participatingPlayerCount = Run.instance.participatingPlayerCount;
                        if (participatingPlayerCount != 0 && self.transform)
                        {
                            int num = participatingPlayerCount;

                            switch (PickupCatalog.GetPickupDef(pickupIndex).itemTier)
                            {
                                case ItemTier.Tier1:
                                    if (RiskyMod.ShareSuiteCommon) num = 1;
                                    break;
                                case ItemTier.Tier2:
                                    if (RiskyMod.ShareSuiteUncommon) num = 1;
                                    break;
                                case ItemTier.Tier3:
                                    if (RiskyMod.ShareSuiteLegendary) num = 1;
                                    break;
                                default: break;
                            }

                            float angle = 360f / (float)num;
                            Vector3 vector = Quaternion.AngleAxis((float)UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);

                            int k = 0;
                            while (k < num)
                            {
                                PickupDropletController.CreatePickupDroplet(pickupIndex, self.transform.position + rewardPositionOffset, vector);
                                k++;
                                vector = rotation * vector;
                            }
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
