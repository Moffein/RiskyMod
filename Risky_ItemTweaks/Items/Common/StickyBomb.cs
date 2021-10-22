using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using R2API;
using RoR2.Projectile;
using UnityEngine;

namespace Risky_ItemTweaks.Items.Common
{
    public class StickyBomb
    {
        public static bool enabled = true;
        public static bool earlyAccessMode = false;

        public static void Modify()
        {
            if (!enabled) return;
            //Remove Vanilla Effect
            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "StickyBomb")
                    );
                c.Remove();
                c.Emit<Risky_ItemTweaks>(OpCodes.Ldsfld, nameof(Risky_ItemTweaks.emptyItemDef));
            };

            //Modify detonation delay
            GameObject sticky = Resources.Load<GameObject>("prefabs/projectiles/stickybomb");
            ProjectileImpactExplosion pie = sticky.GetComponent<ProjectileImpactExplosion>();
            pie.lifetime = 1.2f;

            //Effect handled in SharedHooks.OnHitEnemy
            if (!earlyAccessMode)
            {
                LanguageAPI.Add("ITEM_STICKYBOMB_DESC", "<style=cIsDamage>5%</style> <style=cStack>(+5% per stack)</style> chance on hit to attach a <style=cIsDamage>bomb</style> to an enemy, detonating for <style=cIsDamage>240%</style> TOTAL damage.");
            }
            else
            {
                LanguageAPI.Add("ITEM_STICKYBOMB_DESC", "<style=cIsDamage>5%</style> <style=cStack>(+2.5% per stack)</style> chance on hit to attach a <style=cIsDamage>bomb</style> to an enemy, detonating for <style=cIsDamage>250%</style> <style=cStack>(+125% per stack)</style> TOTAL damage.");
            }
        }
    }
}
