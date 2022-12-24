using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using RoR2.Skills;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies.DroneChanges
{
    public class GunnerTurret
    {
        public static bool allowRepair = true;
        public static bool teleportToMithrix = true;

        private static BodyIndex GunnerTurretIndex;

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
            cb.baseMaxHealth = 300f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseMaxShield = cb.baseMaxHealth * 0.1f;
            cb.levelMaxShield = cb.baseMaxShield * 0.3f;

            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                GunnerTurretIndex = BodyCatalog.FindBodyIndex("Turret1Body");
            };

            GameObject masterObject = LegacyResourcesAPI.Load<GameObject>("prefabs/charactermasters/turret1master");
            AISkillDriver[] skillDrivers = masterObject.GetComponents<AISkillDriver>();
            foreach (AISkillDriver sd in skillDrivers)
            {
                if (sd.skillSlot == SkillSlot.Primary && sd.maxDistance < 120f) sd.maxDistance = 120f;
            }


            if (teleportToMithrix)
            {
                On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += TeleportTurretToMithrix;
            }
        }

        private void TeleportTurretToMithrix(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
        {
            orig(self);
            
            ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
            foreach (TeamComponent tc in teamMembers)
            {
                if (tc.body && tc.body.bodyIndex == GunnerTurretIndex && tc.body.master && tc.body.master.minionOwnership && tc.body.master.minionOwnership.ownerMaster)
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
                            placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
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
