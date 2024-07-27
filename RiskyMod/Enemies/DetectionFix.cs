using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Enemies
{
    public class DetectionFix
    {
        public static bool enabled = true;

        public DetectionFix()
        {
            if (!enabled) return;

            IL.RoR2.CharacterAI.BaseAI.GameObjectPassesSkillDriverFilters += BaseAI_GameObjectPassesSkillDriverFilters;
        }

        private void BaseAI_GameObjectPassesSkillDriverFilters(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchCallvirt<RoR2.CharacterAI.AISkillDriver>("get_minDistanceSqr")))
            {
                c.EmitDelegate<Func<float, float>>(orig =>
                {
                    if (orig <= 0f) orig = float.NegativeInfinity;
                    return orig;
                });
            }
        }
    }
}
