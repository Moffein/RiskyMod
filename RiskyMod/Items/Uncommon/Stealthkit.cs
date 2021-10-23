using R2API;
using UnityEngine;

namespace RiskyMod.Items.Uncommon
{
    public class Stealthkit
    {
        public static bool enabled = true;
        public static GameObject effectPrefab = Resources.Load<GameObject>("Prefabs/Effects/ProcStealthkit");
        public Stealthkit()
        {
            if (!enabled) return;

            //Effect handled in SharedHooks.TakeDamage

            //Disable vanilla behavior
            On.RoR2.CharacterBody.PhasingItemBehaviorServer.Start += (orig, self) =>
            {
                UnityEngine.Object.Destroy(self);
                return;
            };

            LanguageAPI.Add("ITEM_PHASING_PICKUP", "Turn invisible on taking heavy damage.");
            LanguageAPI.Add("ITEM_PHASING_DESC", "Chance on taking damage to gain <style=cIsUtility>40% movement speed</style> and <style=cIsUtility>invisibility</style> for <style=cIsUtility>3s</style> <style=cStack>(+1.5s per stack)</style>. Chance increases the more damage you take.");
        }
    }
}
