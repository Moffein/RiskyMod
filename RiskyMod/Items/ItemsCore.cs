using RiskyMod.Items.Boss;
using RiskyMod.Items.Common;
using RiskyMod.Items.DLC1.Legendary;
using RiskyMod.Items.DLC1.Void;
using RiskyMod.Items.Equipment;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Lunar;
using RiskyMod.Items.Uncommon;
using RoR2;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace RiskyMod.Items
{
    public class ItemsCore
    {
        public static bool enabled = true;
        public static bool uncommonEnabled = true;
        public static bool commonEnabled = true;
        public static bool legendaryEnabled = true;
        public static bool bossEnabled = true;
        public static bool lunarEnabled = true;
        public static bool equipmentEnabled = true;

        public static ItemDef[] changedItemPickups = new ItemDef[0];
        public static ItemDef[] changedItemDescs = new ItemDef[0];

        public static EquipmentDef[] changedEquipPickups = new EquipmentDef[0];
        public static EquipmentDef[] changedEquipDescs = new EquipmentDef[0];

        public ItemsCore()
        {
            if (!enabled) return;
            Planula.enabled = Stealthkit.enabled || Razorwire.enabled || SquidPolyp.enabled;
            ModifyCommon();
            ModifyUncommon();
            ModifyLegendary();
            ModifyVoid();
            ModifyBoss();
            ModifyLunar();
            ModifyEquipment();
            ModifyItemTokens();
        }

        private void ModifyCommon()
        {
            if (!commonEnabled) return;
            new BisonSteak();
            new MonsterTooth();
            new CritGlasses();
            new Fireworks();
            new StickyBomb();
            new Crowbar();
            new Warbanner();
            new Gasoline();
            new TopazBrooch();
            new TougherTimes();
        }

        private void ModifyUncommon()
        {
            if (!uncommonEnabled) return;
            new Predatory();
            new Chronobauble();
            new LeechingSeed();
            new AtG();
            new ElementalBands();
            new Bandolier();
            new Stealthkit();
            new WillOWisp();
            new SquidPolyp();
            new Ukulele();
            new Razorwire();
            new RoseBuckler();
            new Guillotine();
            new Berzerker();
            new HarvesterScythe();
            new WarHorn();
            new Infusion();
        }

        private void ModifyLegendary()
        {
            if (!legendaryEnabled) return;
            new Tesla();
            new FrostRelic();
            new CeremonialDagger();
            new MeatHook();
            new LaserTurbine();
            new HeadHunter();
            new Headstompers();
            new NovaOnHeal();
            new Brainstalks();
            new Soulbound();
            new Behemoth();
            new HappiestMask();

            new LaserScope();
            new BottledChaos();
        }

        private void ModifyVoid()
        {
            new PlasmaShrimp();
            new VoidWisp();
            new Polylute();
        }

        private void ModifyBoss()
        {
            if (!bossEnabled) return;
            new QueensGland();
            new MoltenPerf();
            new ChargedPerf();
            new Shatterspleen();
            new Knurl();
            new Disciple();
            new GenesisLoop();
            new Planula();
        }

        private void ModifyLunar()
        {
            if (!lunarEnabled) return;
            new ShapedGlass();
            new Transcendence();
            new BrittleCrown();
            new Meteorite();
        }

        private void ModifyEquipment()
        {
            if (!equipmentEnabled) return;
            new CritHud();
            new VolcanicEgg();
            new BFG();
            new Capacitor();
            new Backup();
            new SuperLeech();
        }

        private void ModifyItemTokens()
        {
            On.RoR2.ItemCatalog.Init += (orig) =>
            {
                orig();

                if (ModifyItemDefActions != null) ModifyItemDefActions.Invoke();

                foreach (ItemDef item in changedItemPickups)
                {
                    item.pickupToken += "_RISKYMOD";
                }
                foreach (ItemDef item in changedItemDescs)
                {
                    item.descriptionToken += "_RISKYMOD";
                }
                foreach (EquipmentDef item in changedEquipPickups)
                {
                    item.pickupToken += "_RISKYMOD";
                }
                foreach (EquipmentDef item in changedEquipDescs)
                {
                    item.descriptionToken += "_RISKYMOD";
                }
            };
        }

        public delegate void ModifyItemDef();
        public static ModifyItemDef ModifyItemDefActions;

        public static EquipmentDef LoadEquipmentDef(string equipmentname)
        {
            return LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/" + equipmentname);
        }

        public static void ChangeEquipmentCooldown(EquipmentDef ed, float cooldown)
        {
            ed.cooldown = cooldown;
        }
    }
}
