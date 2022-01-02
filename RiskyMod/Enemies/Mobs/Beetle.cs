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
            GameObject beetleObject = Resources.Load<GameObject>("prefabs/characterbodies/beetlebody");
            ModifyStats(beetleObject);
            ModifyAttack();
        }

        private void ModifyStats(GameObject go)
        {
            CharacterBody cb = go.GetComponent<CharacterBody>();

            cb.baseMaxHealth = 96f;
            cb.levelMaxHealth = cb.baseMaxHealth * 0.3f;
            cb.baseMoveSpeed = 8.4f;
        }

        private void ModifyAttack()
        {
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.BeetleMonster.HeadbuttState", "baseDuration", "1");
        }
    }
}
