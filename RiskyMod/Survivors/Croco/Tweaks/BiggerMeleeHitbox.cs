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
            CharacterBody cb = CrocoCore.bodyPrefab.GetComponent<CharacterBody>();
            HitBoxGroup hbg = cb.GetComponentInChildren<HitBoxGroup>();
            if (hbg.groupName == "Slash")
            {
                Transform hitboxTransform = hbg.hitBoxes[0].transform;
                //Default: (34.8, 27.0, 34.4)
                hitboxTransform.localScale = new Vector3(40f, 40f, 45f);  //z is up/down

                //Defualt: (0.0, 13.0, 17.8)
                hitboxTransform.localPosition = new Vector3(0f, 11f, 15f);    //y is up/down
            }
        }
    }
}
