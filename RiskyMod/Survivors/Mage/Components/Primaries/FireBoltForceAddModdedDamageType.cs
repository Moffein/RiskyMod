using R2API;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.Survivors.Mage.Components.Primaries
{
    public class FireBoltForceAddModdedDamageType : MonoBehaviour
    {
        private void Start()
        {
            ProjectileDamage pd = base.GetComponent<ProjectileDamage>();
            if (pd)
            {
                pd.damageType.AddModdedDamageType(SkillTweaks.PrimaryRework.ApplyFireboltRepeat);
                pd.damageType.AddModdedDamageType(SkillTweaks.PrimaryRework.QuadrupleHitstunPower);
            }

            ProjectileImpactExplosion pie = base.GetComponent<ProjectileImpactExplosion>();
            if (pie)
            {
                pie.blastProcCoefficient = 1f;
            }
            Destroy(this);
        }
    }
}
