using RoR2;
using RoR2.Orbs;

namespace RiskyMod.Survivors.Treebot
{
    public class DefaultUtilityHeal
    {
        public DefaultUtilityHeal()
        {
            On.EntityStates.Treebot.Weapon.FireSonicBoom.AddDebuff += (orig, self, body) =>
            {
                body.AddTimedBuff(RoR2Content.Buffs.Weak, self.slowDuration);
                SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
                if (component != null)
                {
                    component.SetStun(-1f);
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
        }
    }
}
