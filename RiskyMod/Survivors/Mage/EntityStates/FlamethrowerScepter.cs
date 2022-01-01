using RoR2;
using R2API;
using RiskyMod.Survivors.Mage;

namespace EntityStates.RiskyMod.Mage
{
    public class FlamethrowerScepter : Flamethrower
    {
        public static int maxFlames = 30;
        public int flames;
        public override void LoadStats()
        {
            base.LoadStats();
            //this.loadBaseTickCount = 30;
            this.loadMaxDistance = 30f;
            //this.loadignitePercentChance = 100f;
            //this.loadBaseTickFrequency = 14f/3f;
            flames = 0;
        }

        public override void ModifyBullet(BulletAttack ba)
        {
            base.ModifyBullet(ba);
            if (flames < maxFlames)
            {
                ba.AddModdedDamageType(ScepterHandler.FlamethrowerScepterDamage);
                flames++;
            }
        }
    }
}
