using R2API;
using RoR2;
using UnityEngine;

namespace Risky_Mod.Items.Legendary
{
    public class FrostRelic
    {
        public static bool enabled = true;

        public static bool removeBubble = true;
        public static bool removeFOV = true;

        public static void Modify()
        {
            if (!enabled) return;

            LanguageAPI.Add("ITEM_ICICLE_DESC", "Killing an enemy surrounds you with an <style=cIsDamage>ice storm</style> that deals <style=cIsDamage>1200% <style=cStack>(+600% per stack)</style> damage per second</style> and <style=cIsUtility>slows</style> enemies by <style=cIsUtility>80%</style> for <style=cIsUtility>1.5s</style>. The storm <style=cIsDamage>grows with every kill</style>, increasing its radius by <style=cIsDamage>3m</style>. Stacks up to <style=cIsDamage>24m</style>.");

            On.RoR2.IcicleAuraController.Awake += (orig, self) =>
            {
                orig(self);

                /*
                Debug.Log(self.baseIcicleAttackInterval);   //0.25
                Debug.Log(self.icicleBaseRadius);   //6
                Debug.Log(self.icicleRadiusPerIcicle);  //2
                Debug.Log(self.icicleDamageCoefficientPerTick); //3
                Debug.Log(self.icicleDamageCoefficientPerTickPerIcicle);    //0
                Debug.Log(self.icicleDuration); //5
                Debug.Log(self.icicleProcCoefficientPerTick);   //0.2
                Debug.Log(self.icicleCountOnFirstKill); //0
                Debug.Log(self.baseIcicleMax);  //6
                Debug.Log(self.icicleMaxPerStack);  //6
                */

                if (Risky_Mod.disableProcChains)
                {
                    self.icicleProcCoefficientPerTick = 0f;
                }
                self.icicleRadiusPerIcicle = 3f;
                self.icicleDuration = 6f;
            };

            On.RoR2.IcicleAuraController.FixedUpdate += (orig, self) =>
            {
                //Update damage based on stack
                int itemCount = 1;
                if (self.cachedOwnerInfo.characterBody && self.cachedOwnerInfo.characterBody.inventory)
                {
                    itemCount = self.cachedOwnerInfo.characterBody.inventory.GetItemCount(RoR2Content.Items.Icicle);
                }
                self.icicleDamageCoefficientPerTick = 1.5f + 1.5f * itemCount;

                orig(self);
            };

            //Clamp max radius
            On.RoR2.IcicleAuraController.UpdateRadius += (orig, self) =>
            {
                if (self.owner)
                {
                    if (self.finalIcicleCount > 0)
                    {
                        float maxRadius = self.icicleBaseRadius + self.baseIcicleMax * self.icicleRadiusPerIcicle;
                        float calculatedRadius = self.cachedOwnerInfo.characterBody ? (self.cachedOwnerInfo.characterBody.radius + self.icicleBaseRadius + self.icicleRadiusPerIcicle * self.finalIcicleCount) : 0f;
                        self.actualRadius = Mathf.Min(maxRadius, calculatedRadius);
                        return;
                    }
                    self.actualRadius = 0f;
                }
            };

            //Disable FOV change
            if (removeFOV)
            {
                On.RoR2.IcicleAuraController.OnIciclesActivated += (orig, self) =>
                {
                    orig(self);

                    CameraTargetParams.AimRequest aimRequest = self.aimRequest;
                    if (aimRequest == null)
                    {
                        return;
                    }
                    aimRequest.Dispose();
                };
            }

            //Remove bubble
            if (removeBubble)
            {
                GameObject indicator = Resources.Load<GameObject>("Prefabs/NetworkedObjects/IcicleAura");
                ParticleSystemRenderer[] pr = indicator.GetComponentsInChildren<ParticleSystemRenderer>();
                foreach (ParticleSystemRenderer p in pr)
                {
                    //Debug.Log(p.name);
                    if (p.name == "Area")
                    {
                        Object.Destroy(p);
                    }
                }
            }
        }
    }
}
