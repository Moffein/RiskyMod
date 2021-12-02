using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Engi
{
    public class EngiFireModes
    {
        public static bool enabled = false;
        public static EngiFireMode currentfireMode = EngiFireMode.Default;
        public static bool enableFireSelect = false;
        public static KeyCode defaultButton = KeyCode.None;
        public static KeyCode autoButton = KeyCode.None;
        public static KeyCode holdButton = KeyCode.None;
        public enum EngiFireMode{ Default, Auto, Hold }

        public EngiFireModes()
        {
            if (!enabled) return;

            On.RoR2.UI.SkillIcon.Update += (orig, self) =>
            {
                orig(self);
                if (self.targetSkill && self.targetSkillSlot == SkillSlot.Primary)
                {
                    if (self.targetSkill.characterBody.bodyIndex == BodyCatalog.FindBodyIndex("EngiBody"))
                    {
                        self.stockText.gameObject.SetActive(true);
                        self.stockText.fontSize = 12f;
                        self.stockText.SetText(currentfireMode.ToString());
                    }
                }
            };

            On.EntityStates.Engi.EngiWeapon.ChargeGrenades.OnEnter += (orig, self) =>
            {
                float oldBaseTotalDuration = EntityStates.Engi.EngiWeapon.ChargeGrenades.baseTotalDuration;
                if (currentfireMode == EngiFireMode.Auto)
                {
                    EntityStates.Engi.EngiWeapon.ChargeGrenades.baseTotalDuration = EntityStates.Engi.EngiWeapon.ChargeGrenades.baseMaxChargeTime;
                }
                else if(currentfireMode == EngiFireMode.Hold)
                {
                    EntityStates.Engi.EngiWeapon.ChargeGrenades.baseTotalDuration = 1000000f;
                }
                orig(self);
                EntityStates.Engi.EngiWeapon.ChargeGrenades.baseTotalDuration = oldBaseTotalDuration;
            };

            RiskyMod.FireModeActions += FireMode;
        }

        public static void CycleFireMode(int direction)
        {
            int newFireMode = direction + (int)currentfireMode;
            if (newFireMode < 0)
            {
                currentfireMode = EngiFireMode.Hold;
            }
            else if (newFireMode > 2)
            {
                currentfireMode = EngiFireMode.Default;
            }
            else
            {
                currentfireMode = (EngiFireMode)newFireMode;
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
                currentfireMode = EngiFireMode.Default;
            }
            if (Input.GetKeyDown(autoButton))
            {
                currentfireMode = EngiFireMode.Auto;
            }
            if (Input.GetKeyDown(holdButton))
            {
                currentfireMode = EngiFireMode.Hold;
            }
        }
    }
}
