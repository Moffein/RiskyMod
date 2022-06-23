using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2
{
    public class BanditFireModes
    {
        public static bool enabled = false;
        public enum Bandit2FireMode { Default, Spam }
        public static Bandit2FireMode currentfireMode = Bandit2FireMode.Default;
        public static KeyCode defaultButton = KeyCode.None;
        public static KeyCode spamButton = KeyCode.None;

        public BanditFireModes()
        {
            if (!enabled || !(Bandit2Core.enabled && (Bandit2Core.burstChanges || Bandit2Core.blastChanges))) return;

            RiskyMod.FireModeActions += FireMode;

            On.RoR2.UI.SkillIcon.Update += (orig, self) =>
            {
                orig(self);
                if (self.targetSkill && self.targetSkillSlot == SkillSlot.Primary)
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
            if ((FireSelect.scrollSelection && scroll != 0) || (Input.GetKeyDown(FireSelect.swapButton) || Input.GetKeyDown(FireSelect.prevButton)))
            {
                CycleFireMode();
            }
            if (Input.GetKeyDown(defaultButton))
            {
                currentfireMode = Bandit2FireMode.Default;
            }
            if (Input.GetKeyDown(spamButton))
            {
                currentfireMode = Bandit2FireMode.Spam;
            }
        }
    }
}
