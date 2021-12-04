using UnityEngine;
using System.IO;
using System.Reflection;

namespace RiskyMod
{
    public class Assets //Meant to be used to hold assetbundle shit... this mod does not use that, though, but still
    {
        public static AssetBundle assetBundle;
        internal static string assemblyDir
        {
            get
            {
                return Path.GetDirectoryName(RiskyMod.pluginInfo.Location);
            }
        }

        public static void Init()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RiskyMod.resources"))
            {
                assetBundle = AssetBundle.LoadFromStream(stream);
            }
            LoadSkillIcons();
        }

        private static void LoadSkillIcons()
        {
            SkillIcons.Bandit2Gunslinger = assetBundle.LoadAsset<Sprite>("Bandit2Gunslinger");
            SkillIcons.Bandit2Desperado = assetBundle.LoadAsset<Sprite>("Bandit2Desperado");
        }

        public static class SkillIcons
        {
            public static Sprite Bandit2Gunslinger;
            public static Sprite Bandit2Desperado;
        }
    }
}
