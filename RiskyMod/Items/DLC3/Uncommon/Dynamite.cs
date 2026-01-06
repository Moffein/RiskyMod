using MonoMod.Cil;
using RiskyMod.SharedHooks;
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

        private void ModifyLang(LanguageModifiers.LanguageModifier langMod)
        {
            if (langMod.token == "ITEM_DRONESDROPDYNAMITE_DESC")
            {
                langMod.local = langMod.local.Replace("85", "120");
            }
        }
    }
}
