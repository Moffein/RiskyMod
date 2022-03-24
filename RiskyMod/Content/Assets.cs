using UnityEngine;
using System.IO;
using System.Reflection;
using RoR2;
using System.Linq;

namespace RiskyMod.Content
{
    public class Assets //Meant to be used to hold assetbundle shit... this mod does not use that, though, but still
    {
        public static AssetBundle assetBundle;
        internal static string languageRoot => System.IO.Path.Combine(Assets.assemblyDir, "language");

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(RiskyMod.pluginInfo.Location);
            }
        }

        public static void Init()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RiskyMod.resources"))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }
            LoadSkillIcons();
            LoadBuffIcons();
            LoadLanguage();
        }

        private static void LoadLanguage()
        {
            On.RoR2.Language.SetFolders += fixme;
        }

        //Credits to Anreol for this code
        private static void fixme(On.RoR2.Language.orig_SetFolders orig, Language self, System.Collections.Generic.IEnumerable<string> newFolders)
        {
            if (System.IO.Directory.Exists(Assets.languageRoot))
            {
                var dirs = System.IO.Directory.EnumerateDirectories(System.IO.Path.Combine(Assets.languageRoot), self.name);
                orig(self, newFolders.Union(dirs));
                return;
            }
            orig(self, newFolders);
        }

        private static void LoadBuffIcons()
        {
            BuffIcons.Infusion = assetBundle.LoadAsset<Sprite>("BuffInfusion");
            BuffIcons.Freeze = assetBundle.LoadAsset<Sprite>("BuffFreeze");
            BuffIcons.RaincoatActive = assetBundle.LoadAsset<Sprite>("BuffRaincoatActive");
            BuffIcons.RaincoatReady = assetBundle.LoadAsset<Sprite>("BuffRaincoatReady");
            BuffIcons.RaincoatCooldown = assetBundle.LoadAsset<Sprite>("BuffRaincoatCooldown");
            BuffIcons.HeadstomperActive = assetBundle.LoadAsset<Sprite>("BuffHeadstomperActive");
            BuffIcons.HeadstomperCooldown = assetBundle.LoadAsset<Sprite>("BuffHeadstomperCooldown");
        }

        private static void LoadSkillIcons()
        {
            SkillIcons.Bandit2Gunslinger = assetBundle.LoadAsset<Sprite>("Bandit2Gunslinger");
            SkillIcons.Bandit2Desperado = assetBundle.LoadAsset<Sprite>("Bandit2Desperado");
        }

        public static class BuffIcons
        {
            public static Sprite Infusion;
            public static Sprite Freeze;
            public static Sprite RaincoatReady;
            public static Sprite RaincoatActive;
            public static Sprite RaincoatCooldown;
            public static Sprite HeadstomperActive;
            public static Sprite HeadstomperCooldown;
        }

        public static class SkillIcons
        {
            public static Sprite Bandit2Gunslinger;
            public static Sprite Bandit2Desperado;
        }
    }
}
