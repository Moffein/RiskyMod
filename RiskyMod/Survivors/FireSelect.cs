using RiskyMod.Survivors.Bandit2;
using RiskyMod.Survivors.Captain;
using UnityEngine;

namespace RiskyMod.Survivors
{
    public class FireSelect
    {
        public static bool scrollSelection = true;
        public static KeyCode swapButton = KeyCode.None;
        public static KeyCode prevButton = KeyCode.None;

        public FireSelect()
        {
            new CaptainFireModes();
            new BanditFireModes();
        }
    }
}
