using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Items.DLC3.Common
{
    public class EclipseLite
    {
        public static bool enabled = true;
        public EclipseLite()
        {
            if (!enabled) return;

            SharedHooks.LanguageModifiers.ModifyLanguageTokenActions += ModifyLang;
            IL.RoR2.CharacterBody.OnSkillCooldown += CharacterBody_OnSkillCooldown;
        }

        private void CharacterBody_OnSkillCooldown(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchLdsfld(typeof(DLC3Content.Items), "BarrierOnCooldown"))
                && c.TryGotoNext(MoveType.After, x=> x.MatchLdcR4(0.0025f)))
            {
                c.EmitDelegate<Func<float, float>>(orig => 0.005f);
            }
        }

        private string ModifyLang(string token, string localized)
        {
            if (token != "ITEM_BARRIERONCOOLDOWN_DESC") return localized;
            return localized.Replace("25", "5");
        }
    }
}
