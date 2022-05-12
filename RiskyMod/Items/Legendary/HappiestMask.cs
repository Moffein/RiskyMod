using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.Legendary
{
    public class HappiestMask
    {
        public static bool enabled = true;
        public static BuffDef GhostCooldown;
        public static BuffDef GhostReady;

        public static bool scaleCount = false;
        public static bool noGhostLimit = false;

        public HappiestMask()
        {
            if (!enabled) return;
            
            ItemsCore.ModifyItemDefActions += ModifyItem;

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "GhostOnKill")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            GhostReady = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_GhostReady",
                false,
                false,
                false,
                new Color(0.9f, 0.9f, 0.9f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/BanditSkull").iconSprite
                );

            GhostCooldown = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_GhostCooldown",
                true,
                true,
                false,
                new Color(88f / 255f, 91f / 255f, 98f / 255f),
                LegacyResourcesAPI.Load<BuffDef>("BuffDefs/BanditSkull").iconSprite
                );

            OnCharacterDeath.OnCharacterDeathInventoryActions += TriggerMaskGhost;
            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    self.AddItemBehavior<GhostOnKillBehavior>(self.inventory.GetItemCount(RoR2Content.Items.GhostOnKill));
                }
            };
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.GhostOnKill);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.GhostOnKill);
        }

        private static void TriggerMaskGhost(GlobalEventManager self, DamageReport damageReport, CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody)
        {
            int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.GhostOnKill);
            if (itemCount > 0)
            {
                if (attackerBody.HasBuff(GhostReady.buffIndex))
                {
                    TeamIndex attackerTeam = attackerBody.teamComponent ? attackerBody.teamComponent.teamIndex : TeamIndex.None;
                    TeamIndex victimTeam = victimBody.teamComponent ? victimBody.teamComponent.teamIndex : TeamIndex.None;

                    if (attackerTeam == TeamIndex.Player || victimTeam != TeamIndex.Player || victimBody.isPlayerControlled)
                    {
                        GhostOnKillBehavior gokb = attackerBody.GetComponent<GhostOnKillBehavior>();
                        if (gokb && gokb.CanSpawnGhost())
                        {
                            gokb.AddGhost(SpawnMaskGhost(victimBody, attackerBody, itemCount));
                            for (int i = 1; i <= 20; i++)
                            {
                                attackerBody.AddTimedBuff(GhostCooldown.buffIndex, i);
                            }
                        }
                    }
                }
            }
        }

        public static CharacterBody SpawnMaskGhost(CharacterBody targetBody, CharacterBody ownerBody, int itemCount)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'RoR2.CharacterBody RoR2.Util::TryToCreateGhost(RoR2.CharacterBody, RoR2.CharacterBody, int)' called on client");
                return null;
            }
            if (!targetBody)
            {
                return null;
            }
            GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(targetBody);
            if (!bodyPrefab)
			{
                return null;
            }
            CharacterMaster characterMaster = MasterCatalog.allAiMasters.FirstOrDefault((CharacterMaster master) => master.bodyPrefab == bodyPrefab);
            if (!characterMaster)
            {
                return null;
            }
            MasterSummon masterSummon = new MasterSummon();
            masterSummon.masterPrefab = characterMaster.gameObject;
            masterSummon.ignoreTeamMemberLimit = true;
            masterSummon.position = targetBody.footPosition;
            CharacterDirection component = targetBody.GetComponent<CharacterDirection>();
            masterSummon.rotation = (component ? Quaternion.Euler(0f, component.yaw, 0f) : targetBody.transform.rotation);
            masterSummon.summonerBodyObject = (ownerBody ? ownerBody.gameObject : null);
            masterSummon.inventoryToCopy = targetBody.inventory;
            masterSummon.useAmbientLevel = null;
            CharacterMaster characterMaster2 = masterSummon.Perform();
            if (!characterMaster2)
            {
                return null;
            }
            else
            {
                Inventory inventory = characterMaster2.inventory;
                if (inventory)
                {
                    if (inventory.GetItemCount(RoR2Content.Items.Ghost) <= 0) inventory.GiveItem(RoR2Content.Items.Ghost);
                    if (inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0) inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);

                    if (ownerBody && ownerBody.teamComponent && ownerBody.teamComponent.teamIndex == TeamIndex.Player)
                    {
                        inventory.GiveItem(RoR2Content.Items.BoostDamage.itemIndex, 105 + 45 * itemCount);
                        inventory.GiveItem(RoR2Content.Items.HealthDecay.itemIndex, 30 * itemCount);
                    }
                    else //Handle enemy-spawned ghosts
                    {
                        inventory.GiveItem(RoR2Content.Items.HealthDecay.itemIndex, 20 * itemCount);
                        MasterSuicideOnTimer mst = characterMaster2.gameObject.AddComponent<MasterSuicideOnTimer>();
                        mst.lifeTimer = 20f * itemCount;
                    }
                }
            }
            CharacterBody body = characterMaster2.GetBody();
            if (body)
            {
                foreach (EntityStateMachine entityStateMachine in body.GetComponents<EntityStateMachine>())
                {
                    entityStateMachine.initialStateType = entityStateMachine.mainStateType;
                }
            }
            return body;
        }

        public class GhostOnKillBehavior : CharacterBody.ItemBehavior
        {
            private List<CharacterBody> activeGhosts;

            private void Awake()
            {
                activeGhosts = new List<CharacterBody>();
            }

            private void FixedUpdate()
            {
                bool flag = this.body.HasBuff(GhostCooldown.buffIndex);
                bool flag2 = this.body.HasBuff(GhostReady.buffIndex);
                if (!flag && !flag2)
                {
                    this.body.AddBuff(GhostReady.buffIndex);
                }
                if (flag2 && flag)
                {
                    this.body.RemoveBuff(GhostReady.buffIndex);
                }

                UpdateGhosts();
            }

            private void UpdateGhosts()
            {
                List<CharacterBody> toRemove = new List<CharacterBody>();
                foreach(CharacterBody cb in activeGhosts)
                {
                    if (!(cb && cb.healthComponent && cb.healthComponent.alive))
                    {
                        toRemove.Add(cb);
                    }
                }

                foreach(CharacterBody cb in toRemove)
                {
                    activeGhosts.Remove(cb);
                }
            }

            public bool CanSpawnGhost()
            {
                int itemCount = base.body.inventory.GetItemCount(RoR2Content.Items.GhostOnKill.itemIndex);
                if (itemCount <= 0) return false;

                if (noGhostLimit) return true;

                int maxGhosts = 3;
                if (scaleCount)
                {
                    maxGhosts += itemCount - 1;
                }
                return activeGhosts.Count < maxGhosts;
            }

            public void AddGhost(CharacterBody cb)
            {
                activeGhosts.Add(cb);
            }
        }
    }
}
