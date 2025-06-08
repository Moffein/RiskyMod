using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.Boss
{
    public class HalcyonSeed
    {
        public static bool enabled = true;
        public static BodyIndex TitanGoldIndex;

        public HalcyonSeed()
        {
            if (enabled)
            {
                ItemsCore.ModifyItemDefActions += ModifyItem;
            }

            RoR2Application.onLoad += OnLoad;

            On.RoR2.CharacterBody.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && !self.isPlayerControlled && self.bodyIndex == TitanGoldIndex && self.teamComponent && self.teamComponent.teamIndex == TeamIndex.Player)
                {
                    if (self.inventory)
                    {
                        if (HalcyonSeed.enabled)
                        {
                            self.inventory.GiveItem(RoR2Content.Items.BoostHp, 10); //Increase initial stack health
                            self.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
                        }

                        self.inventory.GiveItem(Allies.AllyItems.AllyMarkerItem);
                        self.inventory.GiveItem(Allies.AllyItems.AllyScalingItem);

                        //Debug.Log("Uses Ambient Level: " + (self.inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel) > 0)); //This DOES use ambient level
                    }
                }
            };
        }

        private void OnLoad()
        {
            HalcyonSeed.TitanGoldIndex = BodyCatalog.FindBodyIndex("TitanGoldBody");
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.TitanGoldDuringTP);
        }
    }
}
