using RoR2.Orbs;
using RoR2;
using UnityEngine;
using System.Linq;
using EntityStates.Drone.DroneWeapon;
using UnityEngine.Networking;

namespace RiskyMod.Allies.DroneChanges
{
    public class MissileDrone
    {
        public MissileDrone()
        {
            On.EntityStates.Drone.DroneWeapon.FireMissileBarrage.FireMissile += (orig, self, targetMuzzle) =>
            {
                bool firedMissile = false;
                if (self.GetTeam() == TeamIndex.Player)
                {
                    self.missileCount++;
                    self.PlayAnimation("Gesture, Additive", "FireMissile");
                    Ray aimRay = self.GetAimRay();
                    if (self.modelTransform)
                    {
                        ChildLocator component = self.modelTransform.GetComponent<ChildLocator>();
                        if (component)
                        {
                            Transform transform = component.FindChild(targetMuzzle);
                            if (transform)
                            {
                                aimRay.origin = transform.position;
                            }
                        }
                    }

                    if (FireMissileBarrage.effectPrefab)
                    {
                        EffectManager.SimpleMuzzleFlash(FireMissileBarrage.effectPrefab, self.gameObject, targetMuzzle, false);
                    }
                    if (self.characterBody)
                    {
                        self.characterBody.SetAimTimer(2f);
                    }

                    if (NetworkServer.active)
                    {
                        BullseyeSearch search = new BullseyeSearch();
                        search.teamMaskFilter = TeamMask.GetEnemyTeams(TeamIndex.Player);
                        search.filterByLoS = false;
                        search.searchOrigin = aimRay.origin;
                        search.sortMode = BullseyeSearch.SortMode.Angle;
                        search.maxDistanceFilter = 80f; //Should this be 60f?
                        search.maxAngleFilter = 360f;
                        search.RefreshCandidates();

                        HurtBox targetHurtBox = search.GetResults().FirstOrDefault<HurtBox>();
                        if (targetHurtBox != default)
                        {
                            firedMissile = true;
                            MicroMissileOrb missileOrb = new MicroMissileOrb();
                            missileOrb.origin = aimRay.origin;
                            missileOrb.damageValue = self.damageStat * FireMissileBarrage.damageCoefficient;
                            missileOrb.isCrit = self.RollCrit();
                            missileOrb.teamIndex = TeamIndex.Player;
                            missileOrb.attacker = self.gameObject;
                            missileOrb.procChainMask = default;
                            missileOrb.procChainMask.AddProc(ProcType.Missile);
                            missileOrb.procCoefficient = RiskyMod.disableProcChains ? 0f : 1f;
                            missileOrb.damageColorIndex = DamageColorIndex.Item;
                            missileOrb.target = targetHurtBox;
                            missileOrb.speed = 25f; //Same as misisleprojectile. Default is 55f
                            OrbManager.instance.AddOrb(missileOrb);
                        }
                    }
                }

                if (!firedMissile) orig(self, targetMuzzle);
            };
        }
    }
}
