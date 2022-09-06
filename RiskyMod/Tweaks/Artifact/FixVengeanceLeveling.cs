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
            RoR2.CharacterMaster.onStartGlobal += RunVengeanceChanges;
        }

        private void RunVengeanceChanges(CharacterMaster self)
        {
            if (NetworkServer.active && self.inventory && self.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0)
            {
                int alCount = self.inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel);
                if (alCount > 0) self.inventory.RemoveItem(RoR2Content.Items.UseAmbientLevel, alCount);

                int lbCount = self.inventory.GetItemCount(RoR2Content.Items.LevelBonus);
                if (lbCount > 0) self.inventory.RemoveItem(RoR2Content.Items.LevelBonus, lbCount);

                self.inventory.GiveItem(RoR2Content.Items.LevelBonus, (int)TeamManager.instance.GetTeamLevel(TeamIndex.Player) - 1);
            }
        }
    }
}
