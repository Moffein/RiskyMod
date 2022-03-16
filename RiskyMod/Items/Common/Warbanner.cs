using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.Common
{
    public class Warbanner
    {
        public static bool enabled = true;
        public static GameObject WarbannerObject;

        public Warbanner()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            WarbannerObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");

            RecalculateStatsAPI.GetStatCoefficients += HandleStats;

            On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += (orig, self) =>
            {
                orig(self);
                SpawnBanners();
            };

            On.EntityStates.InfiniteTowerSafeWard.Active.OnEnter += (orig, self) =>
            {
                orig(self);
                SpawnBanners();
            };

            //Does this work online?
            /*On.RoR2.VoidRaidGauntletExitController.OnBodyTeleport += (orig, self, body) =>
            {
                orig(self, body);
                if (NetworkServer.active)
                {
                    SpawnBanner(body);
                }
            };*/
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.WardOnLevel);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.WardOnLevel);
        }

        private void SpawnBanner(CharacterBody body)
        {
            if (body.inventory && body.teamComponent && body.teamComponent.teamIndex == TeamIndex.Player)
            {
                int itemCount = body.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                if (itemCount > 0)
                {
                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(WarbannerObject, body.transform.position, Quaternion.identity);
                    gameObject.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
                    gameObject.GetComponent<BuffWard>().Networkradius = 8f + 8f * (float)itemCount;
                    NetworkServer.Spawn(gameObject);
                }
            }
        }

        private void SpawnBanners()
        {
            //Taken from TeleporterInteraction
            ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(TeamIndex.Player);
            for (int j = 0; j < teamMembers.Count; j++)
            {
                TeamComponent teamComponent = teamMembers[j];
                CharacterBody body = teamComponent.body;
                if (body)
                {
                    CharacterMaster master = teamComponent.body.master;
                    if (master)
                    {
                        int itemCount = master.inventory.GetItemCount(RoR2Content.Items.WardOnLevel);
                        if (itemCount > 0)
                        {
                            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(WarbannerObject, body.transform.position, Quaternion.identity);
                            gameObject.GetComponent<TeamFilter>().teamIndex = TeamIndex.Player;
                            gameObject.GetComponent<BuffWard>().Networkradius = 8f + 8f * (float)itemCount;
                            NetworkServer.Spawn(gameObject);
                        }
                    }
                }
            }
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
            {
                //+30% AtkSpd and MoveSpd already present in vanilla
                args.armorAdd += 15f;
                args.damageMultAdd += 0.15f;
            }
        }
    }
}
