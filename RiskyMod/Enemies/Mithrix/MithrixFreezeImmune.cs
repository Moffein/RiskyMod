using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;

namespace RiskyMod.Enemies.Mithrix
{
    public class MithrixFreezeImmune
    {
        public static bool enabled = false;

        public MithrixFreezeImmune()
        {
            if (!enabled) return;

            GameObject brotherObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherBody.prefab").WaitForCompletion();
            SetStateOnHurt ssoh = brotherObject.GetComponent<SetStateOnHurt>();
            ssoh.canBeFrozen = false;

            //Intentionally leaving BrotherHurt as freezable.
        }
    }
}
