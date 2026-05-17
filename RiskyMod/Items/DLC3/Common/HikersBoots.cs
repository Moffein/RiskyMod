using RoR2;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using RiskyMod.SharedHooks;

namespace RiskyMod.Items.DLC3.Common
{
    public class HikersBoots
    {
        public static bool enabled = true;

        public HikersBoots()
        {
            if (!enabled) return;
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += GlobalEventManager_ProcessHitEnemy;
            SharedHooks.LanguageModifiers.ModifyLanguageTokenActions += ModifyLang;
        }

        private void GlobalEventManager_ProcessHitEnemy(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            bool error = true;
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC3Content.Buffs), "CritChanceAndDamage")))
            {
                //Disable unconditional stack refresh
                c.EmitDelegate<Func<BuffDef, BuffDef>>(x => null);

                if (c.TryGotoNext(x => x.MatchLdloc(0), x => x.MatchLdsfld(typeof(DLC3Content.Buffs), "CritChanceAndDamage"), x => x.MatchCallvirt<CharacterBody>("GetBuffCount")))
                {
                    //Re-implement stack refresh inside conditional
                    c.Index++;
                    c.EmitDelegate<Func<CharacterBody, CharacterBody>>(body =>
                    {
                        body.SetTimedBuffDurationIfPresent(DLC3Content.Buffs.CritChanceAndDamage, 7f, true);
                        return body;
                    });

                    //Change buff duration
                    if (c.TryGotoNext(MoveType.After, x => x.MatchLdsfld(typeof(DLC3Content.Buffs), "CritChanceAndDamage"), x => x.MatchLdcR4(10f)))
                    {
                        c.EmitDelegate<Func<float, float>>(x => 7f);
                        error = false;
                    }
                }

            }

            if (error)
            {
                Debug.LogError("RiskyMod: HikersBoots IL Hook failed.");
            }
        }

        private void ModifyLang(LanguageModifiers.LanguageModifier langMod)
        {
            if (langMod.token == "ITEM_CRITATLOWERELEVATION_DESC")
            {
                var split = langMod.local.Split("10");
                string modified = split[0];
                for (int i = 1; i < split.Length; i++)
                {
                    if (i + 1 == split.Length)
                    {
                        modified += "7";
                    }
                    else
                    {
                        modified += "10";
                    }
                    modified += split[i];
                }
                langMod.local = modified;
            }
        }
    }
}
