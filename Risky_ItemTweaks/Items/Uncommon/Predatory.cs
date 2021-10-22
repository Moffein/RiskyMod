using RoR2;

namespace Risky_Mod.Items.Uncommon
{
    public class Predatory
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            On.RoR2.CharacterBody.AddTimedBuff_BuffIndex_float += (orig, self, buffIndex, duration) =>
            {
                orig(self, buffIndex, duration);
                if (buffIndex == RoR2Content.Buffs.AttackSpeedOnCrit.buffIndex)
                {
                    foreach(CharacterBody.TimedBuff tb in self.timedBuffs)
                    {
                        if (tb.buffIndex == RoR2Content.Buffs.AttackSpeedOnCrit.buffIndex && tb.timer < duration)
                        {
                            tb.timer = duration;
                        }
                    }
                }
            };
        }
    }
}
