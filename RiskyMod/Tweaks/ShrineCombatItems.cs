using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Tweaks
{
    public class ShrineCombatItems
    {
        public static bool enabled = true;
        public static float whiteChance = 50f;
        public static float greenChance = 47.5f;
        public static float redChance = 2.5f;
        private static Vector3 rewardPositionOffset = new Vector3(0f, 6f, 0f);
        public ShrineCombatItems()
        {
            if (!enabled) return;
            On.RoR2.ShrineCombatBehavior.Awake += (orig, self) =>
            {
                orig(self);
                if (self.combatDirector)
                {
                    self.combatDirector.expRewardCoefficient *= 0.55f;  //0.2 -> 0.11
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
            Xoroshiro128Plus bossRewardRng = Run.instance.treasureRng;
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
