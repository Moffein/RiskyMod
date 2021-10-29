using RoR2;
using UnityEngine;
using RiskyMod.Tweaks;

namespace RiskyMod.MonoBehaviours
{
    public class OSPComponent : MonoBehaviour
    {
        public static float ospThreshold = 0.9f;    //Max HP Percent that can trigger OSP
        public static float ospTimer = 0.5f;    //After HP goes below ospThreshold, taking lethal damage within this timeframe will trigger OSP.
        public static float ospInvuln = 0.3f;   //Grace period after triggering OSP

        //These will be set during TakeDamage
        public CharacterBody characterBody = null;
        public HealthComponent healthComponent = null;

        private float ospStopwatch;

        public void Awake()
        {
            ospStopwatch = 0f;
        }

        public void FixedUpdate()
        {
            if (ospStopwatch > 0f)
            {
                ospStopwatch -= Time.fixedDeltaTime;
                if (ospStopwatch <= 0f)
                {
                    Debug.Log("OSP timer expired.");
                }
            }
        }

        public void StartOSPTimer()
        {
            if (ospStopwatch <= 0f && CanTriggerOSP())
            {
                ospStopwatch = ospTimer;
                Debug.Log("Starting OSP timer.");
            }
        }

        public bool TriggerOSP()
        {
            bool ospTriggered = false;
            if (CanTriggerOSP())
            {
                ospTriggered = true;
                ospStopwatch = 0f;
                characterBody.AddBuff(TrueOSP.disableOSP);
                characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, ospInvuln);
                characterBody.outOfDangerStopwatch = 0f;
                Debug.Log("Triggering OSP");
            }
            return ospTriggered;
        }

        public bool CanTriggerOSP()
        {
            return !characterBody.HasBuff(TrueOSP.disableOSP) && (healthComponent.health / healthComponent.fullHealth > ospThreshold || ospStopwatch > 0f);
        }
    }
}
