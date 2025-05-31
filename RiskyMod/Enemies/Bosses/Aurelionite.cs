using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2.CharacterAI;
using RoR2.Skills;

namespace RiskyMod.Enemies.Bosses
{
    public class Aurelionite
    {
        public static bool modifyStats = true;
        public static bool enabled = true;

        public Aurelionite()
        {
            if (modifyStats)
            {
                GameObject bodyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanGoldBody.prefab").WaitForCompletion();
                CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
                cb.baseDamage = 60f;
                cb.levelDamage = cb.baseDamage * 0.2f;
            }
            if (Aurelionite.enabled)
            {
                ModifyAI();
            }
        }

        private void ModifyAI()
        {
            GameObject masterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanGoldMaster.prefab").WaitForCompletion();

            BaseAI ba = masterObject.GetComponent<BaseAI>();

            AISkillDriver[] aiDrivers = masterObject.GetComponents<AISkillDriver>();
            foreach (AISkillDriver ai in aiDrivers)
            {
                if (ai.skillSlot == SkillSlot.Special)
                {
                    //ai.minDistance = 0f;
                    ai.maxDistance = 200f;
                    ai.aimType = AISkillDriver.AimType.AtCurrentEnemy;
                    //ai.driverUpdateTimerOverride = 10f;  //laser firing = 8s, laser chargeup = 2s
                }
            }
        }
    }
}
