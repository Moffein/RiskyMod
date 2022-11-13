using RoR2;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;


namespace RiskyMod.Items.DLC1.Void
{
    public class Needletick
    {
        public static bool enabled = true;

        public Needletick()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "BleedOnHitVoid")
                    )
                    &&
                c.TryGotoNext(MoveType.After,
                     x => x.MatchLdcR4(1f)
                    ))
                {
                    c.Emit(OpCodes.Ldloc_2);    //VictimBody
                    c.EmitDelegate<Func<float, CharacterBody, float>>((damageMult, victimBody) =>
                    {
                        if (victimBody)
                        {
                            int collapseStacks = victimBody.GetBuffCount(DLC1Content.Buffs.Fracture);
                            damageMult = 0.75f * (3f + 0.2f*collapseStacks) * 0.3333333333f; //This scales way too hard.
                        }
                        return damageMult;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Needletick IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.BleedOnHitVoid);
        }
    }
}
