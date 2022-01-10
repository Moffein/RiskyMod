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
            GameObject turretBody = Resources.Load<GameObject>("prefabs/characterbodies/EngiTurretBody");
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
            float range = 45f;

            GameObject turretBody = Resources.Load<GameObject>("prefabs/characterbodies/EngiWalkerTurretBody");
            CharacterBody cb = turretBody.GetComponent<CharacterBody>();
            cb.damage = 12f;
            cb.levelDamage = cb.damage * 0.2f;
            cb.regen = 1f;
            cb.levelRegen = cb.regen * 0.2f;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.EngiTurret.EngiTurretWeapon.FireBeam", "damageCoefficient", "2.666666667");
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.EngiTurret.EngiTurretWeapon.FireBeam", "maxDistance", "45");

            Component[] aiDrivers = Resources.Load<GameObject>("prefabs/charactermasters/EngiWalkerTurretMaster").GetComponents<AISkillDriver>();
            foreach (AISkillDriver asd in aiDrivers)
            {
                if (asd.skillSlot != SkillSlot.Primary && asd.customName != "Rest")
                {
                    asd.shouldSprint = true;
                }
                if (asd.customName == "ChaseAndFireAtEnemy")
                {
                    asd.maxDistance = range;
                }
            }
        }
    }
}
