using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
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
        public static GameObject WarbannerObject;
        public static BuffDef warbannerBuff;

        public static bool UseModdedBuff = true;

        public Warbanner()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            WarbannerObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard");

            if (UseModdedBuff)
            {
                SetupBuff();
                RecalculateStatsAPI.GetStatCoefficients += HandleStatsCustom;
            }
            else
            {
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
                args.baseRegenAdd += 0.01f * sender.maxHealth;
            }
        }

        private void HandleStatsVanilla(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(warbannerBuff))
            {
                //+30% AtkSpd and MoveSpd already present in vanilla
                args.damageMultAdd += 0.15f;
                args.baseRegenAdd += 0.01f * sender.maxHealth;
            }
        }
    }
}
