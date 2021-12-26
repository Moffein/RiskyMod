using R2API;

namespace RiskyMod.Survivors.Loader
{
    public class UtilityInterrupt
    {
        public UtilityInterrupt()
        {
            On.EntityStates.Loader.BaseSwingChargedFist.AuthorityModifyOverlapAttack += (orig, self, overlapAttack) =>
            {
                orig(self, overlapAttack);
                overlapAttack.AddModdedDamageType(SharedDamageTypes.InterruptOnHit);
            };
        }
    }
}
