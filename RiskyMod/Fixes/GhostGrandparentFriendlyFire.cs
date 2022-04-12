using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Fixes
{
    public class GhostGrandparentFriendlyFire
    {
        public static bool enabled = true;
        public GhostGrandparentFriendlyFire()
        {
            if (!enabled) return;
            IL.RoR2.GrandParentSunController.ServerFixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                     x => x.MatchLdfld<RoR2.HurtBox>("healthComponent")
                    );
                c.Emit(OpCodes.Ldarg_0);    //suncontroller
                c.EmitDelegate<Func<HealthComponent, GrandParentSunController, HealthComponent>>((victimHealth, self) =>
                {
                    if (self.teamFilter && self.teamFilter.teamIndex == TeamIndex.Player
                    && victimHealth && victimHealth.body.teamComponent && victimHealth.body.teamComponent.teamIndex == TeamIndex.Player)
                    {
                        return null;
                    }
                    return victimHealth;
                });
            };

            IL.RoR2.GrandParentSunController.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                     x => x.MatchLdfld<HealthComponent>("body")
                    );
                c.Emit(OpCodes.Ldarg_0);    //suncontroller
                c.EmitDelegate<Func<CharacterBody, GrandParentSunController, CharacterBody>>((victimBody, self) =>
                {
                    if (self.teamFilter && self.teamFilter.teamIndex == TeamIndex.Player
                    && victimBody && victimBody.teamComponent && victimBody.teamComponent.teamIndex == TeamIndex.Player)
                    {
                        return null;
                    }
                    return victimBody;
                });
            };
        }
    }
}
