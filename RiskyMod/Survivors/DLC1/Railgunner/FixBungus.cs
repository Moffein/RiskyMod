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
                c.GotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(EntityStates.Railgunner.Weapon.BaseFireSnipe), "selfKnockbackForce")
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, EntityStates.Railgunner.Weapon.BaseFireSnipe, float>>((force, self) =>
                {
                    if (self.characterMotor && self.characterMotor.isGrounded)
                    {
                        return 0;
                    }
                    return force;
                });
            };

            IL.EntityStates.Railgunner.Weapon.FirePistol.FireBullet += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                     x => x.MatchLdfld(typeof(EntityStates.Railgunner.Weapon.FirePistol), "selfKnockbackForce")
                    );
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, EntityStates.Railgunner.Weapon.FirePistol, float>>((force, self) =>
                {
                    if (self.characterMotor && self.characterMotor.isGrounded)
                    {
                        return 0;
                    }
                    return force;
                });
            };
        }
    }
}
