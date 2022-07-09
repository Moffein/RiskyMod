using UnityEngine;
using RoR2;
using BepInEx.Configuration;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Collections.Generic;

namespace RiskyMod.Survivors.Merc
{
    public class MercCore
    {
        public static bool enabled = true;

        public static bool evisTargetingFix = true;
        public static bool modifyStats = true;
        public static ConfigEntry<bool> m1ComboFinishTweak;

        public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/MercBody");
        public MercCore()
        {
            if (!enabled) return;
            ModifyStats(bodyPrefab.GetComponent<CharacterBody>());
            ModifySkills(bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
            ModifySpecials(sk);
        }
        private void ModifyPrimaries(SkillLocator sk)
        {
            On.EntityStates.Merc.Weapon.GroundLight2.OnEnter += (orig, self) =>
            {
                if (m1ComboFinishTweak.Value)
                {
                    if (self.isComboFinisher)
                    {
                        self.ignoreAttackSpeed = true;
                    }
                    else
                    {
                        self.ignoreAttackSpeed = false;
                    }
                }
                 orig(self);
            };
        }

        private void ModifySpecials(SkillLocator sk)
        {
            if (evisTargetingFix)
            {
                IL.EntityStates.Merc.EvisDash.FixedUpdate += (il) =>
                {
                    ILCursor c = new ILCursor(il);
                    if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCall<Physics>("OverlapSphere")
                    ))
                    {
                        c.Emit(OpCodes.Ldarg_0);//self
                        c.EmitDelegate<Func<Collider[], EntityStates.Merc.EvisDash, Collider[]>>((colliders, self) =>
                        {
                            if (FriendlyFireManager.friendlyFireMode != FriendlyFireManager.FriendlyFireMode.Off) return colliders;

                            //This is inefficient
                            List<Collider> enemyHurtboxes = new List<Collider>();

                            foreach (Collider cl in colliders)
                            {
                                HurtBox hb = cl.GetComponent<HurtBox>();
                                if (hb && hb.healthComponent != self.healthComponent)
                                {
                                    if(hb.teamIndex != self.GetTeam())
                                    {
                                        enemyHurtboxes.Add(cl);
                                    }
                                }
                            }

                            //Re-order list so that friendly hurtboxes are at the end
                            Collider[] finalColliders = new Collider[enemyHurtboxes.Count];
                            int index = 0;
                            foreach (Collider cl in enemyHurtboxes)
                            {
                                finalColliders[index] = null;
                                index++;
                            }
                            return finalColliders;
                        });
                    }
                    else
                    {
                        Debug.LogError("RiskyMod: Merc EvisDash IL Hook failed");
                    }
                };
            }
        }

        private void ModifyStats(CharacterBody cb)
        {
            if (!modifyStats) return;
            cb.baseRegen = 2.5f;
            cb.levelRegen = cb.baseRegen * 0.2f;
        }
    }
}
