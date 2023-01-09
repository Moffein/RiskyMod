using BepInEx.Configuration;
using RoR2;
namespace RiskyMod.Survivors.DLC1.VoidFiend
{
    public class UtilityMoveSpeedScaling
    {
        public static ConfigEntry<bool> disableScaling;

        public UtilityMoveSpeedScaling()
        {
            On.EntityStates.VoidSurvivor.VoidBlinkBase.GetVelocity += VoidBlinkBase_GetVelocity;
        }

        private UnityEngine.Vector3 VoidBlinkBase_GetVelocity(On.EntityStates.VoidSurvivor.VoidBlinkBase.orig_GetVelocity orig, EntityStates.VoidSurvivor.VoidBlinkBase self)
        {
            if (disableScaling.Value)
            {
                self.moveSpeedStat = 7f * 1.45f;
            }
            return orig(self);
        }
    }
}
