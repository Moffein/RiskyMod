using R2API;
using RoR2;

namespace RiskyMod.Items.Common
{
    public class Warbanner
    {
        public static bool enabled = true;
        public Warbanner()
        {
            if (!enabled) return;

            On.RoR2.HealthComponent.Heal += (orig, self, amount, procChainMask, nonRegen) =>
            {
                if (self.body && self.body.HasBuff(RoR2Content.Buffs.Warbanner.buffIndex))
                {
                    amount *= 1.3f;
                }
                return orig(self, amount, procChainMask, nonRegen);
            };

            //Changes handled in SharedHooks.GetStatCoefficients
            LanguageAPI.Add("ITEM_WARDONLEVEL_PICKUP", "Drop a Warbanner on level up or starting the Teleporter event. Grants allies armor, damage, attack speed, movement speed, and bonus healing.");
            LanguageAPI.Add("ITEM_WARDONLEVEL_DESC", "On <style=cIsUtility>level up</style> or starting the <style=cIsUtility>Teleporter event</style>, drop a banner that strengthens all allies within <style=cIsUtility>16m</style> <style=cStack>(+8m per stack)</style>. Raise <style=cIsHealing>armor</style> by <style=cIsHealing>15</style>, raise <style=cIsDamage>damage</style> by <style=cIsDamage>15%</style>, and raise <style=cIsDamage>attack speed</style>, <style=cIsUtility>movement speed</style>, and <style=cIsHealing>healing</style> by <style=cIsDamage>30%</style>.");
        }
    }
}
