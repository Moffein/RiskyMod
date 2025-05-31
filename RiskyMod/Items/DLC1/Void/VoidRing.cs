using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.DLC1.Void
{
    class VoidRing
    {
        public static bool enabled = true;
        public VoidRing()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            GameObject prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/ElementalRingVoid/ElementalRingVoidBlackHole.prefab").WaitForCompletion();
            ProjectileExplosion pe = prefab.GetComponent<ProjectileExplosion>();
            pe.falloffModel = BlastAttack.FalloffModel.None;
            
            if (RiskyMod.disableProcChains)
            {
                pe.blastProcCoefficient = 0f;   //1.0 default
            }

            IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
            {
                bool error = true;

                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "ElementalRingVoidReady")
                    ))
                {
                    //Reduce cooldown
                    if(c.TryGotoNext(
                         x => x.MatchLdcR4(20f)
                        ))
                    {
                        c.Next.Operand = 15f;

                        //Change damage
                        if (c.TryGotoNext(
                             x => x.MatchLdcR4(1f)
                            ))
                        {
                            c.Next.Operand = 1.2f;

                            if (c.TryGotoNext(MoveType.After,
                                 x => x.MatchMul()
                                ))
                            {
                                c.EmitDelegate<Func<float, float>>(damageCoefficient => damageCoefficient + 0.8f);
                                error = false;
                            }
                        }
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: VoidRing IL Hook failed");
                }
            };
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.ElementalRingVoid);
        }
    }
}
