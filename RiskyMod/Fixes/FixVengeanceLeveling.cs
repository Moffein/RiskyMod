using RoR2;
using UnityEngine.Networking;

namespace RiskyMod.Fixes
{
    public class FixVengeanceLeveling
    {
        public static bool enabled = true;
        public FixVengeanceLeveling()
        {
            if (!enabled) return;
            On.RoR2.CharacterBody.Start += (orig2, self) =>
            {
                orig2(self);
                if (NetworkServer.active && self.teamComponent && self.teamComponent.teamIndex == TeamIndex.Monster
                && self.inventory && self.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0 && self.inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel) <= 0)
                {
                    self.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                }
            };
        }
    }
}
