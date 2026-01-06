using RoR2;

namespace RiskyMod.SharedHooks
{
    internal class LanguageModifiers
    {
        internal delegate void ModifyLanguageToken(LanguageModifier langMod);
        internal static ModifyLanguageToken ModifyLanguageTokenActions;

        internal static string Language_GetLocalizedStringByToken(On.RoR2.Language.orig_GetLocalizedStringByToken orig, Language self, string token)
        {
            string local = orig(self, token);
            if (ModifyLanguageTokenActions != null)
            {
                var langMod = new LanguageModifier
                {
                    token = token,
                    local = local
                };
                ModifyLanguageTokenActions(langMod);
                local = langMod.local;
            }
            return local;
        }

        internal class LanguageModifier
        {
            public string token;
            public string local;
        }
    }
}
