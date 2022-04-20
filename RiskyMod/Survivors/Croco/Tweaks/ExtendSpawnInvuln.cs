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
                    self.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
            };

            On.EntityStates.Croco.Spawn.OnExit += (orig, self) =>
            {
                if (NetworkServer.active && self.characterBody && self.characterBody.HasBuff(RoR2Content.Buffs.HiddenInvincibility))
                {
                    self.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
                }
                orig(self);
            };
        }
    }
}
