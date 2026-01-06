using MonoMod.Cil;
using RiskyMod.SharedHooks;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.DLC3.Food
{
    public class QuickFix
    {
        public static bool enabled = true;

        public QuickFix()
        {
            if (!enabled) return;
            SharedHooks.LanguageModifiers.ModifyLanguageTokenActions += ModifyLang;
            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(MonoMod.Cil.ILContext il)
        {
            bool error = true;
            int loc = -1;
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC3Content.Items), "BonusHealthBoost"),
                x => x.MatchCallvirt<Inventory>("GetItemCountEffective"),
                x => x.MatchStloc(out loc)))
            {
                if (c.TryGotoNext(x => x.MatchLdcR4(0.15f), x => x.MatchLdloc(loc)))
                {
                    c.Index++;
                    c.EmitDelegate<Func<float, float>>(orig => 0.25f);
                    error = false;
                }
            }

            if (error)
            {
                Debug.LogError("RiskyMod: Quick Fix IL hook failed.");
            }
        }

        private void ModifyLang(LanguageModifiers.LanguageModifier langMod)
        {
            if (langMod.token == "ITEM_BONUSHEALTHBOOST_DESC")
            {
                langMod.local = langMod.local.Replace("15", "25");
            }
        }
    }
}
