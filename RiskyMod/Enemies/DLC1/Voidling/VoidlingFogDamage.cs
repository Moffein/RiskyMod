using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using RoR2;

namespace RiskyMod.Enemies.DLC1.Voidling
{
    public class VoidlingFogDamage
    {
        public static bool enabled = true;
        private SceneDef voidRaidScene = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/voidraid/voidraid.asset").WaitForCompletion();
        public VoidlingFogDamage()
        {
            if (!enabled) return;

            //Bruteforce way of doing this
            On.RoR2.FogDamageController.Start += (orig, self) =>
            {
                orig(self);
                if (SceneCatalog.GetSceneDefForCurrentScene() == voidRaidScene)
                {
                    /*Debug.Log("HP% per second: " + self.healthFractionPerSecond);   //0.01
                    Debug.Log("HP% Ramping per second: " + self.healthFractionRampCoefficientPerSecond + "\n");    //0.1*/

                    self.healthFractionPerSecond = 0.025f;
                    self.healthFractionRampCoefficientPerSecond = 0f;
                }
            };
        }
    }
}
