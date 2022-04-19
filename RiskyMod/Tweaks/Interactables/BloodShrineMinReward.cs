using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Tweaks
{
    public class BloodShrineMinReward
    {
        public static bool enabled = true;
        public static int chestCost = 25;
        public BloodShrineMinReward()
        {
            if (!enabled) return;
            On.RoR2.Stage.Start += (orig, self) =>
            {
                orig(self);
                chestCost = Run.instance.GetDifficultyScaledCost(25, Run.instance.difficultyCoefficient);
            };
            IL.RoR2.ShrineBloodBehavior.AddShrineStack += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchStloc(1)
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<uint, ShrineBloodBehavior, uint>>((cost, self) =>
                {
                    float mult = (float)self.purchaseInteraction.cost / 50f;
                    int newCost = (int)(chestCost * mult);
                    return (uint)Mathf.Max((int)cost, newCost);
                });
            };
        }
    }
}
