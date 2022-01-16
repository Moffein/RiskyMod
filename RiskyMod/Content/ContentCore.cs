
using RiskyMod.Content.Enemies;
using RoR2.ContentManagement;

namespace RiskyMod.Content
{
    public class ContentCore
    {
        public static bool initialized = false;
        public static void Init()
        {
            if (initialized) return;
            ContentManager.collectContentPackProviders += ContentCore.ContentManager_collectContentPackProviders;

            new LunarGolemSkyMeadow();
        }

        private static void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(new Content());
        }
    }
}
