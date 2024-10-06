using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Engi
{
    public class PressureMines
    {
        public static bool enabled = true;
        public PressureMines()
        {
            if (!enabled) return;

            //Default 200
            //Spider 1000
            //Railgunner 4000
            //Debug.Log("Default:");
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Engi/EntityStates.Engi.EngiWeapon.FireMines.asset", "force", "4000");

            SkillDef mines = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Engi/EngiBodyPlaceMine.asset").WaitForCompletion();
            mines.skillDescriptionToken = "ENGI_SECONDARY_DESCRIPTION_RISKYMOD";

            IL.EntityStates.Engi.Mine.WaitForTarget.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchCallvirt<ProjectileTargetComponent>("get_target")
                     ))
                {
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, EntityStates.Engi.Mine.WaitForTarget, bool>>((hasTarget, self) =>
                    {
                        return hasTarget && ((self.armingStateMachine.state.GetType() == typeof(EntityStates.Engi.Mine.MineArmingFull)));
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Engi PressureMines IL Hook failed");
                }
            };
        }
    }
}
