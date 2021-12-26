using RoR2;
using R2API;
using UnityEngine;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyM2Bite
    {
        AnimationCurve biteLunge;
        public ModifyM2Bite()
        {
            On.EntityStates.Croco.Bite.AuthorityModifyOverlapAttack += (orig, self, overlapAttack) =>
            {
                orig(self, overlapAttack);
                overlapAttack.damageType = DamageType.BonusToLowHealth | DamageType.BlightOnHit;
                overlapAttack.AddModdedDamageType(SharedDamageTypes.CrocoBiteHealOnKill);
            };

            //This felt jank compared to Bandit, even when modifying the velocity curve.
            /*biteLunge = BuildBiteVelocityCurve();
            On.EntityStates.Croco.Bite.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.characterBody && self.characterBody.isSprinting)
                {
                    self.ignoreAttackSpeed = true;
                    self.forceForwardVelocity = true;
                    self.forwardVelocityCurve = biteLunge;
                }
                self.durationBeforeInterruptable = 0.3f;
            };
            SneedUtils.SneedUtils.SetEntityStateField("entitystates.croco.bite", "ignoreAttackSpeed", "1");*/
        }

        private AnimationCurve BuildBiteVelocityCurve()
        {
            Keyframe kf1 = new Keyframe(0f, 1.5f, -8.182907104492188f, -3.3333332538604738f, 0f, 0.058712735772132876f);
            kf1.weightedMode = WeightedMode.None;
            kf1.tangentMode = 65;

            Keyframe kf2 = new Keyframe(0.3f, 0f, -3.3333332538604738f, -3.3333332538604738f, 0.3333333432674408f, 0.3333333432674408f);    //Time should match up with SlashBlade min duration (hitbox length)
            kf2.weightedMode = WeightedMode.None;
            kf2.tangentMode = 34;

            Keyframe[] keyframes = new Keyframe[2];
            keyframes[0] = kf1;
            keyframes[1] = kf2;

            return new AnimationCurve
            {
                preWrapMode = WrapMode.ClampForever,
                postWrapMode = WrapMode.ClampForever,
                keys = keyframes
            };
        }
    }
}
