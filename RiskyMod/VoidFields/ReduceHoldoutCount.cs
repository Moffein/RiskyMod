using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.VoidFields
{
    public class ReduceHoldoutCount
    {
        public static bool enabled = true;

        public ReduceHoldoutCount()
        {
            if (!enabled) return;

            On.RoR2.ArenaMissionController.Awake += SetRoundsAndOverrideRewards;
            On.RoR2.ArenaMissionController.BeginRound += AugmentBeginRound;
            On.RoR2.ArenaMissionController.AddMonsterType += PreventOnRound5;
        }

        //This is prone to breaking things if other mods want to use ArenaMissionController for something.
        private void PreventOnRound5(On.RoR2.ArenaMissionController.orig_AddMonsterType orig, ArenaMissionController self)
        {
            if (self.currentRound == 5) return;
            orig(self);
        }

        //Would be better to disable the switch statement with IL and recreate the whole switch statement here
        private void AugmentBeginRound(On.RoR2.ArenaMissionController.orig_BeginRound orig, ArenaMissionController self)
        {
            if (NetworkServer.active)
            {
                switch (self.currentRound + 1)
                {
                    case 1:
                        self.AddItemStack();
                        break;
                    case 2:
                        self.AddMonsterType();
                        break;
                    case 3:
                        self.AddItemStack();
                        break;
                    case 4:
                        self.AddMonsterType();
                        break;
                    case 5:
                        self.AddItemStack();
                        break;
                }
            }

            orig(self);
        }

        private void SetRoundsAndOverrideRewards(On.RoR2.ArenaMissionController.orig_Awake orig, RoR2.ArenaMissionController self)
        {
            orig(self);
            self.totalRoundsMax = 5;
            for (int i = 0; i < 5 && i < self.playerRewardOrder.Length; i++)
            {
                switch(i)
                {
                    case 0:
                        self.playerRewardOrder[i] = RiskyMod.tier1Drops;
                        break;
                    case 1:
                        self.playerRewardOrder[i] = RiskyMod.tier1Drops;
                        break;
                    case 2:
                        self.playerRewardOrder[i] = RiskyMod.tier2Drops;
                        break;
                    case 3:
                        self.playerRewardOrder[i] = RiskyMod.tier2Drops;
                        break;
                    case 4:
                        self.playerRewardOrder[i] = RiskyMod.tier3Drops;
                        break;
                }
            }
        }
    }
}
