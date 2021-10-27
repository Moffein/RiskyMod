using RoR2;
using R2API;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace RiskyMod.Items.Common
{
    public class Crowbar
    {
        public static bool enabled = true;
        public static DamageAPI.ModdedDamageType crowbarDamage;
        public Crowbar()
        {
            if (!enabled) return;
            crowbarDamage = DamageAPI.ReserveDamageType();
            //Remove vanilla effect
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "Crowbar")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "FireRing")
                    );
                c.GotoNext(
                     x => x.MatchLdcR4(4f)
                    );
                c.Index++;
                c.Emit(OpCodes.Ldarg_1);//damageinfo
                c.EmitDelegate<Func<float, DamageInfo, float>>((ringThreshold, damageInfo) =>
                {
                    if (DamageAPI.HasModdedDamageType(damageInfo, crowbarDamage))
                    {
                        if (damageInfo.attacker)
                        {
                            CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                            if (attackerBody)
                            {
                                Inventory inv = attackerBody.inventory;
                                if (inv)
                                {
                                    int crowbarCount = inv.GetItemCount(RoR2Content.Items.Crowbar);
                                    if (crowbarCount > 0)
                                    {
                                        ringThreshold *= GetCrowbarMult(crowbarCount);  //Scale up ring activation threshold to match the crowbar damage bonus.
                                    }
                                }
                            }
                        }
                    }
                    return ringThreshold;
                });
            };

            LanguageAPI.Add("ITEM_CROWBAR_DESC", "Deal <style=cIsDamage>+50%</style> <style=cStack>(+50% per stack)</style> damage to enemies above <style=cIsDamage>90% health</style>.");

            //Effect handled in SharedHooks.TakeDamage
        }

        //This is used in multiple places, so it is a static method to make sure calculations are consistent.
        public static float GetCrowbarMult(int crowbarCount)
        {
            return 1f + 0.5f * crowbarCount;
        }
    }
}
