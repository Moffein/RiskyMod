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
        public static ItemDef itemDef = RoR2Content.Items.WardOnLevel;
        public static GameObject WarbannerObject;

        public Warbanner()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, itemDef);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, itemDef);

            WarbannerObject = Resources.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");

            On.RoR2.HealthComponent.Heal += (orig, self, amount, procChainMask, nonRegen) =>
            {
                if (self.body && self.body.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
                {
                    amount *= 1.3f;
                }
                return orig(self, amount, procChainMask, nonRegen);
            };

            RecalculateStatsAPI.GetStatCoefficients += HandleStats;

            On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += (orig, self) =>
            {
                orig(self);
                SpawnBanners();
            };
            On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += (orig, self) =>
            {
                orig(self);
                SpawnBanners();
            };
            On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += (orig, self) =>
            {
                orig(self);
                SpawnBanners();
            };
            On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += (orig, self) =>
            {
                orig(self);
                SpawnBanners();
            };
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
