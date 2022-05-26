using RiskyMod.Survivors.Croco;
using RoR2;
using RoR2.Stats;

namespace RiskyMod.Fixes
{
    public class FixCrocoPoisonAchievement
    {
        public FixCrocoPoisonAchievement()
        {
            On.RoR2.DotController.AddDot += (orig, self, attackerObject, duration, dotIndex, damageMultiplier, maxStacksFromAttacker, totalDamage, preUpgradeDotIndex) =>
            {
                orig(self, attackerObject, duration, dotIndex, damageMultiplier, maxStacksFromAttacker, totalDamage, preUpgradeDotIndex);
                if (dotIndex == RoR2.DotController.DotIndex.Blight)
                {
                    CharacterBody attackerBody = attackerObject.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        CharacterMaster master = attackerBody.master;
                        if (master != null)
                        {
                            PlayerStatsComponent playerStatsComponent = master.playerStatsComponent;
                            if (playerStatsComponent != null)
                            {
                                playerStatsComponent.currentStats.PushStatValue(StatDef.totalCrocoInfectionsInflicted, 1UL);
                            }
                        }
                    }
                }
            };
        }
    }
}
