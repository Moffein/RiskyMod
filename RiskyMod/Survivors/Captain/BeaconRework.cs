using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Captain
{
    public class BeaconRework
    {
        public static class Skills
        {
            public static SkillDef BeaconResupply;
            public static SkillDef BeaconHacking;
        }

        public static BuffDef AmpBuff;

        public BeaconRework(SkillLocator sk)
        {
            Skills.BeaconResupply = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallSupplyDropEquipmentRestock.asset").WaitForCompletion();
            Skills.BeaconHacking = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Captain/CallSupplyDropHacking.asset").WaitForCompletion();
            AddCooldown(Skills.BeaconResupply);
            AddCooldown(Skills.BeaconHacking);
            AddCooldown("RoR2/Base/Captain/CallSupplyDropHealing.asset");
            AddCooldown("RoR2/Base/Captain/CallSupplyDropShocking.asset");

            sk.special.skillFamily.variants[0].skillDef.skillDescriptionToken = "CAPTAIN_SPECIAL_DESCRIPTION_RISKYMOD";

            //Register beacons when they spawn
            CaptainCore.bodyPrefab.AddComponent<CaptainDeployableManager>();
            IL.EntityStates.Captain.Weapon.CallSupplyDropBase.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchCall<NetworkServer>("Spawn")
                   );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<GameObject, EntityStates.Captain.Weapon.CallSupplyDropBase, GameObject>>((beacon, self) =>
                {
                    CaptainDeployableManager cdm = self.gameObject.GetComponent<CaptainDeployableManager>();
                    if (cdm)
                    {
                        cdm.AddBeacon(beacon, self.activatorSkillSlot);
                    }
                    return beacon;
                });
            };

            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);
                if (self.bodyIndex == CaptainCore.CaptainIndex)
                {
                    if (self.skillLocator.special.bonusStockFromBody > 0)
                    {
                        self.skillLocator.FindSkill("SupplyDrop1").SetBonusStockFromBody(self.skillLocator.special.bonusStockFromBody);
                        if (self.skillLocator.special.bonusStockFromBody > 1)
                            self.skillLocator.FindSkill("SupplyDrop2").SetBonusStockFromBody(self.skillLocator.special.bonusStockFromBody);
                    }
                }
            };
            ModifyBeacons(sk);
        }

        private void AddCooldown(string address)
        {
            SkillDef sd = Addressables.LoadAssetAsync<SkillDef>(address).WaitForCompletion();
            AddCooldown(sd);
        }

        private void AddCooldown(SkillDef sd)
        {
            sd.rechargeStock = 1;
            sd.baseRechargeInterval = 60f;
            sd.baseMaxStock = 1;
            sd.beginSkillCooldownOnSkillEnd = false;

            On.RoR2.CaptainSupplyDropController.SetSkillOverride +=
                (On.RoR2.CaptainSupplyDropController.orig_SetSkillOverride orig, CaptainSupplyDropController self, ref SkillDef currentSkillDef, SkillDef newSkillDef, GenericSkill component) =>
                {
                    newSkillDef = currentSkillDef;
                    orig(self, ref currentSkillDef, newSkillDef, component);
                };
        }

        private void ModifyBeacons(SkillLocator sk)
        {
            //Debug.Log("Shock Radius: " + SneedUtils.SneedUtils.GetEntityStateFieldString("EntityStates.CaptainSupplyDrop.ShockZoneMainState", "shockRadius"));//10, same as healing
            ModifyBeaconResupply(sk);
            ModifyBeaconHacking(sk);
        }

        private void ModifyBeaconResupply(SkillLocator sk)
        {
            GameObject beaconPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainSupplyDrop, EquipmentRestock.prefab").WaitForCompletion();
            EntityStateMachine esm = beaconPrefab.GetComponent<EntityStateMachine>();
            esm.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyMod.Captain.Beacon.BeaconResupplyMain));
            Skills.BeaconResupply.skillDescriptionToken = "CAPTAIN_SUPPLY_EQUIPMENT_RESTOCK_DESCRIPTION_RISKYMOD";

            Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.Captain.Beacon.BeaconResupplyMain));
        }
        private void ModifyBeaconHacking(SkillLocator sk)
        {
            AmpBuff = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyModCaptainAmp",
                false,
                false,
                false,
                new Color(0.839f, 0.788f, 0.227f),
                Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ShockNearby/bdTeslaField.asset").WaitForCompletion().iconSprite
                );

            RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender.HasBuff(AmpBuff))
                {
                    args.attackSpeedMultAdd += 0.3f;
                    args.armorAdd += 30f;

                    if (!sender.isPlayerControlled && sender.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical))
                    {
                        args.regenMultAdd += 1f;
                        args.damageMultAdd += 0.15f;
                    }    
                }
            };

            SharedHooks.OnHitEnemy.OnHitAttackerActions += (DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody) =>
            {
                if (!attackerBody.isPlayerControlled && attackerBody.bodyFlags.HasFlag(CharacterBody.BodyFlags.Mechanical) && attackerBody.HasBuff(AmpBuff) && !damageInfo.procChainMask.HasProc(ProcType.LoaderLightning))
                {
                    if (Util.CheckRoll(30f * damageInfo.procCoefficient, attackerBody.master))
                    {
                        float damageCoefficient3 = 1f;
                        float damageValue2 = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient3);

                        LightningOrb lightningOrb = new LightningOrb();
                        lightningOrb.origin = damageInfo.position;
                        lightningOrb.damageValue = damageValue2;
                        lightningOrb.isCrit = damageInfo.crit;
                        lightningOrb.bouncesRemaining = 3;
                        lightningOrb.teamIndex = attackerBody.teamComponent ? attackerBody.teamComponent.teamIndex : TeamIndex.None;
                        lightningOrb.attacker = damageInfo.attacker;

                        lightningOrb.bouncedObjects = new List<HealthComponent>();
                        if (victimBody.healthComponent && !victimBody.healthComponent.alive)
                        {
                            lightningOrb.bouncedObjects.Add(victimBody.healthComponent);
                        }

                        //lightningOrb.bouncedObjects = new List<HealthComponent> { victimBody.healthComponent };
                        lightningOrb.procChainMask = damageInfo.procChainMask;
                        lightningOrb.procChainMask.AddProc(ProcType.LoaderLightning);
                        lightningOrb.procCoefficient = 0.1f;
                        lightningOrb.lightningType = LightningOrb.LightningType.Loader;
                        lightningOrb.damageColorIndex = DamageColorIndex.Item;
                        lightningOrb.range = 25f;
                        HurtBox hurtBox = lightningOrb.PickNextTarget(damageInfo.position);
                        if (hurtBox)
                        {
                            lightningOrb.target = hurtBox;
                            OrbManager.instance.AddOrb(lightningOrb);
                        }
                    }
                }
            };

            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += (orig, self, buffDef, duration) =>
            {
                orig(self, buffDef, duration);

                if (self.isPlayerControlled && buffDef == AmpBuff)
                {
                    float buffDuration = Mathf.Max(duration, 7f);
                    //based on https://github.com/DestroyedClone/RoR1SkillsPort/blob/master/Loader/ActivateShield.cs
                    foreach (var characterMaster in CharacterMaster.readOnlyInstancesList)
                    {
                        if (characterMaster.minionOwnership && characterMaster.minionOwnership.ownerMaster == self.master)
                        {
                            CharacterBody minionBody = characterMaster.GetBody();
                            if (minionBody && !minionBody.isPlayerControlled && (minionBody.bodyFlags &= CharacterBody.BodyFlags.Mechanical) == CharacterBody.BodyFlags.Mechanical)
                            {
                                minionBody.AddTimedBuff(buffDef, buffDuration);
                            }
                        }
                    }
                }
            };

            IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Warbanner")
                    );
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasWarbanner, self) =>
                {
                    return hasWarbanner || self.HasBuff(AmpBuff);
                });
            };

            GameObject beaconPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainSupplyDrop, Hacking.prefab").WaitForCompletion();
            EntityStateMachine esm = beaconPrefab.GetComponent<EntityStateMachine>();
            esm.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyMod.Captain.Beacon.BeaconHackingMain));

            BuffWard ward = beaconPrefab.AddComponent<BuffWard>();
            ward.shape = BuffWard.BuffWardShape.Sphere;
            ward.radius = 10f;
            ward.interval = 0.5f;
            ward.buffDef = AmpBuff;
            ward.buffDuration = 1.5f;
            ward.floorWard = false;
            ward.expires = false;
            ward.invertTeamFilter = false;
            ward.animateRadius = false;
            ward.requireGrounded = false;

            Skills.BeaconHacking.skillDescriptionToken = "CAPTAIN_SUPPLY_HACKING_DESCRIPTION_RISKYMOD";

            Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.Captain.Beacon.BeaconHackingMain));
        }
    }

    public class CaptainDeployableManager : MonoBehaviour
    {
        public SkillLocator skillLocator;
        public CharacterBody body;

        public GenericSkill Beacon1;
        public GenericSkill Beacon2;

        public Queue<GameObject> Beacon1Deployables;
        public Queue<GameObject> Beacon2Deployables;

        public static bool allowLysateStack = false;

        private void Awake()
        {
            body = base.GetComponent<CharacterBody>();
            skillLocator = base.GetComponent<SkillLocator>();

            Beacon1 = skillLocator.FindSkill("SupplyDrop1");
            Beacon2 = skillLocator.FindSkill("SupplyDrop2");

            Beacon1Deployables = new Queue<GameObject>();
            Beacon2Deployables = new Queue<GameObject>();
        }

        public void AddBeacon(GameObject newBeacon, GenericSkill skill)
        {
            if (!NetworkServer.active) return;  //Beacons being instantiated/deleted are server-side.
            int maxBeacons = skillLocator.special.maxStock;
            if (!allowLysateStack && maxBeacons >= 2) maxBeacons = 2;
            if (skill == Beacon1)
            {
                if(Beacon1Deployables.Count >= maxBeacons)
                {
                    GameObject toRemove = Beacon1Deployables.Dequeue();
                    UnityEngine.Object.Destroy(toRemove);
                }

                Beacon1Deployables.Enqueue(newBeacon);
            }
            else if (skill == Beacon2)
            {
                if (Beacon2Deployables.Count >= maxBeacons)
                {
                    GameObject toRemove = Beacon2Deployables.Dequeue();
                    UnityEngine.Object.Destroy(toRemove);
                }
                Beacon2Deployables.Enqueue(newBeacon);
            }
        }

        private void OnDestroy()
        {
            while (Beacon1Deployables.Count > 0)
            {
                UnityEngine.Object.Destroy(Beacon1Deployables.Dequeue());
            }
            while (Beacon2Deployables.Count > 0)
            {
                UnityEngine.Object.Destroy(Beacon2Deployables.Dequeue());
            }
        }
    }
}
