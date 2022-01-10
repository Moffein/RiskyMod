using RoR2;
using RoR2.Orbs;

namespace RiskyMod.Survivors.Treebot
{
    public class SwapUtilityEffects
    {
        public SwapUtilityEffects()
        {
            On.EntityStates.Treebot.Weapon.FireSonicBoom.AddDebuff += (orig, self, body) =>
            {
                SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
                if (component != null)
                {
                    component.SetStun(-1f);
                }
                if (EntityStates.Treebot.Weapon.FirePlantSonicBoom.hitEffectPrefab) //Needs impact effect for hit feedback to feel good.
                {
                    EffectManager.SpawnEffect(EntityStates.Treebot.Weapon.FirePlantSonicBoom.hitEffectPrefab, new EffectData
                    {
                        origin = body.corePosition
                    }, true);
                }
                if (self.healthComponent)
                {
                    HealOrb healOrb = new HealOrb();
                    healOrb.origin = body.corePosition;
                    healOrb.target = self.healthComponent.body.mainHurtBox;
                    healOrb.healValue = 0.05f * self.healthComponent.fullHealth;    //PlantSonicBoom is 10%HP. Heal less because there is no health cost.
                    healOrb.overrideDuration = UnityEngine.Random.Range(0.3f, 0.6f);
                    OrbManager.instance.AddOrb(healOrb);
                }
                Util.PlaySound(EntityStates.Treebot.Weapon.FirePlantSonicBoom.impactSoundString, self.gameObject);
            };

            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Treebot.Weapon.FirePlantSonicBoom", "slowDuration", "4");
            On.EntityStates.Treebot.Weapon.FirePlantSonicBoom.AddDebuff += (orig, self, body) =>
            {
                body.AddTimedBuff(RoR2Content.Buffs.Weak, self.slowDuration);
                SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
                if (component != null)
                {
                    component.SetStun(-1f);
                }
                if (EntityStates.Treebot.Weapon.FirePlantSonicBoom.hitEffectPrefab)
                {
                    EffectManager.SpawnEffect(EntityStates.Treebot.Weapon.FirePlantSonicBoom.hitEffectPrefab, new EffectData
                    {
                        origin = body.corePosition
                    }, true);
                }
                Util.PlaySound(EntityStates.Treebot.Weapon.FirePlantSonicBoom.impactSoundString, self.gameObject);
            };
        }
    }
}
