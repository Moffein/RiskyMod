using EntityStates.BeetleQueenMonster;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Enemies.Bosses
{
    public class BeetleQueen
    {
        public static bool enabled = true;
        public static BuffDef BeetleBuff;
        public static BodyIndex BeetleBodyIndex;
        public static BodyIndex BeetleGuardBodyIndex;
        public static BodyIndex BeetleGuardAllyBodyIndex;

        public static SpawnCard BeetleCard;
        public static SpawnCard BeetleGuardCard;

        public BeetleQueen()
        {
            if (!enabled) return;

            ModifyProjectile();
            //BuildBeetleBuff();
            //ModifySpawns();
            GetStatsCoefficient.HandleStatsActions += ModifyBeetleJuice;
            //RebuildAI();
        }

        private void ModifySpawns()
        {
            BeetleCard = Resources.Load<SpawnCard>("spawncards/characterspawncards/cscbeetle");
            BeetleGuardCard = Resources.Load<SpawnCard>("spawncards/characterspawncards/cscbeetleguard");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.BeetleQueenMonster.SummonEggs", "spawnCard", BeetleCard);
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.BeetleQueenMonster.SummonEggs", "maxSummonCount", "6");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.BeetleQueenMonster.SummonEggs", "summonInterval", "0.25");

            On.EntityStates.BeetleQueenMonster.SummonEggs.SummonEgg += SummonEgg2;
        }

        //Method that calls this is in a NetworkServer.Authority check
        private void SummonEgg2(On.EntityStates.BeetleQueenMonster.SummonEggs.orig_SummonEgg orig, EntityStates.BeetleQueenMonster.SummonEggs self)
        {
            Vector3 searchOrigin = self.GetAimRay().origin;
            RaycastHit raycastHit;
            if (self.inputBank && self.inputBank.GetAimRaycast(float.PositiveInfinity, out raycastHit))
            {
                searchOrigin = raycastHit.point;
            }
            if (self.enemySearch != null)
            {
                self.enemySearch.searchOrigin = searchOrigin;
                self.enemySearch.RefreshCandidates();
                HurtBox hurtBox = self.enemySearch.GetResults().FirstOrDefault<HurtBox>();
                Transform transform = (hurtBox && hurtBox.healthComponent) ? hurtBox.healthComponent.body.coreTransform : self.characterBody.coreTransform;
                if (transform)
                {
                    SpawnCard selectedCard = self.summonCount < SummonEggs.maxSummonCount ? SummonEggs.spawnCard : BeetleGuardCard;
                    DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(selectedCard, new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                        minDistance = 3f,
                        maxDistance = 20f,
                        spawnOnTarget = transform
                    }, RoR2Application.rng);
                    directorSpawnRequest.summonerBodyObject = self.gameObject;
                    DirectorSpawnRequest directorSpawnRequest2 = directorSpawnRequest;
                    directorSpawnRequest2.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest2.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult spawnResult)
                    {
                        Inventory minionInventory = spawnResult.spawnedInstance.GetComponent<Inventory>();
                        minionInventory.CopyEquipmentFrom(self.characterBody.inventory);

                        if (self.characterBody.inventory.GetItemCount(RoR2Content.Items.Ghost) > 0)
                        {
                            minionInventory.GiveItem(RoR2Content.Items.Ghost);

                            minionInventory.GiveItem(RoR2Content.Items.BoostDamage, self.characterBody.inventory.GetItemCount(RoR2Content.Items.BoostDamage));
                        }
                        int decayCount = self.characterBody.inventory.GetItemCount(RoR2Content.Items.HealthDecay);
                        if (decayCount > 0)
                        {
                            minionInventory.GiveItem(RoR2Content.Items.HealthDecay, Math.Max(30, decayCount));
                        }
                    }));
                    DirectorCore instance = DirectorCore.instance;
                    if (instance == null)
                    {
                        return;
                    }
                    instance.TrySpawnObject(directorSpawnRequest);
                }
            }

            //Buff nearby Beetles
            if (self.characterBody)
            {
                List<CharacterBody> cbList = new List<CharacterBody>();
                Collider[] nearbyEnemies = Physics.OverlapSphere(self.characterBody.gameObject.transform.position, 120f * (self.characterBody.gameObject.transform.localScale.x / 1.0f), LayerIndex.entityPrecise.mask);
                for (int i = 0; i < nearbyEnemies.Length; i++)
                {
                    HurtBox hb = nearbyEnemies[i].GetComponent<HurtBox>();
                    if (hb && hb.healthComponent && hb.healthComponent.body)
                    {
                        if (hb.healthComponent.body && hb.healthComponent.body.teamComponent && hb.healthComponent.body.teamComponent.teamIndex == self.GetTeam())
                        {
                            if (hb.healthComponent.body.bodyIndex == BeetleBodyIndex
                                || hb.healthComponent.body.bodyIndex == BeetleGuardBodyIndex
                                || hb.healthComponent.body.bodyIndex == BeetleGuardAllyBodyIndex)
                            {
                                if (!cbList.Contains(hb.healthComponent.body))
                                {
                                    cbList.Add(hb.healthComponent.body);
                                }
                            }
                        }
                    }
                }
                for (int i = 0; i < cbList.Count; i++)
                {
                    cbList[i].AddTimedBuff(BeetleBuff, 15f);
                }
            }
        }

        private void ModifyProjectile()
        {

            GameObject acidProjectile = Resources.Load<GameObject>("prefabs/projectiles/beetlequeenacid").InstantiateClone("RiskyMod_BeetleQueenAcid", true);
            acidProjectile.transform.localScale = 2f * Vector3.one; //Original scale is (1, 1, 1), Beetle Queen Plus is 2.5x
            ProjectileDotZone pdz = acidProjectile.GetComponent<ProjectileDotZone>();
            pdz.overlapProcCoefficient = 0.3f;
            pdz.resetFrequency = 5f;
            pdz.lifetime = 20f; //15f
            ProjectileAPI.Add(acidProjectile);

            GameObject spitProjectile = Resources.Load<GameObject>("prefabs/projectiles/beetlequeenspit").InstantiateClone("RiskyMod_BeetleQueenSpit",true);
            ProjectileImpactExplosion pie = spitProjectile.GetComponent<ProjectileImpactExplosion>();
            //pie.blastDamageCoefficient = 1.3f;
            pie.blastRadius = 6f;
            pie.childrenDamageCoefficient = 0.1f;
            pie.childrenProjectilePrefab = acidProjectile;
            ProjectileAPI.Add(spitProjectile);
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.BeetleQueenMonster.FireSpit", "projectilePrefab", spitProjectile);
        }

        private void BuildBeetleBuff()
        {
            //Save BodyIndexes
            On.RoR2.BodyCatalog.Init += (orig) =>
            {
                orig();
                BeetleBodyIndex = BodyCatalog.FindBodyIndex("BeetleBody");
                BeetleGuardBodyIndex = BodyCatalog.FindBodyIndex("BeetleGuardBody");
                BeetleGuardAllyBodyIndex = BodyCatalog.FindBodyIndex("BeetleGuardAllyBody");
            };

            BeetleBuff = ScriptableObject.CreateInstance<BuffDef>();
            BeetleBuff.buffColor = RoR2Content.Buffs.Warbanner.buffColor;
            BeetleBuff.canStack = false;
            BeetleBuff.isDebuff = false;
            BeetleBuff.name = "RiskyMod_BeetleBuff";
            BeetleBuff.iconSprite = RoR2Content.Buffs.Warbanner.iconSprite;
            BuffAPI.Add(new CustomBuff(BeetleBuff));

            GetStatsCoefficient.HandleStatsActions += HandleBeetleBuff;

            IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Warbanner")
                    );
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasBuff, self) =>
                {
                    return hasBuff || self.HasBuff(BeetleBuff);
                });
            };
        }

        private static void HandleBeetleBuff(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(BeetleBuff))
            {
                args.moveSpeedMultAdd += 0.3f;
                args.attackSpeedMultAdd += 0.3f;
            }
        }

        private static void ModifyBeetleJuice(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffCount = sender.GetBuffCount(RoR2Content.Buffs.BeetleJuice.buffIndex);
            if (buffCount > 0)
            {
                args.armorAdd += buffCount * -5f;
            }
        }

        private void RebuildAI()
        {
            GameObject queenMaster = Resources.Load<GameObject>("prefabs/charactermasters/BeetleQueenMaster");

            Component[] toDelete = queenMaster.GetComponents<AISkillDriver>();
            foreach (AISkillDriver asd in toDelete)
            {
                UnityEngine.Object.Destroy(asd);
            }

            AISkillDriver spit = queenMaster.AddComponent<AISkillDriver>();
            spit.skillSlot = SkillSlot.Primary;
            spit.requireSkillReady = true;
            spit.requireEquipmentReady = false;
            spit.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            spit.minDistance = 0f;
            spit.maxDistance = 130f;
            spit.selectionRequiresTargetLoS = true;
            spit.activationRequiresTargetLoS = true;
            spit.activationRequiresAimConfirmation = true;
            spit.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            spit.aimType = AISkillDriver.AimType.AtMoveTarget;
            spit.ignoreNodeGraph = false;
            spit.driverUpdateTimerOverride = -1f;
            spit.noRepeat = false;
            spit.shouldSprint = false;
            spit.shouldFireEquipment = false;
            spit.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver swarm = queenMaster.AddComponent<AISkillDriver>();
            swarm.skillSlot = SkillSlot.Special;
            swarm.requireSkillReady = true;
            swarm.requireEquipmentReady = false;
            swarm.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            swarm.minDistance = 0f;
            swarm.maxDistance = 130f;
            swarm.selectionRequiresTargetLoS = false;
            swarm.activationRequiresTargetLoS = false;
            swarm.activationRequiresAimConfirmation = false;
            swarm.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            swarm.aimType = AISkillDriver.AimType.AtMoveTarget;
            swarm.ignoreNodeGraph = false;
            swarm.driverUpdateTimerOverride = -1f;
            swarm.noRepeat = true;
            swarm.shouldSprint = false;
            swarm.shouldFireEquipment = false;
            swarm.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver juice = queenMaster.AddComponent<AISkillDriver>();
            juice.skillSlot = SkillSlot.Secondary;
            juice.requireSkillReady = true;
            juice.requireEquipmentReady = false;
            juice.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            juice.minDistance = 0f;
            juice.maxDistance = 80f;
            juice.selectionRequiresTargetLoS = false;
            juice.activationRequiresTargetLoS = false;
            juice.activationRequiresAimConfirmation = false;
            juice.movementType = AISkillDriver.MovementType.FleeMoveTarget;
            juice.aimType = AISkillDriver.AimType.AtMoveTarget;
            juice.ignoreNodeGraph = false;
            juice.driverUpdateTimerOverride = -1f;
            juice.noRepeat = true;
            juice.shouldSprint = false;
            juice.shouldFireEquipment = false;
            juice.buttonPressType = AISkillDriver.ButtonPressType.Hold;

            AISkillDriver chase = queenMaster.AddComponent<AISkillDriver>();
            chase.skillSlot = SkillSlot.None;
            chase.requireSkillReady = false;
            chase.requireEquipmentReady = false;
            chase.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            chase.minDistance = 20f;
            chase.maxDistance = float.PositiveInfinity;
            chase.selectionRequiresTargetLoS = false;
            chase.activationRequiresTargetLoS = false;
            chase.activationRequiresAimConfirmation = false;
            chase.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chase.aimType = AISkillDriver.AimType.None;
            chase.ignoreNodeGraph = false;
            chase.driverUpdateTimerOverride = -1f;
            chase.noRepeat = false;
            chase.shouldSprint = false;
            chase.shouldFireEquipment = false;
            chase.buttonPressType = AISkillDriver.ButtonPressType.Hold;
        }
    }
}
