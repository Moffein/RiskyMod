using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies
{
    public class NoVoidDamage
    {
        public static bool enabled = true;
        public static bool alliesInVoidFields = true;

        public NoVoidDamage()
        {
            if (enabled) On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;

            if (alliesInVoidFields)
            {
                SceneDef arenaDef = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/arena/arena.asset").WaitForCompletion();
                arenaDef.suppressNpcEntry = false;
            }
        }

        private void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                //null attacker/inflictor
                //damagecolor void
                //bypassblock bypassarmor
                if ((!self.body.isPlayerControlled && self.body.teamComponent && self.body.teamComponent.teamIndex == TeamIndex.Player)
                    && !damageInfo.attacker && !damageInfo.inflictor
                    && damageInfo.damageColorIndex == DamageColorIndex.Void
                    && damageInfo.damageType == (DamageType.BypassArmor | DamageType.BypassBlock))
                {
                    damageInfo.damage = 0f;
                    damageInfo.rejected = true;
                }
            }
            orig(self, damageInfo);
        }
    }
}
