using BepInEx.Configuration;
using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Captain;
using UnityEngine;

namespace RiskyMod.Survivors
{
    public class FireSelect
    {
        public static ConfigEntry<bool> scrollSelection;
        public static ConfigEntry<KeyboardShortcut> swapButton;
        public static ConfigEntry<KeyboardShortcut> prevButton;

        public FireSelect()
        {
            new CaptainFireModes();
            new BanditFireModes();
        }
    }
}
