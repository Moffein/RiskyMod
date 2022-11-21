using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace RiskyMod.Survivors.DLC1.Railgunner
{
    public class FixBungus
    {
        public static bool enabled = true;
        public FixBungus()
        {
            IL.EntityStates.Railgunner.Weapon.BaseFireSnipe.OnFireBulletAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(EntityStates.Railgunner.Weapon.BaseFireSnipe), "selfKnockbackForce")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, EntityStates.Railgunner.Weapon.BaseFireSnipe, float>>((force, self) =>
                    {
                        if (self.characterMotor && self.characterMotor.velocity == UnityEngine.Vector3.zero)
                        {
                            return 0;
                        }
                        return force;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Railgunner FixBungus Snipe IL Hook failed");
                }
            };

            IL.EntityStates.Railgunner.Weapon.FirePistol.FireBullet += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(EntityStates.Railgunner.Weapon.FirePistol), "selfKnockbackForce")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, EntityStates.Railgunner.Weapon.FirePistol, float>>((force, self) =>
                    {
                        if (self.characterMotor && self.characterMotor.isGrounded)
                        {
                            return 0;
                        }
                        return force;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Railgunner FixBungus FirePistol IL Hook failed");
                }
            };
        }
    }
}
