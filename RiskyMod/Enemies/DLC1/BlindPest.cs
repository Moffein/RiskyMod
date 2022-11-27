using RoR2;
using RoR2.CharacterAI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.DLC1
{
    public class BlindPest
    {
        public static bool enabled = true;
        public BlindPest()
        {
            if (!enabled) return;

            GameObject pestObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/FlyingVerminBody.prefab").WaitForCompletion();
            CharacterBody cb = pestObject.GetComponent<CharacterBody>();
            cb.baseMaxHealth = 60f;
            cb.levelMaxHealth = 18f;

            cb.baseDamage = 15f;    //15 default
            cb.levelDamage = 3f;

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.FlyingVermin.Weapon.Spit", "damageCoefficient", "1");//2 default

            GameObject masterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/FlyingVerminMaster.prefab").WaitForCompletion();
            CharacterMaster cm = masterObject.GetComponent<CharacterMaster>();
            AISkillDriver[] skillDrivers = cm.GetComponentsInChildren<AISkillDriver>();
            foreach (AISkillDriver skill in skillDrivers)
            {
                if (skill.skillSlot == SkillSlot.Primary)
                {
                    skill.maxDistance = 50f;    //vanilla 70
                }
            }
        }
    }
}
