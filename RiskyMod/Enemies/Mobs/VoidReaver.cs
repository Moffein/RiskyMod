using RoR2;
using UnityEngine.Networking;
using R2API;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;

namespace RiskyMod.Enemies.Mobs
{
    public class VoidReaver
    {
        public static bool enabled = true;
        public static DamageAPI.ModdedDamageType ExtraNullify;
        public static GameObject NullifierPreBombProjectileModded;
        //public static GameObject NullifierBombProjectileModded;

        public VoidReaver()
        {
            if (!enabled) return;

            ExtraNullify = DamageAPI.ReserveDamageType();
            On.RoR2.HealthComponent.TakeDamageProcess += (orig, self, damageInfo) =>
            {
                orig(self, damageInfo);
                if (NetworkServer.active && !damageInfo.rejected && damageInfo.HasModdedDamageType(VoidReaver.ExtraNullify))
                {
                    self.body.AddTimedBuff(RoR2Content.Buffs.NullifyStack, 8f);
                }
            };

            NullifierPreBombProjectileModded = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Nullifier/NullifierPreBombProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyModNullifierPreBombProjectile", true);

            ProjectileDamage pd = NullifierPreBombProjectileModded.GetComponent<ProjectileDamage>();
            pd.damageType.AddModdedDamageType(ExtraNullify);
            Content.Content.projectilePrefabs.Add(NullifierPreBombProjectileModded);

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Nullifier/EntityStates.NullifierMonster.FirePortalBomb.asset", "portalBombProjectileEffect", NullifierPreBombProjectileModded);
        }
    }
}
