using RoR2;
using R2API;
using UnityEngine;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace RiskyMod.Items.Legendary
{
    public class Headhunter
    {
        public static bool enabled = true;
        public static BuffDef headhunterBuff;
        public Headhunter()
        {
            if (!enabled) return;
            LanguageAPI.Add("ITEM_HEADHUNTER_PICKUP", "Deal extra damage to elites, and temporarily gain their power on kill.");
            LanguageAPI.Add("ITEM_HEADHUNTER_DESC", "Deal an additional <style=cIsDamage>30%</style> damage to elite monsters. Upon killing an elite, temporarily gain its power and increase <style=cIsUtility>movement speed</style> and <style=cIsDamage>damage</style> by <style=cIsDamage>30%</style> for <style=cIsDamage>10s</style> <style=cStack>(+5s per stack)</style>.");
            
            //Elite damage bonus is handled in SharedHooks.ModifyFinalDamage

            //Buff logic handled in SharedHooks.GetStatCoefficients
            headhunterBuff = ScriptableObject.CreateInstance<BuffDef>();
            headhunterBuff.buffColor = new Color(210f/255f, 50f/255f, 22f/255f);
            headhunterBuff.canStack = false;
            headhunterBuff.isDebuff = false;
            headhunterBuff.name = "RiskyItemTweaks_HeadhunterBuff";
            headhunterBuff.iconSprite = Resources.Load<Sprite>("textures/bufficons/texBuffAttackSpeedOnCritIcon");
            BuffAPI.Add(new CustomBuff(headhunterBuff));

            //Buff application handled in SharedHooks.OnCharacterDeath

            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "HeadHunter")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };
        }
    }
}
