using MonoMod.Cil;
using RoR2;
using R2API;
using Mono.Cecil.Cil;

namespace RiskyMod.Tweaks.CharacterMechanics
{
    public class NerfVoidtouched
    {
        public static bool enabled = true;
        public NerfVoidtouched()
        {
            if (!enabled) return;
            //Remove vanilla on-hit effect
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "EliteVoid")
                    ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyBuffDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: NerfVoidtouched IL Hook failed");
                }
            };

            //Effect handled in TakeDamage

            //Remove damage penalty
            RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender.HasBuff(DLC1Content.Buffs.EliteVoid))
                {
                    args.damageMultAdd += 0.8f; //0.3f + 0.5f, has inherent -0.3f mult
                }
            };

            SharedHooks.TakeDamage.OnDamageTakenAttackerActions += (DamageInfo damageInfo, HealthComponent self, CharacterBody attackerBody) =>
            {
                if (attackerBody.HasBuff(DLC1Content.Buffs.EliteVoid) && damageInfo.procCoefficient > 0f)
                {
                    self.body.AddTimedBuff(RoR2Content.Buffs.NullifyStack.buffIndex, 8f * damageInfo.procCoefficient);
                }
            };
        }
    }
}
