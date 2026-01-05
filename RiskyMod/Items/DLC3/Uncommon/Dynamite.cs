using MonoMod.Cil;
using System;
using UnityEngine;

namespace RiskyMod.Items.DLC3.Uncommon
{
    public class Dynamite
    {
        public static bool enabled = true;
        public Dynamite()
        {
            if (!enabled) return;

            SharedHooks.LanguageModifiers.ModifyLanguageTokenActions += ModifyLang;
            IL.RoR2.Items.DroneDynamiteBehaviour.FixedUpdate += DroneDynamiteBehaviour_FixedUpdate;
        }

        private void DroneDynamiteBehaviour_FixedUpdate(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(0.85f)))
            {
                c.EmitDelegate<Func<float, float>>(orig => 1.2f);
            }
            else
            {
                Debug.LogError("RiskyMod: Dynamite IL hook failed.");
            }
        }

        private string ModifyLang(string token, string localized)
        {
            if (token != "ITEM_DRONESDROPDYNAMITE_DESC") return localized;
            return localized.Replace("85", "240");
        }
    }
}
