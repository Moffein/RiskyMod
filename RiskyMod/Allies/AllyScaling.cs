using RoR2;
using RoR2.CharacterAI;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RiskyMod.Allies
{
    public class AllyScaling
    {

        public static bool noVoidDeath = true;
        public static bool noOverheat = true;

        public AllyScaling()
        {
            On.RoR2.SummonMasterBehavior.OpenSummonReturnMaster += (orig, self, activator) =>
            {
                CharacterMaster summonResult = orig(self, activator);
                if (summonResult)
                {
                    if (summonResult.teamIndex == TeamIndex.Player && summonResult.inventory)
                    {
                        if (AllyPrefabs.IsPurchaseableDrone(self.masterPrefab))
                        {
                            summonResult.inventory.GiveItem(AllyItems.AllyMarkerItem);
                            summonResult.inventory.GiveItem(AllyItems.AllyScalingItem);

                            if (self.masterPrefab == AllyPrefabs.IncineratorDrone)
                            {
                                summonResult.inventory.GiveItem(AllyItems.AllyRegenItem, 20);
                            }
                            else if (self.masterPrefab == AllyPrefabs.MegaDrone)
                            {
                                summonResult.inventory.GiveItem(AllyItems.AllyRegenItem, 30);
                            }
                            else
                            {
                                summonResult.inventory.GiveItem(AllyItems.AllyRegenItem, 40);
                            }

                            if (self.masterPrefab == AllyPrefabs.GunnerTurret)
                            {
                                summonResult.inventory.GiveItem(AllyItems.AllyResistAoEItem);
                            }
                        }
                    }
                }
                return summonResult;
            };

            On.RoR2.DroneWeaponsBehavior.OnMasterSpawned += (orig, self, spawnResult) =>
            {
                orig(self, spawnResult);
                if (spawnResult.spawnedInstance)
                {
                    CharacterMaster cm = spawnResult.spawnedInstance.GetComponent<CharacterMaster>();
                    if (cm && cm.teamIndex == TeamIndex.Player && cm.inventory)
                    {
                        cm.inventory.GiveItem(AllyItems.AllyMarkerItem);
                        cm.inventory.GiveItem(AllyItems.AllyRegenItem, 40);
                        cm.inventory.GiveItem(AllyItems.AllyScalingItem);
                    }
                }
            };
        }
    }
}
