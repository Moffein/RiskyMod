using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Bandit2.Components
{
    [RequireComponent(typeof(CharacterBody), typeof(SkillLocator))]
    public class SpecialDamageController : MonoBehaviour
    {
        SkillLocator sk;
        CharacterBody cb;

        GenericSkill passiveSkillSlot;

        public void Awake()
        {
            sk = base.GetComponent<SkillLocator>();
            cb = base.GetComponent<CharacterBody>();
            for(int i = 0; i < sk.allSkills.Length; i++)
               {
                foreach (SkillFamily.Variant variant in sk.allSkills[i].skillFamily.variants)
                {
                    if (variant.skillDef == Bandit2.Skills.Gunslinger)
                    {
                        passiveSkillSlot = sk.allSkills[i];
                        break;
                    }
                }
            }
        }

        public SkillDef GetPassiveSkillDef()
        {
            return passiveSkillSlot.skillDef;
        }
    }
}
