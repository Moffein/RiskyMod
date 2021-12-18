using RoR2;
using R2API;

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
                    overlapAttack.damageType = self.GetComponent<CrocoDamageTypeController>().GetDamageType();
                    /*if (overlapAttack.damageType == DamageType.BlightOnHit)
                    {
                        overlapAttack.damageType = DamageType.Generic;
                        overlapAttack.AddModdedDamageType(SharedDamageTypes.Blight7s);
                    }*/
                }
            };
        }
    }
}
