using RoR2;
using UnityEngine;
using R2API;
using RiskyMod.Survivors.Toolbot;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.Uncommon
{
    public class Bandolier
    {
        public static bool enabled = true;
        public Bandolier()
        {
            if (!enabled) return;
            //Buff lifetime and pickup range
            GameObject ammoPack = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandolier/AmmoPack.prefab").WaitForCompletion();

            DestroyOnTimer dt = ammoPack.GetComponent<DestroyOnTimer>();
            dt.duration = 14f;

            BeginRapidlyActivatingAndDeactivating br = ammoPack.GetComponent<BeginRapidlyActivatingAndDeactivating>();
            br.delayBeforeBeginningBlinking = dt.duration - 2f;

            GravitatePickup gp = ammoPack.GetComponentInChildren<GravitatePickup>();

            Collider pickupTrigger = gp.gameObject.GetComponent<Collider>();
            if (pickupTrigger && pickupTrigger.isTrigger)
            {
                pickupTrigger.transform.localScale *= 2f;
            }

            On.RoR2.GenericSkill.ApplyAmmoPack += (orig, self) =>
            {
                orig(self);
                if (self.characterBody)
                {
                    bool isPrimary = self.characterBody.skillLocator && self == self.characterBody.skillLocator.primary;
                    bool isToolbotAkimbo = (ToolbotCore.enablePowerModeChanges && self.characterBody.HasBuff(ToolbotCore.PowerModeBuff)) || (!ToolbotCore.enablePowerModeChanges && self.characterBody.HasBuff(RoR2Content.Buffs.SmallArmorBoost));
                    isToolbotAkimbo = isToolbotAkimbo && self.skillName == "StunDrone";

                    bool isContextualOverride = self.HasSkillOverrideOfPriority(GenericSkill.SkillOverridePriority.Contextual);

                    if ((isPrimary && !isContextualOverride) || isToolbotAkimbo)
                    {
                        self.Reset();
                    }
                }
            };
        }
    }
}
