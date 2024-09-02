using RoR2;
using RoR2.CharacterAI;
using UnityEngine;

namespace RiskyMod.Survivors.Engi
{
    public class TurretChanges
    {
        public static bool turretChanges = true;
        public static bool mobileTurretChanges = true;

        public TurretChanges()
        {
            ModifyTurret();
            ModifyWalkerTurret();
        }

        private void ModifyTurret()
        {
            if (!turretChanges) return;
            GameObject turretBody = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/EngiTurretBody");
            CharacterBody cb = turretBody.GetComponent<CharacterBody>();
            cb.damage = 12f;
            cb.levelDamage = cb.damage * 0.2f;
            cb.regen = 1f;
            cb.levelRegen = cb.regen * 0.2f;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.EngiTurret.EngiTurretWeapon.FireGauss", "damageCoefficient",  "0.933333333");
        }

        private void ModifyWalkerTurret()
        {
            if (!mobileTurretChanges) return;
            GameObject turretBody = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/EngiWalkerTurretBody");
            CharacterBody cb = turretBody.GetComponent<CharacterBody>();
            cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            cb.damage = 12f;
            cb.levelDamage = cb.damage * 0.2f;
            cb.regen = 1f;
            cb.levelRegen = cb.regen * 0.2f;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.EngiTurret.EngiTurretWeapon.FireBeam", "damageCoefficient", "2.666666667");
        }
    }
}
