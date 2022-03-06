using RoR2;
using UnityEngine;

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
        }

        private void ModifyStats(GameObject go)
        {
            CharacterBody cb = go.GetComponent<CharacterBody>();

            //cb.baseMaxHealth = 96f;
            //cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            float newSpeed = 8.4f;
            if (cb.baseMoveSpeed < newSpeed)    //Check in case SpeedyBeetles is installed
            {
                cb.baseMoveSpeed = newSpeed;
            }
        }

        private void ModifyAttack()
        {
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.BeetleMonster.HeadbuttState", "baseDuration", "1");
        }
    }
}
