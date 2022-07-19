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

        public Aurelionite()
        {
            if (modifyStats)
            {
                GameObject bodyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanGoldBody.prefab").WaitForCompletion();
                CharacterBody cb = bodyObject.GetComponent<CharacterBody>();
                cb.baseDamage = 50f;
                cb.levelDamage = cb.baseDamage * 0.2f;
            }
            if (Titan.enabled)
            {
                LaserRework();
                ModifyAI();
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Titan/EntityStates.TitanMonster.FireGoldFist.asset", "exitDuration", "1.5"); //Vanilla 3
            }
        }

        private void LaserRework()
        {
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Titan/EntityStates.TitanMonster.FireGoldMegaLaser.asset", "damageCoefficient", "1.6");    //Vanilla 1
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Titan/EntityStates.TitanMonster.FireGoldMegaLaser.asset", "fireFrequency", "10");    //Vanilla 8
            Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Titan/ChargeGoldLaser.asset").WaitForCompletion().baseRechargeInterval = 15f;   //Vanilla 20
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Titan/EntityStates.TitanMonster.ChargeGoldMegaLaser.asset", "baseDuration", "2"); //Vanilla 3
        }

        private void ModifyAI()
        {
            GameObject masterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Titan/TitanGoldMaster.prefab").WaitForCompletion();

            BaseAI ba = masterObject.GetComponent<BaseAI>();
            ba.aimVectorMaxSpeed = 60f; //Vanilla 180

            AISkillDriver[] aiDrivers = masterObject.GetComponents<AISkillDriver>();
            foreach (AISkillDriver ai in aiDrivers)
            {
                if (ai.skillSlot == SkillSlot.Special)
                {
                    ai.minDistance = 0f;
                    ai.maxDistance = 200f;
                    ai.aimType = AISkillDriver.AimType.AtMoveTarget;    //See if this makes it smoother
                    ai.driverUpdateTimerOverride = 10f;  //laser firing = 8s, laser chargeup = 2s
                }
            }
        }
    }
}
