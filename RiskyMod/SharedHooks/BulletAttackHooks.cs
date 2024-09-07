using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.SharedHooks
{
    public static class BulletAttackHooks
    {
        public static void RemoveBulletFalloff(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCallvirt(typeof(BulletAttack), "Fire")))
            {
                c.EmitDelegate<Func<BulletAttack, BulletAttack>>(orig =>
                {
                    orig.falloffModel = BulletAttack.FalloffModel.None;
                    return orig;
                });
            }
        }

        public static void AddBulletFalloff(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCallvirt(typeof(BulletAttack), "Fire")))
            {
                c.EmitDelegate<Func<BulletAttack, BulletAttack>>(orig =>
                {
                    orig.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
                    return orig;
                });
            }
        }
    }
}
