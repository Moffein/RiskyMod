using RoR2;
using UnityEngine;
using RoR2.Projectile;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using R2API;
using System.Runtime.CompilerServices;
using RiskyMod.MonoBehaviours;
using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyM2Spit
    {
        public ModifyM2Spit()
        {
            GameObject spitModded = EntityStates.RiskyMod.Croco.FireSpitModded.projectilePrefabVanilla.InstantiateClone("RiskyMod_CrocoSpit", true);
            Content.Content.projectilePrefabs.Add(spitModded);

            DamageAPI.ModdedDamageTypeHolderComponent mdc = spitModded.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(SharedDamageTypes.CrocoBlight6s);

            ProjectileDamage pd = spitModded.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            EntityStates.RiskyMod.Croco.FireSpitModded.projectilePrefab = spitModded;
        }
    }
}
