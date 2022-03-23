using MonoMod.Cil;
using System;
using RoR2;

namespace RiskyMod.Allies
{
    public class IncreaseShotRadius
    {
        public static bool enabled = true;
        public IncreaseShotRadius()
        {
            if (!enabled) return;

            //MegaDrone
            IL.EntityStates.Drone.DroneWeapon.FireMegaTurret.FireBullet += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                     );
                c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                {
                    bulletAttack.radius = 0.3f;
                    bulletAttack.smartCollision = true;
                    return bulletAttack;
                });
            };

            //Gunner Turret
            IL.EntityStates.Drone.DroneWeapon.FireGatling.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                     );
                c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                {
                    bulletAttack.radius = 0.3f;
                    bulletAttack.smartCollision = true;
                    return bulletAttack;
                });
            };

            //Gunner Drone and Backup
            IL.EntityStates.Drone.DroneWeapon.FireTurret.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                     );
                c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                {
                    bulletAttack.radius = 0.3f;
                    bulletAttack.smartCollision = true;
                    return bulletAttack;
                });
            };
        }
    }
}
