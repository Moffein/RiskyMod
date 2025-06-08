using RiskyMod.Enemies;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using RoR2.Skills;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies.DroneChanges
{
    public class GunnerTurret
    {
        public static bool allowRepair = true;
        public static bool teleportWithPlayer = true;
        public static bool teleportToMithrix = true;
        public static bool weakToMithrix = true;

        private static BodyIndex gunnerTurretBodyIndex;

        public GunnerTurret()
        {
            GameObject gunnerTurret = AllyPrefabs.GunnerTurret;

            SkillDef turretSkill = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Drones/Turret1BodyTurret.asset").WaitForCompletion();
            turretSkill.baseMaxStock = 1;
            turretSkill.baseRechargeInterval = 0f;

            if (GunnerTurret.allowRepair)
            {
                CharacterDeathBehavior cdb = gunnerTurret.GetComponent<CharacterDeathBehavior>();
                if (cdb)
                {
                    cdb.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyMod.Turret1.Turret1DeathState));
                }
            }
            Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.Turret1.Turret1DeathState));

            //Gets run before scaling changes
            CharacterBody cb = gunnerTurret.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 400f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseMaxShield = cb.baseMaxHealth * 0.1f;
            cb.levelMaxShield = cb.baseMaxShield * 0.3f;
            cb.baseArmor = 20f;

            RoR2Application.onLoadFinished += OnLoad;

            GameObject masterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/Turret1Master.prefab").WaitForCompletion();
            AISkillDriver[] skillDrivers = masterObject.GetComponents<AISkillDriver>();
            foreach (AISkillDriver sd in skillDrivers)
            {
                if (sd.skillSlot == SkillSlot.Primary && sd.maxDistance < 90f) sd.maxDistance = 90f;
            }


            if (teleportWithPlayer)
            {
                if (teleportToMithrix) On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += TeleportTurretToMithrix;

                if (!SoftDependencies.TeleporterTurretsLoaded)
                {
                    On.RoR2.HoldoutZoneController.OnEnable += HoldoutZoneController_OnEnable;
                }
                else
                {
                    Debug.LogError("RiskyMod: Disabling GunnerTurret teleport because TeleporterTurrets is installed.");
                }
            }

            if (weakToMithrix)
            {
                SharedHooks.TakeDamage.ModifyInitialDamageActions += WeakToMithrixHook;
            }
        }

        private void OnLoad()
        {
            gunnerTurretBodyIndex = BodyCatalog.FindBodyIndex("Turret1Body");
        }

        private void WeakToMithrixHook(DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody)
        {
            if (self.body.bodyIndex == gunnerTurretBodyIndex
                && (attackerBody.bodyIndex == Enemies.Mithrix.MithrixCore.brotherBodyIndex
                || attackerBody.bodyIndex == Enemies.Mithrix.MithrixCore.brotherHurtBodyIndex))
            {
                damageInfo.damage *= 2f;
            }
        }

        private void HoldoutZoneController_OnEnable(On.RoR2.HoldoutZoneController.orig_OnEnable orig, HoldoutZoneController self)
        {
            orig(self);
            TeleportTurretsToPlayer();
        }

        private void TeleportTurretToMithrix(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
        {
            orig(self);
            TeleportTurretsToPlayer();
        }

        public static void TeleportTurretsToPlayer()
        {
            ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
            foreach (TeamComponent tc in teamMembers)
            {
                if (tc.body && tc.body.bodyIndex == gunnerTurretBodyIndex && tc.body.master && tc.body.master.minionOwnership && tc.body.master.minionOwnership.ownerMaster)
                {
                    CharacterBody ownerBody = tc.body.master.minionOwnership.ownerMaster.GetBody();
                    if (ownerBody)
                    {
                        //Copied from Chirr
                        SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
                        spawnCard.hullSize = tc.body.hullClassification;
                        spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
                        spawnCard.prefab = SneedUtils.SneedUtils.teleportHelperPrefab;

                        GameObject teleportDestinationHelper = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                            position = ownerBody.corePosition,
                            minDistance = 5f,
                            maxDistance = 45f
                        }, RoR2Application.rng));

                        if (teleportDestinationHelper)
                        {
                            Vector3 position = teleportDestinationHelper.transform.position;
                            TeleportHelper.TeleportBody(tc.body, position);
                            GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(tc.body.gameObject);
                            if (teleportEffectPrefab)
                            {
                                EffectManager.SimpleEffect(teleportEffectPrefab, position, Quaternion.identity, true);
                            }
                            UnityEngine.Object.Destroy(teleportDestinationHelper);
                        }
                    }
                }
            }
        }
    }
}
