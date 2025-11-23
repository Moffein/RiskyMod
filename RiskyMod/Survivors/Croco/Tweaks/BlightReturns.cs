using MonoMod.Cil;
using RoR2;
using RoR2.Skills;
using SneedHooks;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Survivors.Croco.Tweaks
{
    public class BlightReturns
    {
        public static bool enabled;
        public static float damageMultPerBuff = 0.05f;

        public BlightReturns()
        {
            if (!enabled) return;
            Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Croco/CrocoPassiveBlight.asset").WaitForCompletion().skillDescriptionToken = "CROCO_PASSIVE_ALT_DESCRIPTION_RISKYMOD";
            if (SoftDependencies.LinearDamageLoaded)
            {
                SneedHooks.ModifyFinalDamage.ModifyFinalDamageActions += ModifyFinalDamage_Additive;
            }
            else
            {
                SneedHooks.ModifyFinalDamage.ModifyFinalDamageActions += ModifyFinalDamage;
            }
            On.RoR2.DotController.AddDot_GameObject_float_HurtBox_DotIndex_float_Nullable1_Nullable1_Nullable1 += ModifyBlight;
            IL.RoR2.GlobalEventManager.ProcessHitEnemy += GlobalEventManager_ProcessHitEnemy;
        }

        private void GlobalEventManager_ProcessHitEnemy(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(x => x.MatchLdfld<DamageInfo>("damageType"), x => x.MatchLdcI4(1048576))
                && c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(5)))
            {
                c.EmitDelegate<Func<float, float>>(x => 10f);
            }
        }

        private void ModifyFinalDamage(ModifyFinalDamage.DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, HealthComponent victim, CharacterBody victimBody)
        {
            int buffCount = victimBody.GetBuffCount(RoR2Content.Buffs.Blight);
            if (buffCount > 0)
            {
                damageModifierArgs.damageMultFinal *= 1f + (damageMultPerBuff * buffCount);
                if (damageInfo.dotIndex != DotController.DotIndex.Blight) damageInfo.damageColorIndex = DamageColorIndex.Void;
            }
        }

        private void ModifyFinalDamage_Additive(ModifyFinalDamage.DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, HealthComponent victim, CharacterBody victimBody)
        {
            int buffCount = victimBody.GetBuffCount(RoR2Content.Buffs.Blight);
            if (buffCount > 0)
            {
                damageModifierArgs.damageMultAdd += damageMultPerBuff * buffCount;
                if (damageInfo.dotIndex != DotController.DotIndex.Blight) damageInfo.damageColorIndex = DamageColorIndex.Void;
            }
        }

        private void ModifyBlight(On.RoR2.DotController.orig_AddDot_GameObject_float_HurtBox_DotIndex_float_Nullable1_Nullable1_Nullable1 orig, DotController self, GameObject attackerObject, float duration, HurtBox hitHurtBox, DotController.DotIndex dotIndex, float damageMultiplier, uint? maxStacksFromAttacker, float? totalDamage, DotController.DotIndex? preUpgradeDotIndex)
        {
            //Set Blight damage to 0 if attacker already has a blight stack to prevent damage scaling
            if (dotIndex == DotController.DotIndex.Blight)
            {
                foreach (var stack in self.dotStackList)
                {
                    if (stack.attackerObject == attackerObject && stack.dotIndex == dotIndex)
                    {
                        damageMultiplier = 0f;
                        break;
                    }
                }
            }

            orig(self, attackerObject, duration, hitHurtBox, dotIndex, damageMultiplier, maxStacksFromAttacker, totalDamage, preUpgradeDotIndex);

            //Reset Blight duration
            if (dotIndex == DotController.DotIndex.Blight)
            {
                for (int i = 0; i < self.dotStackList.Count; i++)
                {
                    if (self.dotStackList[i].dotIndex == DotController.DotIndex.Blight)
                    {
                        self.dotStackList[i].timer = Mathf.Max(self.dotStackList[i].timer, duration);
                    }
                }
            }
        }
    }
}
