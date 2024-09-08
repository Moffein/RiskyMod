using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Croco.Tweaks
{
    public class BuffFrenziedLeap
    {
        public static bool enabled = true;
        public BuffFrenziedLeap()
        {
            if (!enabled) return;
            Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoChainableLeap.asset").WaitForCompletion().baseRechargeInterval = 6f;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Croco.ChainableLeap", "refundPerHit", "0");
            IL.EntityStates.Croco.ChainableLeap.DoImpactAuthority += ChainableLeap_DoImpactAuthority;
        }

        private void ChainableLeap_DoImpactAuthority(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(
                 x => x.MatchStloc(0)
                ))
            {
                c.Emit(OpCodes.Ldarg_0);    //self
                c.EmitDelegate<Func<BlastAttack.Result, EntityStates.Croco.ChainableLeap, BlastAttack.Result>>((result, self) =>
                {
                    //1s per hit CDR
                    self.skillLocator.primary.RunRecharge((float)result.hitCount);
                    self.skillLocator.secondary.RunRecharge((float)result.hitCount);
                    self.skillLocator.utility.RunRecharge((float)result.hitCount);
                    self.skillLocator.special.RunRecharge((float)result.hitCount);
                    return result;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("RiskyMod: Croco ChainableLeap IL Hook failed");
            }
        }
    }
}
