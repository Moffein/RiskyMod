using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Mithrix
{
    public class MithrixFallImmune
    {
        public static bool enabled = true;
        public MithrixFallImmune()
        {
            if (!enabled) return;

            GameObject brotherObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherBody.prefab").WaitForCompletion();
            CharacterBody brotherBody = brotherObject.GetComponent<CharacterBody>();
            brotherBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            GameObject brotherHurtObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherHurtBody.prefab").WaitForCompletion();
            CharacterBody brotherHurtBody = brotherHurtObject.GetComponent<CharacterBody>();
            brotherHurtBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
        }
    }
}
