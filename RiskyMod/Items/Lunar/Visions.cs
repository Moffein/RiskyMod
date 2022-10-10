using System;
using EntityStates;
using JetBrains.Annotations;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RiskyMod.Items.Lunar
{
    public class Visions
    {
        public static bool enabled = true;

        public static LunarPrimaryReloadSkillDef VisionsReloadableSkill;

        public Visions()
        {
            if (!enabled) return;

            Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.LunarReplacement.ReloadVisions));
            LunarPrimaryReplacementSkill visionsDef = LegacyResourcesAPI.Load<LunarPrimaryReplacementSkill>("SkillDefs/LunarReplacements/LunarPrimaryReplacement");

            LunarPrimaryReloadSkillDef visionsReloadDef = ScriptableObject.CreateInstance<LunarPrimaryReloadSkillDef>();
            visionsReloadDef.activationState = new SerializableEntityStateType(typeof(EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle));
            visionsReloadDef.activationStateMachineName = "Weapon";
            visionsReloadDef.baseMaxStock = 12;
            visionsReloadDef.baseRechargeInterval = 0f;
            visionsReloadDef.beginSkillCooldownOnSkillEnd = true;
            visionsReloadDef.canceledFromSprinting = false;
            visionsReloadDef.cancelSprintingOnActivation = true;
            visionsReloadDef.dontAllowPastMaxStocks = true;
            visionsReloadDef.forceSprintDuringState = false;
            visionsReloadDef.fullRestockOnAssign = true;
            visionsReloadDef.graceDuration = 0.4f;
            visionsReloadDef.icon = visionsDef.icon;
            visionsReloadDef.interruptPriority = InterruptPriority.Skill;
            visionsReloadDef.rechargeStock = 0;
            visionsReloadDef.reloadInterruptPriority = InterruptPriority.Any;
            visionsReloadDef.reloadState = new SerializableEntityStateType(typeof(EntityStates.RiskyMod.LunarReplacement.ReloadVisions));
            visionsReloadDef.requiredStock = 1;
            visionsReloadDef.resetCooldownTimerOnUse = true;
            visionsReloadDef.skillDescriptionToken = visionsDef.skillDescriptionToken;
            visionsReloadDef.skillName = visionsDef.skillName;
            visionsReloadDef.skillNameToken = visionsDef.skillNameToken;
            visionsReloadDef.stockToConsume = 1;
            Content.Content.skillDefs.Add(visionsReloadDef);

            Visions.VisionsReloadableSkill = visionsReloadDef;

            //How does this change interactions with the skills of other characters?
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.GetMinimumInterruptPriority += (orig, self) =>
            {
                return InterruptPriority.PrioritySkill;
            };

            IL.RoR2.CharacterBody.OnInventoryChanged += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdsfld(typeof(CharacterBody.CommonAssets), "lunarPrimaryReplacementSkillDef")
                     ))
                {
                    c.EmitDelegate<Func<SkillDef, SkillDef>>(orig => Visions.VisionsReloadableSkill);
                }
                else
                {
                    Debug.LogError("RiskyMod: Visions of Heresy IL Hook failed.");
                }
            };
        }
    }
}

namespace EntityStates.RiskyMod.LunarReplacement
{
    public class ReloadVisions : BaseState
    {
        public static float baseDuration = 2f;

		private float totalDuration;

        public override void OnEnter()
        {
            base.OnEnter();

            totalDuration = baseDuration;
            if (base.characterBody && base.characterBody.inventory)
            {
                int itemCount = Mathf.Max(1, base.characterBody.inventory.GetItemCount(RoR2Content.Items.LunarPrimaryReplacement));
                totalDuration *= itemCount;
            }
            totalDuration /= base.attackSpeedStat;
        }

        public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority && base.fixedAge >= totalDuration)
			{
                if (base.skillLocator)
                {
                    base.skillLocator.primary.stock = base.skillLocator.primary.maxStock;
                }
				this.outer.SetNextStateToMain();
			}
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}