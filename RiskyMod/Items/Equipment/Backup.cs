using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.Equipment
{
    public class Backup
    {
        public static bool enabled = true;
        public static bool ignoreTeamLimit = true;
        public static BodyIndex BackupDroneIndex;
        public static GameObject backupMaster = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/DroneBackupMaster");
        public Backup()
        {
            if (!enabled)
            {
                HandleBackupVanilla();
                return;
            }
            On.RoR2.EquipmentSlot.FireDroneBackup += (orig, self) =>
            {
                int sliceCount = 4;
                float num = 25f;
                if (NetworkServer.active)
                {
                    BackupTracker bt = self.gameObject.GetComponent<BackupTracker>();
                    if (!bt)
                    {
                        bt = self.gameObject.AddComponent<BackupTracker>();
                    }

                    if (!bt.canSpawn)
                    {
                        return false;
                    }

                    float y = Quaternion.LookRotation(self.GetAimRay().direction).eulerAngles.y;
                    float d = 3f;
                    int spawnCount = 0;
                    foreach (float num2 in new DegreeSlices(sliceCount, 0.5f))
                    {
                        if (bt.canSpawn)
                        {
                            Quaternion rotation = Quaternion.Euler(0f, y + num2, 0f);
                            Quaternion rotation2 = Quaternion.Euler(0f, y + num2 + 180f, 0f);
                            Vector3 position = self.transform.position + rotation * (Vector3.forward * d);
                            CharacterMaster characterMaster = new MasterSummon
                            {
                                masterPrefab = backupMaster,
                                position = position,
                                rotation = rotation,
                                summonerBodyObject = self.gameObject,
                                ignoreTeamMemberLimit = ignoreTeamLimit,
                                useAmbientLevel = true
                            }.Perform();
                            if (characterMaster)
                            {
                                spawnCount++;
                                CharacterBody cb = characterMaster.GetBody();
                                if (cb && cb.healthComponent)
                                {
                                    MasterSuicideOnTimer msot = characterMaster.gameObject.AddComponent<MasterSuicideOnTimer>();
                                    msot.lifeTimer = num + UnityEngine.Random.Range(0f, 3f);
                                    bt.AddHealthcomponent(cb.healthComponent);
                                }

                                if (characterMaster.teamIndex == TeamIndex.Player && characterMaster.inventory)
                                {
                                    characterMaster.inventory.GiveItem(Allies.AllyItems.AllyMarkerItem);
                                    characterMaster.inventory.GiveItem(Allies.AllyItems.AllyScalingItem);
                                    characterMaster.inventory.GiveItem(Allies.AllyItems.AllyAllowOverheatDeathItem);
                                    characterMaster.inventory.GiveItem(Allies.AllyItems.AllyAllowVoidDeathItem);
                                }
                            }
                        }
                    }

                    if (spawnCount < sliceCount)
                    {
                        float cooldownRestorePercent = 1f - ((float)spawnCount / (float)sliceCount);
                        if (self.inventory && self.equipmentIndex == RoR2Content.Equipment.DroneBackup.equipmentIndex)  //Check index just in case it triggered from Chaos or something.
                        {
                            self.inventory.DeductActiveEquipmentCooldown(cooldownRestorePercent * self.inventory.CalculateEquipmentCooldownScale());
                        }
                    }
                }
                self.subcooldownTimer = 0.5f;
                return true;
            };
        }

        private static void HandleBackupVanilla()
        {
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                Backup.BackupDroneIndex = BodyCatalog.FindBodyIndex("BackupDroneBody");
            };

            On.RoR2.CharacterBody.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && !self.isPlayerControlled && self.bodyIndex == Backup.BackupDroneIndex && self.teamComponent && self.teamComponent.teamIndex == TeamIndex.Player)
                {
                    if (self.inventory)
                    {
                        self.inventory.GiveItem(Allies.AllyItems.AllyMarkerItem);
                        self.inventory.GiveItem(Allies.AllyItems.AllyScalingItem);
                        self.inventory.GiveItem(Allies.AllyItems.AllyRegenItem, 40);
                    }
                }
            };
        }
    }

    public class BackupTracker : MonoBehaviour
    {
        public static int maxCount = 8;

        private List<HealthComponent> droneList;

        public void FixedUpdate()
        {
            if (droneList.Count > 0)
            {
                List<HealthComponent> toRemove = new List<HealthComponent>();
                foreach (HealthComponent h in droneList)
                {
                    if (!h || !h.alive)
                    {
                        toRemove.Add(h);
                    }
                }

                if (toRemove.Count > 0)
                {
                    foreach (HealthComponent h in toRemove)
                    {
                        droneList.Remove(h);
                    }
                }
            }
        }

        public void AddHealthcomponent(HealthComponent h)
        {
            droneList.Add(h);
        }

        public void Awake()
        {
            droneList = new List<HealthComponent>();
        }

        public bool canSpawn
        {
            get
            {
                return droneList.Count < maxCount;
            }
        }
    };
}
