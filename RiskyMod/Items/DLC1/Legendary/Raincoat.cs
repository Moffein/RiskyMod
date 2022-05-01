using RoR2;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class Raincoat
    {
        public static bool enabled = false;

        public Raincoat()
        {
            if (!enabled) return;

            On.RoR2.BuffCatalog.Init += (orig) =>
            {
                orig();

                DLC1Content.Buffs.ImmuneToDebuffCooldown.iconSprite = Content.Assets.BuffIcons.RaincoatCooldown;
                DLC1Content.Buffs.ImmuneToDebuffReady.iconSprite = Content.Assets.BuffIcons.RaincoatReady;
            };
        }
    }
}
