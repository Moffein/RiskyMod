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
        public static GameObject backupMaster = Resources.Load<GameObject>("Prefabs/CharacterMasters/DroneBackupMaster");
        public Backup()
        {
            if (!enabled) return;
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
                                ignoreTeamMemberLimit = ignoreTeamLimit
                            }.Perform();
                            if (characterMaster)
                            {
                                CharacterBody cb = characterMaster.GetBody();
                                if (cb && cb.healthComponent)
                                {
                                    MasterSuicideOnTimer msot = characterMaster.gameObject.AddComponent<MasterSuicideOnTimer>();
                                    msot.lifeTimer = num + UnityEngine.Random.Range(0f, 3f);
                                    bt.AddHealthcomponent(cb.healthComponent);
                                }
                            }
                        }
                    }
                }
                self.subcooldownTimer = 0.5f;
                return true;
            };
        }
    }

    public class BackupTracker : MonoBehaviour
    {
        public static int maxCount = 4;

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
