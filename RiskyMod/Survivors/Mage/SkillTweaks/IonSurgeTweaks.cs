using System;
using MonoMod.Cil;
using RoR2;

namespace RiskyMod.Survivors.Mage
{
    public class IonSurgeTweaks
    {
        public IonSurgeTweaks()
        {
            IL.EntityStates.Mage.FlyUpState.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BlastAttack>("Fire")
                    );
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>((blastAttack) =>
                {
                    blastAttack.damageType = DamageType.Shock5s;
                    return blastAttack;
                });
            };

            IL.EntityStates.Mage.FlyUpState.HandleMovements += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdfld<EntityStates.BaseState>("moveSpeedStat")
                    );
                c.Index++;
                c.EmitDelegate<Func<float, float>>(orig =>
                {
                    return 7f;
                });
            };
        }
    }
}
