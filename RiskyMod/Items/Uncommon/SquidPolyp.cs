using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.MonoBehaviours;
using RiskyMod.SharedHooks;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.Uncommon
{
    public class SquidPolyp
    {
        public static bool enabled = true;
        public static GameObject procEffectPrefab;

        //Does this need turretblacklist?
        public SquidPolyp()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.Squid);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.Squid);

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnInteractionBegin += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Squid")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            //LanguageAPI.Add("ITEM_SQUIDTURRET_PICKUP", "Taking heavy damage summons a Squid Turret nearby.");
            //LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", "Chance on taking damage to summon a <style=cIsDamage>Squid Turret</style> that <style=cIsUtility>distracts</style> and attacks nearby enemies at <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> attack speed</style>. Chance increases the more damage you take. Can have up to <style=cIsUtility>2</style> <style=cStack>(+1 per stack)</style> Squids at a time.");

            procEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/claygooorbimpact").InstantiateClone("RiskyItemTweaks_SquidPolypProc", false);
            EffectComponent ec = procEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_treeBot_m2_launch";
            EffectAPI.AddEffect(procEffectPrefab);

            TakeDamage.HandleOnHpLostActions += OnHpLost;
        }

        private void OnHpLost(DamageInfo damageInfo, HealthComponent self, Inventory inventory, float percentHpLost)
        {
            int polypCount = inventory.GetItemCount(RoR2Content.Items.Squid);
            if (polypCount > 0)
            {
                if (percentHpLost > 0f)
                {
                    if (Util.CheckRoll(percentHpLost, self.body.master))
                    {
                        SquidMinionComponent sq = self.gameObject.GetComponent<SquidMinionComponent>();
                        if (!sq)
                        {
                            sq = self.gameObject.AddComponent<SquidMinionComponent>();
                        }
                        if (sq.CanSpawnSquid())
                        {
                            EffectManager.SimpleEffect(SquidPolyp.procEffectPrefab, self.body.corePosition, Quaternion.identity, true);
                            SpawnCard spawnCard = Resources.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscSquidTurret");
                            DirectorPlacementRule placementRule = new DirectorPlacementRule
                            {
                                placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                                minDistance = 5f,
                                maxDistance = 25f,
                                position = self.body.corePosition
                            };
                            DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(spawnCard, placementRule, RoR2Application.rng);
                            directorSpawnRequest.teamIndexOverride = self.body.teamComponent.teamIndex;
                            directorSpawnRequest.summonerBodyObject = self.gameObject;
                            directorSpawnRequest.ignoreTeamMemberLimit = true;  //Polyps should always be able to spawn. Does this need a cap for performance?
                            directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
                            {
                                if (!result.success)
                                {
                                    return;
                                }
                                CharacterMaster component6 = result.spawnedInstance.GetComponent<CharacterMaster>();
                                component6.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                                if (self.itemCounts.invadingDoppelganger > 0)
                                {
                                    //Doppelganger Turrets decay faster.
                                    component6.inventory.GiveItem(RoR2Content.Items.InvadingDoppelganger);
                                    component6.inventory.GiveItem(RoR2Content.Items.HealthDecay, 12);
                                }
                                else
                                {
                                    component6.inventory.GiveItem(RoR2Content.Items.BoostAttackSpeed, 10 * (polypCount - 1));
                                    component6.inventory.GiveItem(RoR2Content.Items.HealthDecay, 25);
                                }
                                sq.AddSquid(result.spawnedInstance);
                            }));
                            DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                        }
                    }
                }
            }
        }
    }
}
