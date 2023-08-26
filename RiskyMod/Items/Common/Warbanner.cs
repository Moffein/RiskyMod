using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.MonoBehaviours;
using RiskyMod.SharedHooks;
using RoR2;
using System;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Items.Common
{
    public class Warbanner
    {
        public static bool enabled = true;
        public static GameObject WarbannerObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");
        public static BuffDef warbannerBuff;

        public static bool UseModdedBuff = true;

        public Warbanner()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            if (UseModdedBuff)
            {
                SetupBuff();
                RecalculateStatsAPI.GetStatCoefficients += HandleStatsCustom;
            }
            else
            {
                WardHeal wh = WarbannerObject.AddComponent<WardHeal>();
                wh.healInterval = 1f;
                wh.healFraction = 0.01f;
                RecalculateStatsAPI.GetStatCoefficients += HandleStatsVanilla;
            }

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
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.WardOnLevel);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.WardOnLevel);
            SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.WardOnLevel, ItemTag.Healing);
        }

        private void SetupBuff()
        {
            BuffDef vanillaWarbanner = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/WardOnLevel/bdWarbanner.asset").WaitForCompletion();

            warbannerBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyMod_WarbannerBuff",
                false,
                false,
                false,
                vanillaWarbanner.buffColor,
                vanillaWarbanner.iconSprite
                );


            IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Warbanner")
                    ))
                {
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasBuff, self) =>
                    {
                        return hasBuff || self.HasBuff(Warbanner.warbannerBuff);
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Warbanner UpdateAllTemporaryVisualEffects IL Hook failed");
                }
            };

            WarbannerObject = WarbannerObject.InstantiateClone("RiskyModWarbannerObject", true);
            Content.Content.networkedObjectPrefabs.Add(WarbannerObject);    //Seems like InstantiateClone auto does this for some reason. TODO: Address, though it seems harmless.
            WardHeal wh = WarbannerObject.AddComponent<WardHeal>();
            wh.healInterval = 1f;
            wh.healFraction = 0.01f;

            BuffWard ward = WarbannerObject.GetComponent<BuffWard>();
            ward.buffDef = warbannerBuff;

            RoR2.RoR2Application.onLoad += ReplaceWarbanner;

            IL.RoR2.TeleporterInteraction.ChargingState.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(x => x.MatchLdstr("Prefabs/NetworkedObjects/WarbannerWard")))
                {
                    c.Index += 2;
                    c.EmitDelegate<Func<GameObject, GameObject>>(wardObject =>
                    {
                        return WarbannerObject;
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: Warbanner TeleporterInteraction IL Hook failed.");
                }
            };
        }

        private void ReplaceWarbanner()
        {
            if (!RoR2.Items.WardOnLevelManager.wardPrefab) Debug.LogError("RiskyMod: Warbanner assigned before WardOnLevel Init");
            RoR2.Items.WardOnLevelManager.wardPrefab = WarbannerObject;
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

        private void HandleStatsCustom(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(warbannerBuff))
            {
                args.moveSpeedMultAdd += 0.3f;
                args.attackSpeedMultAdd += 0.3f;
                args.damageMultAdd += 0.15f;
            }
        }

        private void HandleStatsVanilla(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(warbannerBuff))
            {
                //+30% AtkSpd and MoveSpd already present in vanilla
                args.damageMultAdd += 0.15f;
            }
        }
    }
}
