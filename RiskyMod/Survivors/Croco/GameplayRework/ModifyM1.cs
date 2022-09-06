using RoR2;
using R2API;
using RoR2.Projectile;
using UnityEngine;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyM1
    {
        public ModifyM1()
        {
            On.EntityStates.Croco.Slash.AuthorityModifyOverlapAttack += (orig, self, overlapAttack) =>
            {
                orig(self, overlapAttack);
                if (self.isComboFinisher)
                {
                    if (RiskyMod.SpikestripPlasmaCore)
                    {
                        DeeprotCompat(overlapAttack, self.skillLocator);
                    }
                    else
                    {
                        overlapAttack.AddModdedDamageType(SharedDamageTypes.CrocoPoison6s);
                    }
                }
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void DeeprotCompat(OverlapAttack overlapAttack, SkillLocator skillLocator)
        {
            bool deeprotEquipped = false;
            foreach (GenericSkill gs in skillLocator.allSkills)
            {
                if (gs.skillDef == PlasmaCoreSpikestripContent.Content.Skills.DeepRot.scriptableObject.SkillDefinition)
                {
                    deeprotEquipped = true;
                    overlapAttack.damageType = DamageType.PoisonOnHit | DamageType.BlightOnHit; //Check to see if this changes later.
                    break;
                }
            }

            if (!deeprotEquipped)
            {
                overlapAttack.AddModdedDamageType(SharedDamageTypes.CrocoPoison6s);
            }
        }
    }
}
