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
using UnityEngine.AddressableAssets;
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
            RecalculateStatsAPI.GetStatCoefficients += ModifyBeetleJuice;
        }

        private void ModifyProjectile()
        {

            GameObject acidProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BeetleQueen/BeetleQueenAcid.prefab").WaitForCompletion().InstantiateClone("RiskyMod_BeetleQueenAcid", true);
            acidProjectile.transform.localScale = 2f * Vector3.one; //Original scale is (1, 1, 1), Beetle Queen Plus is 2.5x
            ProjectileDotZone pdz = acidProjectile.GetComponent<ProjectileDotZone>();
            pdz.overlapProcCoefficient = 0.3f;
            pdz.resetFrequency = 5f;
            pdz.lifetime = 25f; //15f
            Content.Content.projectilePrefabs.Add(acidProjectile);

            GameObject spitProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BeetleQueen/BeetleQueenSpit.prefab").WaitForCompletion().InstantiateClone("RiskyMod_BeetleQueenSpit", true);

            ProjectileImpactExplosion pie = spitProjectile.GetComponent<ProjectileImpactExplosion>();
            //pie.blastDamageCoefficient = 1.3f;
            pie.blastRadius = 6f;
            pie.childrenDamageCoefficient = 0.1f;
            pie.childrenProjectilePrefab = acidProjectile;

            Content.Content.projectilePrefabs.Add(spitProjectile);

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/BeetleQueen/EntityStates.BeetleQueenMonster.FireSpit.asset", "projectilePrefab", spitProjectile);
        }

        private static void ModifyBeetleJuice(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            int buffCount = sender.GetBuffCount(RoR2Content.Buffs.BeetleJuice.buffIndex);
            if (buffCount > 0)
            {
                args.armorAdd += buffCount * -5f;
            }
        }
    }
}
