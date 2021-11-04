using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskyMod.Items.Legendary;
using RiskyMod.Items.Uncommon;
using RoR2;
using System;

namespace RiskyMod.SharedHooks
{
    public class StealBuffVFX
    {
        public StealBuffVFX()
        {
            if (HeadHunter.enabled || Berzerker.enabled)
            {
                IL.RoR2.CharacterBody.OnClientBuffsChanged += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(
                         x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "WarCryBuff")
                        );
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasWarCry, self) =>
                    {
                        return hasWarCry || (Berzerker.enabled && self.HasBuff(Berzerker.berzerkBuff)) || (HeadHunter.enabled && self.HasBuff(HeadHunter.headhunterBuff));
                    });
                };
            }

            if (HarvesterScythe.enabled)
            {
                IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    c.GotoNext(
                         x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "LifeSteal")
                        );
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasBuff, self) =>
                    {
                        return hasBuff || (HarvesterScythe.enabled && self.HasBuff(HarvesterScythe.scytheBuff));
                    });
                };
            }
        }
    }
}
