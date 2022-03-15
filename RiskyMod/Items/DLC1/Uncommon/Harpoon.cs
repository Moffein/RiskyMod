using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using RoR2;

namespace RiskyMod.Items.DLC1.Uncommon
{
    public class Harpoon
    {
        public static bool enabled = true;
        public Harpoon()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "MoveSpeedOnKill")
                    );

                c.GotoNext(
                     x => x.MatchLdcR4(1f)
                    );
                c.Next.Operand = 3f;

                c.GotoNext(
                     x => x.MatchLdcR4(0.5f)
                    );
                c.Next.Operand = 1.5f;
            };
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.MoveSpeedOnKill);
        }
    }
}
