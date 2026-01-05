using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.DLC3.Uncommon
{
    public class KineticDampener
    {
        public static bool enabled;

        public KineticDampener()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            IL.RoR2.HealthComponent.GetShieldBoosterDamage += HealthComponent_GetShieldBoosterDamage;
            IL.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
        }

        private void HealthComponent_TakeDamageProcess(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCall<HealthComponent>("GetShieldBoosterDamage")) && c.TryGotoNext(x => x.MatchCallvirt<BlastAttack>("Fire")))
            {
                c.EmitDelegate<Func<BlastAttack, BlastAttack>>(blast =>
                {
                    blast.radius = 20f;
                    return blast;
                });
            }
            else
            {
                Debug.LogError("RiskyMod: KineticDampener TakeDamageProcess IL hook failed.");
            }
        }

        private void HealthComponent_GetShieldBoosterDamage(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(0.1f)))
            {
                c.EmitDelegate<Func<float, float>>(orig => 0.5f);
            }
            else
            {
                Debug.LogError("RiskyMod: KineticDampener GetShieldBoosterDamage IL hook failed.");
            }
        }

        private void CharacterBody_RecalculateStats(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool error = true;

            int loc = -1;

            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC3Content.Items), "ShieldBooster"),
                x => x.MatchCallvirt<Inventory>("GetItemCountEffective"),
                x => x.MatchStloc(out loc)))
            {
                if (c.TryGotoNext(x => x.MatchLdloc(loc)) && c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(0.04f)))
                {
                    c.EmitDelegate<Func<float, float>>(orig => 0.1f);
                    error = false;
                }
            }

            if (error)
            {
                Debug.LogError("RiskyMod: KineticDampener RecalculateStats IL hook failed.");
            }
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC3Content.Items.ShieldBooster);
        }
    }
}
