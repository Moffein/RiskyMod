using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class BanditSpecialGracePeriod
    {
        public static bool enabled = true;

        public static float durationOnline = 1.0f;
        public static float durationLocalUser = 0.5f;

        public static GameObject resetEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/Bandit2ResetEffect");
        public static GameObject skullEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/Bandit2KillEffect");
        public BanditSpecialGracePeriod()
        {
            if (!enabled) return;
            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdcI4(4),
                     x => x.MatchAnd(),
                     x => x.MatchLdcI4(4)
                    ))
                {
                    c.EmitDelegate<Func<DamageType, DamageType>>((orig) =>
                    {
                        return DamageType.Generic;
                    });

                    if(c.TryGotoNext(
                         x => x.MatchLdcI4(268435456),
                         x => x.MatchAnd(),
                         x => x.MatchLdcI4(268435456)
                        ))
                    {
                        c.EmitDelegate<Func<DamageType, DamageType>>((orig) =>
                        {
                            return DamageType.Generic;
                        });
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: BanditSpecialGracePeriod IL Hook failed");
                }
            };
        }

        public static float GetDuration(GameObject attacker)
        {
            return SneedUtils.SneedUtils.IsLocalUser(attacker) ? durationLocalUser : durationOnline;
        }

        public static float GetDuration(CharacterBody attackerBody)
        {
            return SneedUtils.SneedUtils.IsLocalUser(attackerBody) ? durationLocalUser : durationOnline;
        }
    }
}
