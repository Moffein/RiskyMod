using EntityStates;
using R2API;
using RoR2;
using RoR2.Skills;

namespace RiskyMod.Survivors.Commando
{
    public class CommandoCore
    {
        public static bool enabled = true;
        public static bool enablePrimarySkillChanges = true;
        public static bool enableSecondarySkillChanges = true;
        public static bool enableUtilitySkillChanges = true;
        public static bool enableSpecialSkillChanges = true;
        public CommandoCore()
        {
            if (!enabled) return;
        }
    }

    public class Skills
    {

    }
}
