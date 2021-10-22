using Risky_Mod.Items.Boss;
using Risky_Mod.Items.Common;
using Risky_Mod.Items.Legendary;
using Risky_Mod.Items.Lunar;
using Risky_Mod.Items.Uncommon;
using RoR2;

namespace Risky_Mod.Items
{
    public class ItemsCore
    {
        public static bool itemTweaksEnabled = true;
        public static bool uncommonEnabled = true;
        public static bool commonEnabled = true;
        public static bool legendaryEnabled = true;
        public static bool bossEnabled = true;
        public static bool lunarEnabled = true;

        public void Modify()
        {
            if (!itemTweaksEnabled) return;
            Planula.enabled = Stealthkit.enabled || Razorwire.enabled || SquidPolyp.enabled;
            ModifyCommon();
            ModifyUncommon();
            ModifyLegendary();
            ModifyBoss();
            ModifyLunar();
        }

        private void ModifyCommon()
        {
            if (!commonEnabled) return;
            BisonSteak.Modify();
            MonsterTooth.Modify();
            CritGlasses.Modify();
            Fireworks.Modify();
            StickyBomb.Modify();
            Crowbar.Modify();
            Warbanner.Modify();
            Gasoline.Modify();
            RepArmor.Modify();
        }

        private void ModifyUncommon()
        {
            if (!uncommonEnabled) return;
            Predatory.Modify();
            Chronobauble.Modify();
            LeechingSeed.Modify();
            AtG.Modify();
            ElementalBands.Modify();
            Bandolier.Modify();
            Stealthkit.Modify();
            WillOWisp.Modify();
            SquidPolyp.Modify();
            Ukulele.Modify();
            Razorwire.Modify();
            RoseBuckler.Modify();
            Guillotine.Modify();
            Berzerker.Modify();
        }

        private void ModifyLegendary()
        {
            if (!legendaryEnabled) return;
            Tesla.Modify();
            FrostRelic.Modify();
            CeremonialDagger.Modify();
            MeatHook.Modify();
            LaserTurbine.Modify();
            Headhunter.Modify();
            Headstompers.Modify();
            NovaOnHeal.Modify();
        }

        private void ModifyBoss()
        {
            if (!bossEnabled) return;
            QueensGland.Modify();
            MoltenPerf.Modify();
            ChargedPerf.Modify();
            Shatterspleen.Modify();
            Knurl.Modify();
            Disciple.Modify();
            Planula.Modify();
            GenesisLoop.Modify();
        }

        private void ModifyLunar()
        {
            if (!lunarEnabled) return;
            ShapedGlass.Modify();
        }

        public static void AddToAIBlacklist(string itemName)
        {
            ItemIndex i = ItemCatalog.FindItemIndex(itemName);
            if (i != ItemIndex.None)
            {
                AddToAIBlacklist(i);
            }
        }

        public static void AddToAIBlacklist(ItemIndex index)
        {
            //Debug.Log("Adding BrotherBlacklist tag to ItemIndex " + index);
            ItemDef itemDef = ItemCatalog.GetItemDef(index);
            if (itemDef.DoesNotContainTag(ItemTag.BrotherBlacklist))
            {
                System.Array.Resize(ref itemDef.tags, itemDef.tags.Length + 1);
                itemDef.tags[itemDef.tags.Length - 1] = ItemTag.BrotherBlacklist;
            }
        }
    }
}
