using RoR2;

namespace RiskyMod.Tweaks
{
    public class DistantRoostCredits
    {
        public static bool enabled = true;
        public DistantRoostCredits()
        {
            if (!enabled) return;
            On.RoR2.SceneDirector.Start += (orig, self) =>
            {
                SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                if (sd && sd.baseSceneName.Equals("blackbeach"))
                {
                    ClassicStageInfo csi = SceneInfo.instance.GetComponent<ClassicStageInfo>();
                    if (csi)
                    {
                        csi.sceneDirectorInteractibleCredits = 220;
                    }
                }
                orig(self);
            };
        }
    }
}
