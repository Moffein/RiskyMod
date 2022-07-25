using RoR2;
using UnityEngine.Networking;

namespace RiskyMod.Enemies.Mobs
{
    public class VoidReaver
    {
        public static bool enabled = true;

        public VoidReaver()
        {
            if (!enabled) return;

            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                orig(self, damageInfo);
                if (NetworkServer.active && !damageInfo.rejected && damageInfo.damageType.HasFlag(RoR2.DamageType.Nullify))
                {
                    self.body.AddTimedBuff(RoR2Content.Buffs.NullifyStack, 8f);
                }
            };
        }
    }
}
