using UnityEngine;

namespace RiskyMod.Survivors.Bandit2.Components
{
    public class DesperadoPersist : MonoBehaviour
    {
        public int stacks = 0;
        private bool addedHook;

        private void Awake()
        {
            addedHook = false;
            if (!DesperadoRework.enabled)
            {
                addedHook = true;
                RoR2.Stage.onServerStageBegin += Stage_onServerStageBegin;
            }
        }

        private void Stage_onServerStageBegin(RoR2.Stage obj)
        {
            stacks = 0;
        }

        private void OnDestroy()
        {
            if (addedHook)
            {
                RoR2.Stage.onServerStageBegin -= Stage_onServerStageBegin;
            }
        }
    }
}
