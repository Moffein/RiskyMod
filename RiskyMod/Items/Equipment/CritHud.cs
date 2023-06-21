using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using UnityEngine;

namespace RiskyMod.Items.Equipment
{
    public class CritHud
    {
        public static bool enabled = true;
        public CritHud()
        {
            if (!enabled) return;
            On.RoR2.EquipmentCatalog.Init += (orig) =>
            {
                orig();
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, RoR2Content.Equipment.CritOnUse);
            };
            //Disable vanilla behavior
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                         x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "FullCrit")
                        ))
                {
                    c.Remove();
                    c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyBuffDef));
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: CritHud IL Hook failed");
                }
            };

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(RoR2Content.Buffs.FullCrit))
            {
                args.critAdd += 100f;

                float critWithoutBuff = sender.crit - 100f;
                if (critWithoutBuff > 0f) args.critDamageMultAdd += critWithoutBuff * 0.01f;
            }
        }
    }
}
