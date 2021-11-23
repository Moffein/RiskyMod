using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2.Components
{
    public class PassiveController : MonoBehaviour
    {
        SkillLocator sk;
        CharacterBody cb;

        private int prevSecondaryStock = 0;
        private int prevUtilityStock = 0;
        private int prevSpecialStock = 0;

        private bool quickdrawEnabled = false;

        GenericSkill passiveSkillSlot;

        public void Awake()
        {
            sk = base.GetComponent<SkillLocator>();
            if (!sk)
            {
                Destroy(this);
            }
            else
            {
                cb = base.GetComponent<CharacterBody>();
                if (!cb)
                {
                    Destroy(this);
                }
                else
                {
                    for (int i = 0; i < sk.allSkills.Length; i++)
                    {
                        if (sk.allSkills[i].skillFamily.variants[0].skillDef == Bandit2.Skills.Backstab)
                        {
                            passiveSkillSlot = sk.allSkills[i];
                            break;
                        }
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            if (passiveSkillSlot.skillDef == Bandit2.Skills.Quickdraw)
            {
                if (sk.secondary.stock < prevSecondaryStock || sk.utility.stock < prevUtilityStock || sk.special.stock < prevSpecialStock)
                {
                    sk.primary.stock = sk.primary.maxStock;
                }
                prevSecondaryStock = sk.secondary.stock;
                prevUtilityStock = sk.utility.stock;
                prevSpecialStock = sk.special.stock;

                if ((cb.bodyFlags & CharacterBody.BodyFlags.HasBackstabPassive) == CharacterBody.BodyFlags.HasBackstabPassive)
                {
                    cb.bodyFlags &= ~CharacterBody.BodyFlags.HasBackstabPassive;
                }
            }
            else
            {
                if ((cb.bodyFlags & CharacterBody.BodyFlags.HasBackstabPassive) != CharacterBody.BodyFlags.HasBackstabPassive)
                {
                    cb.bodyFlags |= CharacterBody.BodyFlags.HasBackstabPassive;
                }
            }

            if (sk.primary.stock > sk.primary.maxStock)
            {
                sk.primary.stock = sk.primary.maxStock;
            }
        }
    }
}
