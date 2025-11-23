using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.Boss
{
    public class HalcyonSeed
    {
        public static bool enabled = true;

        public HalcyonSeed()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            On.RoR2.CharacterBody.Start += CharacterBody_Start;
        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            if (!self.isPlayerControlled && self.bodyIndex == RoR2Content.BodyPrefabs.TitanGoldBody.bodyIndex && self.teamComponent && self.teamComponent.teamIndex == TeamIndex.Player)
            {
                if (NetworkServer.active)
                {
                    self.inventory.GiveItemPermanent(RoR2Content.Items.BoostHp, 10); //Increase initial stack health
                    self.inventory.GiveItemPermanent(RoR2Content.Items.AdaptiveArmor);
                }
                self.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ImmuneToExecutes | CharacterBody.BodyFlags.ImmuneToLava;
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.TitanGoldDuringTP);
        }
    }
}
