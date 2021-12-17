using RoR2;
using R2API;

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
                overlapAttack.AddModdedDamageType(SharedDamageTypes.Blight7s);
                overlapAttack.AddModdedDamageType(SharedDamageTypes.DebuffRegen);
            };
        }
    }
}
