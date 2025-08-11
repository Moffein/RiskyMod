using RiskyMod.SharedHooks;
using RoR2;
using SneedHooks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RiskyMod.Enemies.DLC1
{
    public class XiConstruct
    {
        public static bool enabled = true;
        public static BuffDef VulnerableDebuff;

        public XiConstruct()
        {
            if (!enabled) return;

            BuffDef sourceBuff = Addressables.LoadAssetAsync<BuffDef>("RoR2/Base/ArmorReductionOnHit/bdPulverized.asset").WaitForCompletion();
            GameObject enemyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/MegaConstructBody.prefab").WaitForCompletion();
            VulnerableDebuff = SneedUtils.SneedUtils.CreateBuffDef("RiskyModXiConstructVulnerableDebuff", false, false, false, new Color(0.9f * 140f / 255f, 0.9f * 185f / 255f, 0.9f * 191f / 255f), sourceBuff.iconSprite);

            if (SoftDependencies.LinearDamageLoaded)
            {
                SneedHooks.ModifyFinalDamage.ModifyFinalDamageActions += ModifyfinalDamage_Additive;
            }
            else
            {
                SneedHooks.ModifyFinalDamage.ModifyFinalDamageActions += ModifyfinalDamage;
            }

            On.EntityStates.MajorConstruct.Weapon.ChargeLaser.OnEnter += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.characterBody)
                {
                    self.characterBody.AddTimedBuff(VulnerableDebuff, self.duration + 0.4f);
                }
            };
            On.EntityStates.MajorConstruct.Weapon.FireLaser.OnEnter += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.characterBody)
                {
                    self.characterBody.AddTimedBuff(VulnerableDebuff, self.duration + 0.4f);
                }
            };
            On.EntityStates.MajorConstruct.Weapon.TerminateLaser.OnEnter += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.characterBody)
                {
                    self.characterBody.AddTimedBuff(VulnerableDebuff, self.duration + 0.4f);
                }
            };
        }

        private void ModifyfinalDamage(ModifyFinalDamage.DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, HealthComponent victim, CharacterBody victimBody)
        {
            if (victimBody.HasBuff(VulnerableDebuff))
            {
                damageModifierArgs.damageMultFinal *= 1.5f;
                if (damageInfo.damageColorIndex == DamageColorIndex.Default)
                {
                    damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                }
            }
        }

        private void ModifyfinalDamage_Additive(ModifyFinalDamage.DamageModifierArgs damageModifierArgs, DamageInfo damageInfo, HealthComponent victim, CharacterBody victimBody)
        {
            if (victimBody.HasBuff(VulnerableDebuff))
            {
                damageModifierArgs.damageMultAdd += 0.5f;
                if (damageInfo.damageColorIndex == DamageColorIndex.Default)
                {
                    damageInfo.damageColorIndex = DamageColorIndex.WeakPoint;
                }
            }
        }
    }
}
