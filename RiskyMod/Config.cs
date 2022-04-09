using BepInEx.Configuration;
using System.IO;

namespace RiskyMod
{
    public static class Config
    {
        public static ConfigFile Items;
        public static ConfigFile Survivors;
        public static string ConfigFolderPath { get => Path.Combine(BepInEx.Paths.ConfigPath, RiskyMod.pluginInfo.Metadata.GUID); }


        private const string uncommonString = "Items - Uncommon";
        private const string commonString = "Items - Common";
        private const string legendaryString = "Items - Legendary";
        private const string voidString = "Items - Void";
        private const string bossString = "Items - Boss";
        private const string lunarString = "Items - Lunar";
        private const string equipmentString = "Items - Equipment";
        private const string itemConfigDescString = "Enable changes to this item.";

        private const string commandoString = "Survivors: Commando";
        private const string huntressString = "Survivors: Huntress";
        private const string toolbotString = "Survivors: MUL-T";
        private const string engiString = "Survivors: Engineer";
        private const string treebotString = "Survivors: REX";
        private const string crocoString = "Survivors: Acrid";
        private const string banditString = "Survivors: Bandit";
        private const string captainString = "Survivors: Captain";
        private const string mageString = "Survivors: Artificer";
        private const string loaderString = "Survivors: Loader";
        private const string mercString = "Survivors: Mercenary";
        private const string railgunnerString = "Survivors: Railgunner";
        private const string voidFiendString = "Survivors: Void Fiend";

        public static void ReadConfig()
        {
            Items = new ConfigFile(Path.Combine(ConfigFolderPath, $"RiskyMod_Items.cfg"), true);
            Survivors = new ConfigFile(Path.Combine(ConfigFolderPath, $"RiskyMod_Survivors.cfg"), true);
        }
    }
}
