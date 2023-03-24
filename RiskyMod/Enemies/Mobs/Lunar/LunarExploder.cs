using System;
using RoR2;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;

namespace RiskyMod.Enemies.Mobs.Lunar
{
    public class LunarExploder
    {
        public static bool enabled = true;

        public static GameObject modifiedFirePool;

        public LunarExploder()
        {
            if (!enabled) return;

            modifiedFirePool = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarExploder/LunarExploderProjectileDotZone.prefab").WaitForCompletion().InstantiateClone("RiskyModLunarExploderProjectileDotZone", true);
            ProjectileDotZone pdz = modifiedFirePool.GetComponent<ProjectileDotZone>();
            pdz.overlapProcCoefficient = 0.2f;  //0.5 is vanilla
            Allies.DotZoneResist.AddDotZoneDamageType(modifiedFirePool);

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.LunarExploderMonster.DeathState", "projectilePrefab", modifiedFirePool);

            Content.Content.projectilePrefabs.Add(modifiedFirePool);
        }
    }
}
