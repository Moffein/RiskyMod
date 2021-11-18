using MonoMod.RuntimeDetour;
using R2API.Utils;
using RoR2.Skills;

namespace RiskyMod.Survivors.Captain

{
    //Based on https://thunderstore.io/package/PlasmaCore3/AlwaysAllowOrbitalSkills/
    public class CaptainOrbitalHiddenRealms
    {
        public static bool enabled = true;
        public CaptainOrbitalHiddenRealms()
        {
            if (!enabled) return;

            var getParticipatingPlayerCount = new Hook(typeof(CaptainOrbitalSkillDef).GetMethodCached("get_isAvailable"),
                typeof(CaptainOrbitalHiddenRealms).GetMethodCached(nameof(IsAvailable)));
        }
        private static bool IsAvailable(CaptainOrbitalSkillDef self)
        {
            return true;
        }
    }
}
