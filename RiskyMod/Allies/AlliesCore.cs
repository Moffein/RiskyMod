using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mono.Cecil.Cil;
using UnityEngine.AddressableAssets;
using RoR2.Skills;
using RiskyMod.Allies.DroneChanges;
using R2API;

namespace RiskyMod.Allies
{

    public class AlliesCore
    {
        public static bool enabled = true;
        public static bool nerfDroneParts = true;
        public static bool dronePartsIgnoresAllyCap = true;
        public static bool beetleGlandDontRetaliate = true;
        public static bool normalizeDroneDamage = true;

        public AlliesCore()
        {
            AllyItems.Init();

            if (!enabled) return;
            if (nerfDroneParts)
            {
                Items.ItemsCore.ModifyItemDefActions += ModifyDroneParts;
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.DroneWeaponsChainGun.FireChainGun", "damageCoefficient", "0.6");   //coef 1 orig

                IL.RoR2.DroneWeaponsBoostBehavior.OnEnemyHit += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(MoveType.After,
                         x => x.MatchCallvirt<CharacterBody>("get_damage")
                        ))
                    {
                        c.Emit(OpCodes.Ldarg_1);    //DamageInfo
                        c.EmitDelegate<Func<float, DamageInfo, float>>((bodyDamage, damageInfo) => damageInfo.damage);
                    }
                    else
                    {
                        Debug.LogError("RiskyMod: AlliesCore DroneWeaponsBoostBehavior.OnEnemyHit IL Hook failed");
                    }
                };
            }

            if (dronePartsIgnoresAllyCap)
            {
                IL.RoR2.DroneWeaponsBehavior.TrySpawnDrone += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(
                         x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                        ))
                    {
                        c.EmitDelegate<Func<DirectorSpawnRequest, DirectorSpawnRequest>>(spawnRequest =>
                        {
                            spawnRequest.ignoreTeamMemberLimit = dronePartsIgnoresAllyCap;
                            return spawnRequest;
                        });
                    }
                    else
                    {
                        Debug.LogError("RiskyMod: AlliesCore DroneWeaponsBehavior.TrySpawnDrone IL Hook failed");
                    }
                };
            }

            if (AlliesCore.normalizeDroneDamage)
            {
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireTurret", "damageCoefficient", "0.25");   //Shared with Gunner Drones, but those use a dedicated component to handle attacking now

                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireGatling", "damageCoefficient", "0.45");   //Damage 18 -> 12, coef 0.3 -> 0.45
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.HealBeam", "healCoefficient", "1.7");   //Damage 10 -> 12, coef 2 -> 1.6667
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireMegaTurret", "damageCoefficient", "2.2");   //Damage 14 -> 12, coef 2.2 -> 2.5667 mult by 0.8
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireTwinRocket", "damageCoefficient", "4.6667");   //Damage 14 -> 12, coef 4 -> 4.6667

                //SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Drone.DroneWeapon.FireMissileBarrage", "damageCoefficient", "1.7");   //Damage 14 -> 12, coef 1 -> 1.166666667

                NormalizeDroneDamage(AllyPrefabs.GunnerDrone);
                NormalizeDroneDamage(AllyPrefabs.HealDrone);
                NormalizeDroneDamage(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/backupdronebody"));
                NormalizeDroneDamage(AllyPrefabs.MissileDrone);
                NormalizeDroneDamage(AllyPrefabs.EquipmentDrone);
                NormalizeDroneDamage(AllyPrefabs.EmergencyDrone);
                NormalizeDroneDamage(AllyPrefabs.IncineratorDrone);
                NormalizeDroneDamage(AllyPrefabs.MegaDrone);
                NormalizeDroneDamage(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/dronecommanderbody"));
                NormalizeDroneDamage(AllyPrefabs.GunnerTurret);
                NormalizeDroneDamage(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/RoboBallGreenBuddyBody"));
                NormalizeDroneDamage(LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/RoboBallRedBuddyBody"));
            }

            new AllyScaling();

            new DroneTargeting();
            new ModifyBulletAttacks();

            new SuperAttackResist();
            new MushrumResist();
            new EliteDamageModifiers();
            new NoVoidDamage();

            new StricterLeashing();
            TweakDrones();
        }

        private static void ModifyDroneParts()
        {
            HG.ArrayUtils.ArrayAppend(ref Items.ItemsCore.changedItemDescs, DLC1Content.Items.DroneWeapons);
        }

        private void TweakDrones()
        {
            new GunnerTurret();
            new MegaDrone();
            new IncineratorDrone();
            new MissileDrone();
            new GunnerDrone();
            new HealDrone();
            new DroneCommander();
            new EmergencyDrone();
        }

        public static void NormalizeDroneDamage(GameObject bodyObject)
        {
            if (bodyObject)
            {
                CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
                if (cb)
                {
                    cb.baseDamage = 12f;
                    cb.levelDamage = cb.baseDamage * 0.2f;
                }
            }
        }
    }
}
