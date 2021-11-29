using UnityEngine;
using RoR2;

namespace RiskyMod.Survivors.Bandit2
{
    public class IncreaseKnifeHitboxSize
    {
        public static bool enabled = true;
        public IncreaseKnifeHitboxSize()
        {
            if (!enabled) return;
            CharacterBody cb = RoR2Content.Survivors.Bandit2.bodyPrefab.GetComponent<CharacterBody>();
            HitBoxGroup hbg = cb.GetComponentInChildren<HitBoxGroup>();
            if (hbg.groupName == "SlashBlade")
            {
                Transform hitboxTransform = hbg.hitBoxes[0].transform;
                hitboxTransform.localScale = new Vector3(hitboxTransform.localScale.x, hitboxTransform.localScale.y * 1.4f, hitboxTransform.localScale.z * 1.3f);
                hitboxTransform.localPosition += new Vector3(0f, 0f, 1f);
            }
        }
    }
}
