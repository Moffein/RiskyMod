using RoR2;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Croco
{
    public class ExtendSpawnInvuln
    {
        public ExtendSpawnInvuln()
        {
            On.EntityStates.Croco.Spawn.OnEnter += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.characterBody)
                {
                    float invulnDuration = EntityStates.Croco.Spawn.minimumSleepDuration + EntityStates.Croco.WakeUp.duration + 2f;
                    self.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, invulnDuration);
                }
            };
        }
    }
}
