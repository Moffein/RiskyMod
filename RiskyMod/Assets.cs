using UnityEngine;
using System.IO;
using System.Reflection;
using RoR2;

namespace RiskyMod
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
            RoR2.RoR2Application.onLoad += (delegate ()
            {
                if (Directory.Exists(Assets.languageRoot))
                {
                    FixLanguageFolders(Assets.languageRoot);
                }
            });
        }

        //Credits to Anreol for this code
        public static void FixLanguageFolders(string rootFolder)
        {
            var allLanguageFolders = Directory.EnumerateDirectories(rootFolder);
            foreach (Language language in Language.GetAllLanguages())
            {
                foreach (var folder in allLanguageFolders)
                {
                    if (folder.Contains(language.name))
                    {
                        HG.ArrayUtils.ArrayAppend<string>(ref language.folders, folder);
                    }
                }
            }
            //Reload all folders, by this time, the language has already been initialized, thats why we are doing this.
            Language.currentLanguage.UnloadStrings();
            Language.currentLanguage.LoadStrings();
            Language.english.UnloadStrings();
            Language.english.LoadStrings();
        }
        private static void LoadBuffIcons()
        {
            BuffIcons.Infusion = assetBundle.LoadAsset<Sprite>("BuffInfusion");
            BuffIcons.Freeze = assetBundle.LoadAsset<Sprite>("BuffFreeze");
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
        }

        public static class SkillIcons
        {
            public static Sprite Bandit2Gunslinger;
            public static Sprite Bandit2Desperado;
        }
    }
}
