using RoR2;
using R2API;
using RiskyMod.Survivors.Mage;

namespace EntityStates.RiskyMod.Mage
{
    public class FlamethrowerScepter : Flamethrower
    {
        public override void LoadStats()
        {
            base.LoadStats();
            //this.loadBaseTickCount = 30;
            this.loadMaxDistance = 30f;
            //this.loadignitePercentChance = 100f;
            //this.loadBaseTickFrequency = 14f/3f;
        }

        public override void ModifyBullet(BulletAttack ba)
        {
            base.ModifyBullet(ba);
            ba.AddModdedDamageType(ScepterHandler.FlamethrowerScepterDamage);
        }
    }
}
