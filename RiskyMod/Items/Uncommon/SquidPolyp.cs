using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace RiskyMod.Items.Uncommon
{
    public class SquidPolyp
    {
        public static bool enabled = true;
        public static GameObject procEffectPrefab;

        //Does this need turretblacklist?
        public SquidPolyp()
        {
            if (!enabled) return;

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnInteractionBegin += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Squid")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            //Effect handled in SharedHooks.TakeDamage

            LanguageAPI.Add("ITEM_SQUIDTURRET_PICKUP", "Taking heavy damage summons a Squid Turret nearby.");
            LanguageAPI.Add("ITEM_SQUIDTURRET_DESC", "Chance on taking damage to summon a <style=cIsDamage>Squid Turret</style> that <style=cIsUtility>distracts</style> and attacks nearby enemies at <style=cIsDamage>100% <style=cStack>(+100% per stack)</style> attack speed</style>. Chance increases the more damage you take. Can have up to <style=cIsUtility>2</style> <style=cStack>(+1 per stack)</style> Squids at a time.");

            procEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/claygooorbimpact").InstantiateClone("RiskyItemTweaks_SquidPolypProc", false);
            EffectComponent ec = procEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "Play_treeBot_m2_launch";
            EffectAPI.AddEffect(procEffectPrefab);
        }
    }
}
