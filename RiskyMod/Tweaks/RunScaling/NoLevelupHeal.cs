using RoR2;

namespace RiskyMod.Tweaks.RunScaling
{
    //Merged from https://github.com/Moffein/NoLevelupHeal
    public class NoLevelupHeal
    {
        public static bool enabled = true;
        public NoLevelupHeal()
        {
            if (!enabled) return;

            //Todo: test to see if this glitches out with the AWU shield
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                float oldLevel = self.level;
                float oldHP = self.healthComponent.health;
                float oldShield = self.healthComponent.shield;
                orig(self);
                if (self.level > oldLevel)
                {
                    if (self.teamComponent.teamIndex != TeamIndex.Player && self.healthComponent.combinedHealthFraction < 1f)
                    {
                        if (self.healthComponent.health > oldHP)
                        {
                            self.healthComponent.health = oldHP;
                        }

                        if (self.healthComponent.shield > oldShield)
                        {
                            self.healthComponent.shield = oldShield;
                        }
                    }
                }
            };
        }
    }
}
