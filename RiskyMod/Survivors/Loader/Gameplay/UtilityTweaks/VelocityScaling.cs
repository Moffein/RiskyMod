using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace RiskyMod.Survivors.Loader
{
    public class VelocityScaling
    {
        public VelocityScaling()
        {
            IL.EntityStates.Loader.BaseSwingChargedFist.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCall<EntityStates.Loader.BaseSwingChargedFist>("get_punchSpeed")
                    );
                c.Index++;
                c.EmitDelegate<Func<float, float>>(speed =>
                {
                    return GetScaledSpeed(speed);
                });
            };
        }

        public static float GetScaledSpeed(float speed)
        {
            if (speed > 100f)
            {
                speed = 100f + 1.5f * (speed - 100f);
            }
            return speed;
        }
    }
}
