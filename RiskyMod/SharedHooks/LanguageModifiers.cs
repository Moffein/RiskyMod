using RoR2;

namespace RiskyMod.SharedHooks
{
    internal class LanguageModifiers
    {
        internal delegate string ModifyLanguageToken(string token, string localized);
        internal static ModifyLanguageToken ModifyLanguageTokenActions;

        internal static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token)
        {
            string local = orig(self, token);
            if (ModifyLanguageTokenActions != null) local = ModifyLanguageTokenActions(token, local);
            return local;
        }
    }
}
