using EntityStates.Loader;

namespace RiskyMod.Survivors.Loader
{
    public class FixScepterUtilityBarrier
    {
        //Remove this when Scepter updates and fixes this.
        public FixScepterUtilityBarrier()
        {
            if (!SoftDependencies.ScepterPluginLoaded) return;
            On.EntityStates.Loader.BaseSwingChargedFist.OnMeleeHitAuthority += (orig, self) =>
            {
                orig(self);
                self.healthComponent.AddBarrierAuthority(LoaderMeleeAttack.barrierPercentagePerHit * self.healthComponent.fullBarrier);
            };
        }
    }
}
