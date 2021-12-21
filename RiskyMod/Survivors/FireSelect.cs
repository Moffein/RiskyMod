using RiskyMod.Survivors.Captain;
using RiskyMod.Survivors.Croco;
using RiskyMod.Survivors.Engi;
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
            new EngiFireModes();
        }
    }
}
