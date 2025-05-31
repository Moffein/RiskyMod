using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace RiskyMod.Enemies.Bosses
{
    public class SCU
    {
        public static bool enabled = true;
        public SCU()
        {
            if (!enabled) return;
            ModifyAI();
            ModifyStats();
        }

        private void ModifyAI()
        {
            GameObject enemyMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RoboBallBoss/RoboBallBossMaster.prefab").WaitForCompletion();
            AISkillDriver[] skillDrivers = enemyMaster.GetComponents<AISkillDriver>();

            foreach (AISkillDriver ai in skillDrivers)
            {
                if (ai.skillSlot == SkillSlot.Special && ai.maxUserHealthFraction < 0.4f)
                {
                    ai.maxUserHealthFraction = 0.4f;
                }
            }
        }

        private void ModifyStats()
        {
            GameObject enemyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/RoboBallBoss/RoboBallBossBody.prefab").WaitForCompletion();
            CharacterBody cb = enemyObject.GetComponent<CharacterBody>();

            cb.baseDamage = 25f;    //orig is 15
            cb.levelDamage = cb.baseDamage * 0.2f;
        }
    }
}
