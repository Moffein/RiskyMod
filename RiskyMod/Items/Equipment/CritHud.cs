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
                HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, RoR2Content.Equipment.CritHud);
            };
            //Disable vanilla behavior
            IL.RoR2.CharacterBody.RecalculateStats += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                         x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "FullCrit")
                        );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyBuffDef));
            };

            //Todo - move to recalculatestatsapi once critdamagemult is fixed
            On.RoR2.CharacterBody.RecalculateStats += (orig, self) =>
            {
                orig(self);

                if (self.HasBuff(RoR2Content.Buffs.FullCrit))
                {
                    float crit = 100f;
                    float diff = 100f - self.crit;
                    if (diff > 0f)
                    {
                        crit -= diff;
                        self.crit += diff;
                    }
                    if (crit > 0f)
                    {
                        self.critMultiplier += crit * 0.01f;
                    }
                }
            };
        }
    }
}
