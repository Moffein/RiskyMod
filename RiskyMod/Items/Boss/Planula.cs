using MonoMod.Cil;
using RoR2;
using RoR2.Audio;
using System;

namespace RiskyMod.Items.Boss
{
    //This is just here to prevent anti-synergy with the Stealthkit/Razorwire/Polyp changes (Planula healing before HP lost is calculated)
    public class Planula
    {
        public static bool enabled = true;
        private static NetworkSoundEventDef eggSound;
        public Planula()
        {
            if (!enabled) return;
            eggSound = LegacyResourcesAPI.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseParentEggHeal");
            //Remove vanilla effect
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(HealthComponent.ItemCounts), "parentEgg")
                    );
                c.EmitDelegate<Func<int, int>>(orig =>
                {
                    return 0;
                });
            };

            SharedHooks.TakeDamage.TakeDamageEndActions += TakeDamageEnd;
        }

        private static void TakeDamageEnd(DamageInfo damageInfo, HealthComponent self)
        {
            int planulaCount = self.itemCounts.parentEgg;
            if (planulaCount > 0)
            {
                self.Heal(planulaCount * 15f, default(ProcChainMask), true);
                EntitySoundManager.EmitSoundServer(eggSound.index, self.gameObject);
            }
        }
    }
}
