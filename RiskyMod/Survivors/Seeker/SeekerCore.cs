using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Seeker
{
    public class SeekerCore
    {
        public static bool enabled;
        public static bool noSelfRevive;
        public static bool unseenHandScalesDamage;
        public SeekerCore()
        {
            if (!enabled) return;

            DisableSelfRevive();
            UnseenHandScalesDamage();
        }

        private void DisableSelfRevive()
        {
            if (!noSelfRevive) return;

            IL.RoR2.SeekerController.UnlockGateEffects += SeekerController_UnlockGateEffects;

            SkillDef meditate = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC2/Seeker/SeekerBodyMeditate.asset").WaitForCompletion();
            for (int i = 0; i < meditate.keywordTokens.Length; i++)
            {
                if (meditate.keywordTokens[i] == "KEYWORD_LOWHEALTHPROTECTION")
                {
                    meditate.keywordTokens[i] = "KEYWORD_LOWHEALTHPROTECTION_RISKYMOD";
                    return;
                }
            }
        }

        private void SeekerController_UnlockGateEffects(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchStfld(typeof(CharacterMaster), "seekerSelfRevive")))
            {
                c.EmitDelegate<Func<bool, bool>>(x => false);
            }
            else
            {
                Debug.LogError("RiskyMod: Seeker DisableSelfRevive IL hook failed.");
            }
        }

        private void UnseenHandScalesDamage()
        {
            if (!unseenHandScalesDamage) return;

            GameObject handProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Seeker/UnseenHandMovingProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyModUnseenHandMovingProjectile", true);
            Content.Content.projectilePrefabs.Add(handProjectile);
            UnseenHandHealingProjectile u = handProjectile.GetComponent<UnseenHandHealingProjectile>();
            u.chakraIncrease = 0f;
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/DLC2/Seeker/EntityStates.Seeker.UnseenHand.asset", "fistProjectilePrefab", handProjectile);

            IL.EntityStates.Seeker.UnseenHand.FixedUpdate += UnseenHand_FixedUpdate;
            SkillDef def = Addressables.LoadAssetAsync<SkillDef>("RoR2/DLC2/Seeker/SeekerBodyUnseenHand.asset").WaitForCompletion();
            for (int i = 0; i < def.keywordTokens.Length; i++)
            {
                if (def.keywordTokens[i] == "SEEKER_SECONDARY_TRANQUILITY_DESCRIPTION")
                {
                    def.keywordTokens[i] = "SEEKER_SECONDARY_TRANQUILITY_DESCRIPTION_RISKYMOD";
                    return;
                }
            }
        }

        private void UnseenHand_FixedUpdate(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdfld(typeof(EntityStates.BaseState), "damageStat")))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<float, EntityStates.Seeker.UnseenHand, float>>((damage, self) =>
                {
                    float mult = 1f;
                    if (self.characterBody)
                    {
                        mult += (float)self.characterBody.GetBuffCount(DLC2Content.Buffs.ChakraBuff) / 6f;
                    }
                    return damage * mult;
                });
            }
            else
            {
                Debug.LogError("RiskyMod: Seeker Unseen Hand IL Hook failed.");
            }
        }
    }
}
