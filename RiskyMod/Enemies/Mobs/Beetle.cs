using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.CharacterAI;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Mobs
{
    public class Beetle
    {
        public static bool enabled = true;
        public Beetle()
        {
            if (!enabled) return;
            GameObject beetleObject = LegacyResourcesAPI.Load<GameObject>("prefabs/characterbodies/beetlebody");
            ModifyStats(beetleObject);
            ModifyAttack();
            ExpandHitbox(beetleObject);

            //SneedUtils.SneedUtils.DumpEntityStateConfig(Addressables.LoadAssetAsync<EntityStateConfiguration>("RoR2/Base/Beetle/EntityStates.BeetleMonster.SpawnState.asset").WaitForCompletion());
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Beetle/EntityStates.BeetleMonster.SpawnState.asset", "duration", "3.2");  //5 vanilla
        }

        private void ModifyStats(GameObject go)
        {
            CharacterBody cb = go.GetComponent<CharacterBody>();

            //cb.baseMaxHealth = 96f;
            //cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            float newSpeed = 8f;   //Vanilla 6
            if (cb.baseMoveSpeed < newSpeed)    //Check in case SpeedyBeetles is installed
            {
                cb.baseMoveSpeed = newSpeed;
            }
        }

        private void ModifyAttack()
        {
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.BeetleMonster.HeadbuttState", "baseDuration", "1.2");   //Vanilla 1.5

            //Increase AI Headbutt range
            GameObject masterObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleMaster.prefab").WaitForCompletion();

            float dist = 5f;
            AISkillDriver[] skills = masterObject.GetComponents<AISkillDriver>();
            foreach(AISkillDriver ai in skills)
            {
                if (ai.skillSlot == SkillSlot.Primary)
                {
                    if (ai.maxDistance < dist) ai.maxDistance = dist;
                }
            }
        }

        private void ExpandHitbox(GameObject enemyObject)
        {
            CharacterBody cb = enemyObject.GetComponent<CharacterBody>();
            HitBoxGroup hbg = cb.GetComponentInChildren<HitBoxGroup>();
            if (hbg.groupName == "Headbutt")
            {
                Transform hitboxTransform = hbg.hitBoxes[0].transform;
                //Debug.Log("Beetle Hitbox: " + hitboxTransform.localScale);    //(1.0, 1.0, 1.7)
                hitboxTransform.localScale = new Vector3(2.5f, 4f, 3.4f);

                //Debug.Log("Beetle Hitbox Pos: " + hitboxTransform.localPosition);   //(0.0, 0.3, 0.2)
                hitboxTransform.localPosition = new Vector3(0f, 0.3f, 0.2f);    //y is forward
            }
        }
    }
}
