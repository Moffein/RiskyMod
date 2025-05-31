using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Engi
{
    public class TurretChanges
    {
        public static bool normalizeStats = true;

        public TurretChanges()
        {
            if (!normalizeStats) return;
            ModifyTurret();
            ModifyWalkerTurret();
        }

        private void ModifyTurret()
        {
            GameObject turretBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiTurretBody.prefab").WaitForCompletion();
            CharacterBody cb = turretBody.GetComponent<CharacterBody>();
            cb.damage = 12f;
            cb.levelDamage = cb.damage * 0.2f;
            cb.regen = 1f;
            cb.levelRegen = cb.regen * 0.2f;
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Engi/EntityStates.EngiTurret.EngiTurretWeapon.FireGauss.asset", "damageCoefficient",  "0.933333333");
        }

        private void ModifyWalkerTurret()
        {
            GameObject turretBody = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiWalkerTurretBody.prefab").WaitForCompletion();
            CharacterBody cb = turretBody.GetComponent<CharacterBody>();
            cb.damage = 12f;
            cb.levelDamage = cb.damage * 0.2f;
            cb.regen = 1f;
            cb.levelRegen = cb.regen * 0.2f;
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Engi/EntityStates.EngiTurret.EngiTurretWeapon.FireBeam.asset", "damageCoefficient", "2.666666667");
        }
    }
}
