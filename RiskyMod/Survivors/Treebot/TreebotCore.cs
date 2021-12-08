using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Treebot
{
    public class TreebotCore
    {
        public static bool enabled = true;
        public TreebotCore()
        {
            if (!enabled) return;
        }
    }
}
