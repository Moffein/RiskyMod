using MonoMod.Cil;
using System;
using RoR2;
using RoR2.Orbs;
using Mono.Cecil.Cil;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Void
{
    public class PlasmaShrimp
    {
        public static bool enabled = true;
        public PlasmaShrimp()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Items), "MissileVoid")
                     ))
                {
                    //Scale proc chance off of proc coefficient
                    if (c.TryGotoNext(MoveType.After,
                    x => x.MatchLdfld<HealthComponent>("shield")
                    ))
                    {
                        c.Emit(OpCodes.Ldarg_1);    //damageInfo
                        c.Emit(OpCodes.Ldloc, 6);    //master
                        c.EmitDelegate<Func<float, DamageInfo, CharacterMaster, float>>((origShield, damageInfo, master) =>
                        {
                            if (origShield <= 0f) return origShield;//If shield is <= 0, dont bother rolling since the damage will be rejected anywyas.

                            bool shouldFire = true;
                            if (damageInfo.procCoefficient < 1f)
                            {
                                shouldFire = Util.CheckRoll(100f * damageInfo.procCoefficient, master);
                            }

                            return shouldFire ? origShield : 0f;    //Say that the player has 0 shield to stop the missile from firing.
                        });

                        //Modify damage
                        if (c.TryGotoNext(
                         x => x.MatchLdcR4(0.4f)
                        ))
                        {
                            c.Next.Operand = 0.28f; //Stack damage
                            c.Index += 4;
                            c.EmitDelegate<Func<float, float>>(damage =>
                            {
                                return damage + 0.12f;  //Add to stack damage to get first stack damage
                            });
                            if (RiskyMod.disableProcChains)
                            {
                                if (c.TryGotoNext(
                                     x => x.MatchStfld<GenericDamageOrb>("procCoefficient")
                                    ))
                                {
                                    c.Index--;
                                    c.Next.Operand = 0f;
                                }
                            }
                            error = false;
                        }
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: PlasmaShrimp IL Hook failed");
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.MissileVoid);
        }
    }
}
