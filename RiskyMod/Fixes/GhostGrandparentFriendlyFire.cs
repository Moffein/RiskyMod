using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Fixes
{
    public class GhostGrandparentFriendlyFire
    {
        public static bool enabled = true;
        public GhostGrandparentFriendlyFire()
        {
            if (!enabled) return;

            //Handles the damage
            IL.RoR2.GrandParentSunController.ServerFixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld<RoR2.HurtBox>("healthComponent")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);    //suncontroller
                    c.EmitDelegate<Func<HealthComponent, GrandParentSunController, HealthComponent>>((victimHealth, self) =>
                    {
                        GameObject ownerObject = self.ownership.ownerObject;
                        if (ownerObject)
                        {
                            TeamComponent tc = ownerObject.GetComponent<TeamComponent>();
                            if (tc && tc.teamIndex == TeamIndex.Player
                            && victimHealth && victimHealth.body.teamComponent && victimHealth.body.teamComponent.teamIndex == TeamIndex.Player)
                            {
                                return null;
                            }
                        }
                        return victimHealth;
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: GhostGrandparentFriendlyFire ServerFixedUpdate IL Hook failed");
                }
            };

            //Handles the visuals
            IL.RoR2.GrandParentSunController.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);

                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld<HealthComponent>("body")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);    //suncontroller
                    c.EmitDelegate<Func<CharacterBody, GrandParentSunController, CharacterBody>>((victimBody, self) =>
                    {
                        GameObject ownerObject = self.ownership.ownerObject; if (ownerObject)
                        {
                            TeamComponent tc = ownerObject.GetComponent<TeamComponent>();
                            if (tc && tc.teamIndex == TeamIndex.Player
                            && victimBody && victimBody.teamComponent && victimBody.teamComponent.teamIndex == TeamIndex.Player)
                            {
                                return null;
                            }
                        }
                        return victimBody;
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: GhostGrandparentFriendlyFire FixedUpdate IL Hook failed");
                }
            };
        }
    }
}
