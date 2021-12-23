using RoR2;

namespace RiskyMod.Fixes
{
    //Merged from https://github.com/Moffein/NoLevelupHeal
    public class NoLevelupHeal
    {
        public static bool enabled = true;
        public NoLevelupHeal()
        {
            if (!enabled) return;
            //TODO: See if this can be refactored to not glitch out with AWU shield.
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                float oldLevel = self.level;
                float oldHP = self.healthComponent.health;
                float oldShield = self.healthComponent.shield;
                orig(self);
                if (self.level > oldLevel)
                {
                    if ((self.teamComponent.teamIndex != TeamIndex.Player || !self.isPlayerControlled) && self.healthComponent.combinedHealthFraction < 1f)
                    {
                        self.healthComponent.health = oldHP;
                        if (self.baseNameToken != "SUPERROBOBALLBOSS_BODY_NAME")
                        {
                            self.healthComponent.shield = oldShield;
                        }
                    }
                }
            };
        }
    }
}
