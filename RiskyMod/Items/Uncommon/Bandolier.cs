using RoR2;
using UnityEngine;
using R2API;
using RiskyMod.Survivors.Toolbot;

namespace RiskyMod.Items.Uncommon
{
    public class Bandolier
    {
        public static bool enabled = true;
        public Bandolier()
        {
            if (!enabled) return;
            //Buff lifetime and pickup range
            GameObject ammoPack = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/AmmoPack");

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
                    if (isPrimary || isToolbotAkimbo)
                    {
                        self.Reset();
                    }
                }
            };
        }
    }
}
