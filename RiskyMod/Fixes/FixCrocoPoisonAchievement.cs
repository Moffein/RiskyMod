using RiskyMod.Survivors.Croco;
using RoR2;
using RoR2.Stats;

namespace RiskyMod.Fixes
{
    public class FixCrocoPoisonAchievement
    {
        public FixCrocoPoisonAchievement()
        {
            On.RoR2.DotController.AddDot_GameObject_float_HurtBox_DotIndex_float_Nullable1_Nullable1_Nullable1 += FixAchieveTracking;
        }

        private void FixAchieveTracking(On.RoR2.DotController.orig_AddDot_GameObject_float_HurtBox_DotIndex_float_Nullable1_Nullable1_Nullable1 orig, DotController self, UnityEngine.GameObject attackerObject, float duration, HurtBox hitHurtBox, DotController.DotIndex dotIndex, float damageMultiplier, uint? maxStacksFromAttacker, float? totalDamage, DotController.DotIndex? preUpgradeDotIndex)
        {
            orig(self, attackerObject, duration, hitHurtBox, dotIndex, damageMultiplier, maxStacksFromAttacker, totalDamage, preUpgradeDotIndex);
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
        }
    }
}
