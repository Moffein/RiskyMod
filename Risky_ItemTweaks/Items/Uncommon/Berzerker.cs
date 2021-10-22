using UnityEngine;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace Risky_ItemTweaks.Items.Uncommon
{
    public class Berzerker
    {
        public static bool enabled = true;
        public static BuffDef berzerkBuff;
        public static void Modify()
        {
            if (!enabled) return;

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.AddMultiKill+= (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "WarCryOnMultiKill")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            //On-kill logic handled in SharedHooks.OnCharacterDeath

            //Buff logic handled in SharedHooks.GetStatCoefficients
            berzerkBuff = ScriptableObject.CreateInstance<BuffDef>();
            berzerkBuff.buffColor = new Color(210f / 255f, 50f / 255f, 22f / 255f);
            berzerkBuff.canStack = true;
            berzerkBuff.isDebuff = false;
            berzerkBuff.name = "RiskyItemTweaks_BerzerkBuff";
            berzerkBuff.iconSprite = RoR2Content.Buffs.WarCryBuff.iconSprite;
            BuffAPI.Add(new CustomBuff(berzerkBuff));

            //Display visual when using the custom berzerkBuff
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
                    return hasWarCry || self.HasBuff(Berzerker.berzerkBuff);
                });
            };
        }
    }
}
