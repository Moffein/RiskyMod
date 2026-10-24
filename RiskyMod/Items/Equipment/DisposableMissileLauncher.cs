using MonoMod.Cil;
using RiskyMod.SharedHooks;
using System;
using UnityEngine;

namespace RiskyMod.Items.Equipment
{
    public class DisposableMissileLauncher
    {
        public static bool enabled;

        public DisposableMissileLauncher()
        {
            if (!enabled) return;
            IL.RoR2.EquipmentSlot.FireMissile += EquipmentSlot_FireMissile;
            SharedHooks.LanguageModifiers.ModifyLanguageTokenActions += ModifyLang;
        }

        private void EquipmentSlot_FireMissile(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(3f)))
            {
                c.EmitDelegate<Func<float, float>>(damageCoefficient => 3.8f);
            }
            else
            {
                Debug.LogError("RiskyMod: DisposableMissileLauncher FireMissile IL hook failed.");
            }
        }

        private void ModifyLang(LanguageModifiers.LanguageModifier langMod)
        {
            if (langMod.token == "EQUIPMENT_COMMANDMISSILE_DESC")
            {
                langMod.local = langMod.local.Replace("300", "380");
            }
        }
    }
}
