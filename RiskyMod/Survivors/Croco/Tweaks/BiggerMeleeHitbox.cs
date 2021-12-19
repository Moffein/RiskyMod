using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Croco
{
    //Taken from https://github.com/TheTimeSweeper/WagaTamashiiWaMadaMoeteOru/blob/master/AcridHitboxBuff/AcridHitboxMod.cs
    public class BiggerMeleeHitbox
    {
        public static bool enabled = true;
        public BiggerMeleeHitbox()
        {
            if (!enabled) return;
            CharacterBody cb = RoR2Content.Survivors.Croco.bodyPrefab.GetComponent<CharacterBody>();
            HitBoxGroup hbg = cb.GetComponentInChildren<HitBoxGroup>();
            if (hbg.groupName == "Slash")
            {
                Transform hitboxTransform = hbg.hitBoxes[0].transform;
                //Default: (34.8, 27.0, 34.4)
                hitboxTransform.localScale = new Vector3(34.8f, 27f, 45f);  //z is up/down
                hitboxTransform.localPosition = new Vector3(0f, 11f, 17.8f);    //y is up/down
                //hitboxTransform.localPosition += new Vector3(0f, 0f, 1f);
            }
        }
    }
}
