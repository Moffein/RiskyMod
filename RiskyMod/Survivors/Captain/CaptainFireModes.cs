using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Captain
{
    public class CaptainFireModes
    {
        public static bool enabled = false;

        public static CaptainFireMode currentfireMode = CaptainFireMode.Default;
        public static bool enableFireSelect = false;
        public static KeyCode defaultButton = KeyCode.None;
        public static KeyCode autoButton = KeyCode.None;
        public static KeyCode chargeButton = KeyCode.None;
        public enum CaptainFireMode { Default, Auto, Charged }

        public CaptainFireModes()
        {
            if (!enabled || !(CaptainCore.enabled && CaptainCore.enablePrimarySkillChanges)) return;

            On.RoR2.UI.SkillIcon.Update += (orig, self) =>
            {
                orig(self);
                if (self.targetSkill && self.targetSkillSlot == SkillSlot.Primary)
                {
                    if (self.targetSkill.characterBody.bodyIndex == CaptainCore.CaptainIndex && self.targetSkill.skillDef.activationState.stateType == typeof(EntityStates.RiskyMod.Captain.ChargeShotgun))
                    {
                        self.stockText.gameObject.SetActive(true);
                        self.stockText.fontSize = 12f;
                        self.stockText.SetText(currentfireMode.ToString());
                    }
                }
            };

            RiskyMod.FireModeActions += FireMode;
        }

        public static void CycleFireMode(int direction)
        {
            int newFireMode = direction + (int)currentfireMode;
            if (newFireMode < 0)
            {
                currentfireMode = CaptainFireMode.Charged;
            }
            else if (newFireMode > 2)
            {
                currentfireMode = CaptainFireMode.Default;
            }
            else
            {
                currentfireMode = (CaptainFireMode)newFireMode;
            }
        }

        public void FireMode()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (FireSelect.scrollSelection && scroll != 0)
            {
                CycleFireMode(scroll < 0 ? -1 : 1);
            }
            if (Input.GetKeyDown(FireSelect.swapButton))
            {
                CycleFireMode(1);
            }
            if (Input.GetKeyDown(FireSelect.prevButton))
            {
                CycleFireMode(-1);
            }
            if (Input.GetKeyDown(defaultButton))
            {
                currentfireMode = CaptainFireMode.Default;
            }
            if (Input.GetKeyDown(autoButton))
            {
                currentfireMode = CaptainFireMode.Auto;
            }
            if (Input.GetKeyDown(chargeButton))
            {
                currentfireMode = CaptainFireMode.Charged;
            }
        }
    }
}
