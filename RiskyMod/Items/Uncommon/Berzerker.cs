using UnityEngine;
using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace RiskyMod.Items.Uncommon
{
    public class Berzerker
    {
        public static bool enabled = true;
        public static BuffDef berzerkBuff;
        public Berzerker()
        {
            if (!enabled) return;

            LanguageAPI.Add("ITEM_WARCRYONMULTIKILL_PICKUP", "Enter a frenzy after killing enemies.");
            LanguageAPI.Add("ITEM_WARCRYONMULTIKILL_DESC", "<style=cIsDamage>Killing enemies</style> sends you into a <style=cIsDamage>frenzy</style> for <style=cIsDamage>6s</style>. Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>10%</style> and <style=cIsDamage>attack speed</style> by <style=cIsDamage>20%</style> for each enemy killed. Stacks up to 5 <style=cStack>(+3 per stack)</style> times.");

            //Remove Vanilla Effect
            IL.RoR2.CharacterBody.AddMultiKill+= (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "WarCryOnMultiKill")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
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
        }
    }
}
