using MonoMod.Cil;
using System;
using RoR2;
using UnityEngine;

namespace RiskyMod.Allies
{
    public class ModifyBulletAttacks
    {
        public static bool enabled = true;
        public ModifyBulletAttacks()
        {
            if (!enabled) return;

            //Gunner Turret
            IL.EntityStates.Drone.DroneWeapon.FireGatling.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                     ))
                {
                    c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                    {
                        bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
                        bulletAttack.radius = 0.5f;
                        bulletAttack.smartCollision = true;
                        return bulletAttack;
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: ModifyBulletAttacks DroneWeapon.FireGatling IL Hook failed");
                }
            };

            //Gunner Drone and Backup
            IL.EntityStates.Drone.DroneWeapon.FireTurret.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                     ))
                {
                    c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                    {
                        bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
                        bulletAttack.radius = 0.5f;
                        bulletAttack.smartCollision = true;
                        return bulletAttack;
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: ModifyBulletAttacks DroneWeapon.FireTurret IL Hook failed");
                }
            };
        }
    }
}
