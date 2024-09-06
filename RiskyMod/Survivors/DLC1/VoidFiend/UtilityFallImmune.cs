using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.DLC1.VoidFiend
{
    public class UtilityFallImmune
    {
        public static bool enabled = true;
        public static BuffDef UtilityBuff;
        public UtilityFallImmune()
        {
            if (!enabled) return;

            BuffDef vanillaSpeedBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/SprintOutOfCombat/bdWhipBoost.asset").WaitForCompletion();    //Steal icon + color from here
            UtilityBuff = SneedUtils.SneedUtils.CreateBuffDef("RiskyModVoidFiendFallImmuneBuff", false, false, false, new Color(0.376f, 0.843f, 0.898f), vanillaSpeedBuff.iconSprite);

            //Give a bonus to move speed when affected by the buff.
            R2API.RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender.HasBuff(UtilityBuff))
                {
                    args.moveSpeedMultAdd += 0.3f;
                }
            };

            //Reject fall damage while buff is active
            On.RoR2.HealthComponent.TakeDamageProcess += (orig, self, damageInfo) =>
            {
                if (damageInfo.damageType.damageType.HasFlag(DamageType.FallDamage) && self.body.HasBuff(UtilityBuff))
                {
                    damageInfo.rejected = true;
                    damageInfo.damage = 0f;
                }
                orig(self, damageInfo);
            };

            //Remove the buff upon touching the ground
            On.RoR2.CharacterMotor.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.isGrounded && self.body.HasBuff(UtilityBuff))
                {
                    self.body.RemoveBuff(UtilityBuff);
                }
            };

            //Add the buff when exiting Tresspass
            On.EntityStates.VoidSurvivor.VoidBlinkBase.OnExit += (orig, self) =>
            {
                orig(self);
                //Buffs are handled server-side, so NetworkServer.active is needed.
                if (NetworkServer.active && self.characterBody && self.characterMotor && !self.characterMotor.isGrounded)
                {
                    if (!self.characterBody.HasBuff(UtilityBuff))
                    {
                        self.characterBody.AddBuff(UtilityBuff);
                    }
                }
            };
        }
    }
}
