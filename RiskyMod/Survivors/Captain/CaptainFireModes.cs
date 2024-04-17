using BepInEx.Configuration;
using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Captain
{
    public class CaptainFireModes
    {
        public static ConfigEntry<bool> enabled;

        public static CaptainFireMode currentfireMode = CaptainFireMode.Default;
        public static ConfigEntry<KeyboardShortcut> defaultButton;
        public static ConfigEntry<KeyboardShortcut> autoButton;
        public static ConfigEntry<KeyboardShortcut> chargeButton;
        public enum CaptainFireMode { Default, Auto, Charged }

        public CaptainFireModes()
        {
            if (!(CaptainCore.enabled && CaptainCore.enablePrimarySkillChanges)) return;

            On.RoR2.UI.SkillIcon.Update += (orig, self) =>
            {
                orig(self);
                if (self.targetSkill && self.targetSkillSlot == SkillSlot.Primary)
                {
                    if (enabled.Value)
                    {
                        if (self.targetSkill.characterBody.bodyIndex == CaptainCore.CaptainIndex && self.targetSkill.skillDef.activationState.stateType == typeof(EntityStates.RiskyMod.Captain.ChargeShotgun))
                        {
                            self.stockText.gameObject.SetActive(true);
                            self.stockText.fontSize = 12f;
                            self.stockText.SetText(currentfireMode.ToString());
                        }
                    }
                    /*else
                    {
                        self.stockText.gameObject.SetActive(false);
                    }*/
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
            if (FireSelect.scrollSelection.Value && scroll != 0)
            {
                CycleFireMode(scroll < 0 ? -1 : 1);
            }
            if (SneedUtils.SneedUtils.GetKeyPressed(FireSelect.swapButton))
            {
                CycleFireMode(1);
            }
            if (SneedUtils.SneedUtils.GetKeyPressed(FireSelect.prevButton))
            {
                CycleFireMode(-1);
            }
            if (SneedUtils.SneedUtils.GetKeyPressed(CaptainFireModes.defaultButton))
            {
                currentfireMode = CaptainFireMode.Default;
            }
            if (SneedUtils.SneedUtils.GetKeyPressed(CaptainFireModes.autoButton))
            {
                currentfireMode = CaptainFireMode.Auto;
            }
            if (SneedUtils.SneedUtils.GetKeyPressed(CaptainFireModes.chargeButton))
            {
                currentfireMode = CaptainFireMode.Charged;
            }
        }
    }
}
