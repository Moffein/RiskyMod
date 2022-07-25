using RoR2;
using UnityEngine.Networking;

namespace RiskyMod.Tweaks.Artifact
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
                if (NetworkServer.active && self.teamComponent && self.teamComponent.teamIndex != TeamIndex.Player
                && self.inventory && self.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0)
                {
                    self.inventory.RemoveItem(RoR2Content.Items.UseAmbientLevel, self.inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel));
                    self.inventory.RemoveItem(RoR2Content.Items.LevelBonus, self.inventory.GetItemCount(RoR2Content.Items.LevelBonus));
                    self.inventory.GiveItem(RoR2Content.Items.LevelBonus, (int)TeamManager.instance.GetTeamLevel(TeamIndex.Player) - 1);
                }
            };
        }
    }
}
