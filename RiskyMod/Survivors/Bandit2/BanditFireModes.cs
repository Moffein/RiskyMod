using BepInEx.Configuration;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class BanditFireModes
    {
        public static ConfigEntry<bool> enabled;
        public enum Bandit2FireMode { Default, Spam }
        public static Bandit2FireMode currentfireMode = Bandit2FireMode.Default;
        public static ConfigEntry<KeyboardShortcut> defaultButton;
        public static ConfigEntry<KeyboardShortcut> spamButton;

        public BanditFireModes()
        {
            if (!(Bandit2Core.enabled && (Bandit2Core.burstChanges || Bandit2Core.blastChanges))) return;

            RiskyMod.FireModeActions += FireMode;

            On.RoR2.UI.SkillIcon.Update += (orig, self) =>
            {
                orig(self);
                if (enabled.Value && self.targetSkill && self.targetSkillSlot == SkillSlot.Primary)
                {
                    if (self.targetSkill.characterBody.bodyIndex == Bandit2Core.Bandit2Index
                    && (self.targetSkill.skillDef.activationState.stateType == typeof(EntityStates.RiskyMod.Bandit2.Primary.FirePrimaryShotgun)
                    || self.targetSkill.skillDef.activationState.stateType == typeof(EntityStates.RiskyMod.Bandit2.Primary.FirePrimaryRifle)))
                    {
                        self.stockText.gameObject.SetActive(true);
                        self.stockText.fontSize = 12f;
                        self.stockText.SetText(currentfireMode.ToString() + "(" + self.targetSkill.stock + ")");
                    }
                }
            };

        }

        public static void CycleFireMode()
        {
            if (currentfireMode == Bandit2FireMode.Default)
            {
                currentfireMode = Bandit2FireMode.Spam;
            }    
            else
            {
                currentfireMode = Bandit2FireMode.Default;
            }
        }
        public void FireMode()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if ((FireSelect.scrollSelection.Value && scroll != 0) || (FireSelect.swapButton.Value.IsDown() || FireSelect.prevButton.Value.IsDown()))
            {
                CycleFireMode();
            }
            if (SneedUtils.SneedUtils.GetKeyPressed(BanditFireModes.defaultButton))
            {
                currentfireMode = Bandit2FireMode.Default;
            }
            if (SneedUtils.SneedUtils.GetKeyPressed(BanditFireModes.spamButton))
            {
                currentfireMode = Bandit2FireMode.Spam;
            }
        }
    }
}
