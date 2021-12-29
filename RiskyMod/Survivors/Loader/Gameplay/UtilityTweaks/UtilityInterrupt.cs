using R2API;
using UnityEngine;

namespace RiskyMod.Survivors.Loader
{
    public class UtilityInterrupt
    {
        public static GameObject ZapConeInterrupt;
        public UtilityInterrupt()
        {
            On.EntityStates.Loader.BaseSwingChargedFist.AuthorityModifyOverlapAttack += (orig, self, overlapAttack) =>
            {
                orig(self, overlapAttack);
                overlapAttack.AddModdedDamageType(SharedDamageTypes.InterruptOnHit);
            };

            ZapConeInterrupt = Resources.Load<GameObject>("prefabs/projectiles/loaderzapcone").InstantiateClone("RiskyMod_LoaderZapCone", true);
            DamageAPI.ModdedDamageTypeHolderComponent mdc = ZapConeInterrupt.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(SharedDamageTypes.InterruptOnHit);
            ProjectileAPI.Add(ZapConeInterrupt);
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Loader.SwingZapFist", "projectilePrefab", ZapConeInterrupt);
        }
    }
}
