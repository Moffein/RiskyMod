using RoR2;

namespace RiskyMod.Tweaks
{
    public class AIBlacklistItems
    {
        public static bool enabled = true;
        public AIBlacklistItems()
        {
            if (!enabled) return;

            On.RoR2.ItemCatalog.Init += (orig) =>
            {
                orig();
                if (SoftDependencies.AIBlacklistUseVanillaBlacklist)
                {
                    SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.Icicle, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.RoboBallBuddy, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.CaptainDefenseMatrix, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.Bear, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.ShockNearby, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(RoR2Content.Items.NovaOnHeal, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(DLC1Content.Items.ImmuneToDebuff, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(DLC1Content.Items.DroneWeapons, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(DLC1Content.Items.FreeChest, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(DLC1Content.Items.RegeneratingScrap, ItemTag.AIBlacklist);
                    SneedUtils.SneedUtils.AddItemTag(DLC1Content.Items.PrimarySkillShuriken, ItemTag.AIBlacklist);
                }
            };
        }
    }
}
