using RoR2;
using R2API;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyM2Bite
    {
        public ModifyM2Bite()
        {
            On.EntityStates.Croco.Bite.AuthorityModifyOverlapAttack += (orig, self, overlapAttack) =>
            {
                orig(self, overlapAttack);
                overlapAttack.damageType = DamageType.BonusToLowHealth;
                overlapAttack.AddModdedDamageType(SharedDamageTypes.CrocoBiteHealOnKill);

                if (CrocoCore.HasDeeprot(self.skillLocator))
                {
                    overlapAttack.damageType |= DamageType.PoisonOnHit | DamageType.BlightOnHit;
                }
                else
                {
                    overlapAttack.AddModdedDamageType(SharedDamageTypes.CrocoBlight6s);
                }
            };
        }
    }
}
