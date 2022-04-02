using RoR2;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RiskyMod.Survivors.Mage.Components
{
    public class MageStockController : MonoBehaviour
    {
        public static float graceDuration = 0.4f;    //Used when there's still stocks in the mag
        public static float baseDuration = 1f;

        private CharacterBody body;
        private SkillLocator skills;

        private float reloadStopwatch;
        private float delayStopwatch;

        private bool rightMuzzle;

        public static List<Type> ValidStates = new List<Type> { typeof(EntityStates.Mage.Weapon.FireFireBolt), typeof(EntityStates.Mage.Weapon.FireLightningBolt) };
        public static GameObject fireMuzzleflashEffectPrefab;
        public static GameObject lightningMuzzleflashEffectPrefab;
        public static GameObject iceMuzzleflashEffectPrefab;


        private void Awake()
        {
            body = base.GetComponent<CharacterBody>();
            skills = base.GetComponent<SkillLocator>();

            reloadStopwatch = 0f;
            delayStopwatch = 0f;
            rightMuzzle = true;
        }

        private void FixedUpdate()
        {
            if (!skills.hasAuthority) return;
            if (skills.primary.stock < skills.primary.maxStock && ValidStates.Contains(skills.primary.activationState.stateType))
            {
                if (skills.primary.stock <= 0) delayStopwatch = 0f;
                if (delayStopwatch > 0f)
                {
                    delayStopwatch -= Time.fixedDeltaTime;
                }
                else
                {
                    reloadStopwatch -= Time.fixedDeltaTime;
                    if (reloadStopwatch <= 0f)
                    {
                        reloadStopwatch += baseDuration / body.attackSpeed;

                        skills.primary.AddOneStock();
                        Util.PlaySound("Play_railgunner_m2_reload_basic", base.gameObject);
                        ShowReloadVFX();

                        if (skills.primary.stock > skills.primary.maxStock)
                        {
                            skills.primary.stock = skills.primary.maxStock;
                        }
                    }
                }
            }
            else
            {
                reloadStopwatch = baseDuration / body.attackSpeed;
            }
        }

        public void FireSkill(float duration)
        {
            delayStopwatch = graceDuration;  //Duration is already scaled to attack speed. InitialDelay is simply for inputs, and is ignored if the mag is empty.
            reloadStopwatch = baseDuration / body.attackSpeed;// + (skills.primary.stock <= 0 ? duration : 0f);
        }
        private void ShowReloadVFX()
        {
            GameObject currentEffectPrefab = null;
            if (skills.primary.activationState.stateType == typeof(EntityStates.Mage.Weapon.FireFireBolt))
            {
                currentEffectPrefab = fireMuzzleflashEffectPrefab;
            }
            else if (skills.primary.activationState.stateType == typeof(EntityStates.Mage.Weapon.FireLightningBolt))
            {
                currentEffectPrefab = lightningMuzzleflashEffectPrefab;
            }
            else
            {
                currentEffectPrefab = iceMuzzleflashEffectPrefab;
            }


            if (currentEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(currentEffectPrefab, base.gameObject, rightMuzzle ? "MuzzleRight" : "MuzzleLeft", true);
            }

            rightMuzzle = !rightMuzzle;
        }
    }
}
