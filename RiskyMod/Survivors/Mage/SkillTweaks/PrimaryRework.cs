using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.MonoBehaviours;
using RiskyMod.Survivors.Mage.Components.Primaries;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine;
using RoR2.Skills;

namespace RiskyMod.Survivors.Mage.SkillTweaks
{
    public class PrimaryRework
    {
        public static DamageAPI.ModdedDamageType QuadrupleHitstunPower;
        public static DamageAPI.ModdedDamageType ApplyFireboltRepeat;

        public static bool enabled;

        public PrimaryRework()
        {
            if (!enabled) return;
            QuadrupleHitstunPower = DamageAPI.ReserveDamageType();
            IL.RoR2.SetStateOnHurt.OnTakeDamageServer += SetStateOnHurt_OnTakeDamageServer;

            if (MageCore.modifyFireBolt) SetupFirebolt();
            if (MageCore.modifyPlasmaBolt) SetupLightningBolt();
        }

        private void SetStateOnHurt_OnTakeDamageServer(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After, x => x.MatchLdfld<DamageReport>("damageDealt")))
            {
                c.Emit(OpCodes.Ldarg_1);
                c.EmitDelegate<Func<float, DamageReport, float>>((damageDealt, damageReport) =>
                {
                    if (damageReport.damageInfo != null && damageReport.damageInfo.HasModdedDamageType(QuadrupleHitstunPower))
                    {
                        return damageDealt * 4f;
                    }
                    return damageDealt;
                });
            }
        }

        private void SetupFirebolt()
        {
            ApplyFireboltRepeat = DamageAPI.ReserveDamageType();
            GameObject fireBolt = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageFireboltBasic.prefab").WaitForCompletion().InstantiateClone("MoffeinArtificerPrimaryRework_Firebolt");
            fireBolt.GetComponent<ProjectileSimple>().lifetime = 10f;
            fireBolt.AddComponent<FireBoltForceAddModdedDamageType>();
            Content.Content.projectilePrefabs.Add(fireBolt);
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Mage/EntityStates.Mage.Weapon.FireFireBolt.asset", "damageCoefficient", "0.7");
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Mage/EntityStates.Mage.Weapon.FireFireBolt.asset", "projectilePrefab", fireBolt);

            //fireboltRepeatObject = assetBundle.LoadAsset<GameObject>("FireboltRepeatPrefab");
            FireBoltRepeatComponent.damageEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            /*FireBoltRepeatComponent.damageEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion().InstantiateClone("MoffeinArtificerPrimaryRework_Firebolt_DamageEffect");
            EffectComponent ec = FireBoltRepeatComponent.damageEffectPrefab.GetComponent<EffectComponent>();
            ec.soundName = "";
            ContentAddition.AddEffect(FireBoltRepeatComponent.damageEffectPrefab);*/

            SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFireFirebolt.asset").WaitForCompletion();
            skillDef.attackSpeedBuffsRestockSpeed = false;
            skillDef.skillDescriptionToken = "MAGE_PRIMARY_FIRE_DESCRIPTION_RISKYMOD";

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            if (!NetworkServer.active || damageInfo.rejected || !damageInfo.HasModdedDamageType(ApplyFireboltRepeat) || damageInfo.procCoefficient <= 0f) return;

            FireBoltRepeatComponent frc = self.gameObject.AddComponent<FireBoltRepeatComponent>();
            frc.attacker = damageInfo.attacker;
            frc.victim = self.gameObject;
            frc.victimHealthComponent = self;
            frc.damage = damageInfo.damage;
            frc.damageType = damageInfo.damageType;
            frc.damageType.RemoveModdedDamageType(ApplyFireboltRepeat);
            frc.isCrit = damageInfo.crit;
            frc.procCoefficient = damageInfo.procCoefficient;
            frc.force = damageInfo.force.magnitude;
        }

        private void SetupLightningBolt()
        {
            GameObject lightningBolt = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageLightningboltBasic.prefab").WaitForCompletion().InstantiateClone("MoffeinArtificerPrimaryRework_Lightningbolt");
            lightningBolt.GetComponent<ProjectileSimple>().lifetime = 10f;
            lightningBolt.AddComponent<LightningBoltTriggerComponent>();
            Content.Content.projectilePrefabs.Add(lightningBolt);
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Mage/EntityStates.Mage.Weapon.FireLightningBolt.asset", "damageCoefficient", "0.85");
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Mage/EntityStates.Mage.Weapon.FireLightningBolt.asset", "projectilePrefab", lightningBolt);

            SkillDef skillDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Mage/MageBodyFireLightningBolt.asset").WaitForCompletion();
            skillDef.attackSpeedBuffsRestockSpeed = false;
            skillDef.skillDescriptionToken = "MAGE_PRIMARY_LIGHTNING_DESCRIPTION_RISKYMOD";

            ProjectileImpactExplosion pie = lightningBolt.GetComponent<ProjectileImpactExplosion>();

            LightningBoltTriggerComponent.lightningboltRepeatObject = Content.Assets.assetBundle.LoadAsset<GameObject>("LightningboltRepeatPrefab");
            LightningBoltRepeatComponent lrc = LightningBoltTriggerComponent.lightningboltRepeatObject.AddComponent<LightningBoltRepeatComponent>();

            lrc.range = 7.5f;
            pie.blastRadius = 0f;

            pie.blastProcCoefficient = 0f;

            lrc.activationEffectPrefab = pie.impactEffect;
            pie.impactEffect = null;

            pie.blastDamageCoefficient = 0f;

            NetworkIdentity n = LightningBoltTriggerComponent.lightningboltRepeatObject.AddComponent<NetworkIdentity>();
            PrefabAPI.RegisterNetworkPrefab(LightningBoltTriggerComponent.lightningboltRepeatObject);
            Content.Content.networkedObjectPrefabs.Add(LightningBoltTriggerComponent.lightningboltRepeatObject);
            RadialForceMassLimited rf = LightningBoltTriggerComponent.lightningboltRepeatObject.AddComponent<RadialForceMassLimited>();
            rf.radius = lrc.range;
            rf.damping = 0.5f;
            rf.forceMagnitude = -250f;
            rf.forceCoefficientAtEdge = 0.5f;
            rf.maxMass = 250f;  //Greater wisps are 300. Yank hook is 250.
            rf.flyingOnly = false;
            rf.ignorePlayerControlled = true;

            lrc.whiffEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFXLightning.prefab").WaitForCompletion();
            lrc.activationEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/LightningStakeNova.prefab").WaitForCompletion();
            /*lrc.activationEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/EliteLightning/LightningStakeNova.prefab").WaitForCompletion().InstantiateClone("MoffeinArtificerPrimaryRework_LightningExplosionVfx");
            lrc.activationEffectPrefab.GetComponent<EffectComponent>().soundName = "Play_item_proc_chain_lightning";
            ContentAddition.AddEffect(lrc.activationEffectPrefab);*/

            lrc.attackSound = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            lrc.attackSound.eventName = "Play_item_proc_chain_lightning";
            Content.Content.networkSoundEventDefs.Add(lrc.attackSound);

            On.EntityStates.Mage.Weapon.FireLightningBolt.ModifyProjectileInfo += FireLightningBolt_ModifyProjectileInfo;
        }

        private void FireLightningBolt_ModifyProjectileInfo(On.EntityStates.Mage.Weapon.FireLightningBolt.orig_ModifyProjectileInfo orig, EntityStates.Mage.Weapon.FireLightningBolt self, ref FireProjectileInfo fireProjectileInfo)
        {
            orig(self, ref fireProjectileInfo);
            fireProjectileInfo.damageTypeOverride |= DamageType.SlowOnHit;
        }
    }
}
