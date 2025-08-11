using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Mithrix
{
    public class MithrixHealthBuff
    {
        public static bool enabled = true;
        public MithrixHealthBuff()
        {
            if (!enabled) return;

            CharacterBody body = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherBody.prefab").WaitForCompletion().GetComponent<CharacterBody>();
            if (body.baseMaxHealth < 1200f)
            {
                body.baseMaxHealth = 1200f;
                body.levelMaxHealth = body.baseMaxHealth * 0.3f;
            }
        }
    }
}
