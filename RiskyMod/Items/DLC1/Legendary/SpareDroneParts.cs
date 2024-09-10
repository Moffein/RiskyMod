using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Legendary
{
    public class SpareDroneParts
    {
        public static bool ignoreAllyCap = true;
        public static bool enabled = true;

        public SpareDroneParts()
        {
            if (ignoreAllyCap)
            {
                IL.RoR2.DroneWeaponsBehavior.TrySpawnDrone += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(
                         x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                        ))
                    {
                        c.EmitDelegate<Func<DirectorSpawnRequest, DirectorSpawnRequest>>(spawnRequest =>
                        {
                            spawnRequest.ignoreTeamMemberLimit = true;
                            return spawnRequest;
                        });
                    }
                    else
                    {
                        Debug.LogError("RiskyMod: SpareDroneParts DroneWeaponsBehavior.TrySpawnDrone IL Hook failed");
                    }
                };
            }

            if (enabled)
            {
                Items.ItemsCore.ModifyItemDefActions += ModifyDroneParts;
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.DroneWeaponsChainGun.FireChainGun", "damageCoefficient", "0.5");   //1
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.DroneWeaponsChainGun.FireChainGun", "damageCoefficientPerBounce", "0.5");
                SneedUtils.SneedUtils.SetEntityStateField("EntityStates.DroneWeaponsChainGun.FireChainGun", "additionalBounces", "1");

                IL.RoR2.DroneWeaponsBoostBehavior.OnEnemyHit += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(MoveType.After,
                         x => x.MatchCallvirt<CharacterBody>("get_damage")
                        ))
                    {
                        c.Emit(OpCodes.Ldarg_1);    //DamageInfo
                        c.EmitDelegate<Func<float, DamageInfo, float>>((bodyDamage, damageInfo) => damageInfo.damage);
                    }
                    else
                    {
                        Debug.LogError("RiskyMod: SpareDroneParts DroneWeaponsBoostBehavior.OnEnemyHit IL Hook failed");
                    }
                };
            }
        }

        private static void ModifyDroneParts()
        {
            HG.ArrayUtils.ArrayAppend(ref Items.ItemsCore.changedItemDescs, DLC1Content.Items.DroneWeapons);
        }

    }
}
