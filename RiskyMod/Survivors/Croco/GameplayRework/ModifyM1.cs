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
                    if (CrocoCore.HasDeeprot(self.skillLocator))
                    {
                        overlapAttack.damageType = DamageType.PoisonOnHit | DamageType.BlightOnHit; //Check to see if this changes later.
                    }
                    else
                    {
                        overlapAttack.AddModdedDamageType(SharedDamageTypes.CrocoPoison6s);
                    }
                }
            };
        }
    }
}
