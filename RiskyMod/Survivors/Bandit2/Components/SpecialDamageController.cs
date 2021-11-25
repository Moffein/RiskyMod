using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2.Components
{
    public class SpecialDamageController : MonoBehaviour
    {
        SkillLocator sk;
        CharacterBody cb;

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
                        if (sk.allSkills[i].skillFamily.variants[0].skillDef == Bandit2.Skills.Gunslinger)
                        {
                            passiveSkillSlot = sk.allSkills[i];
                            break;
                        }
                    }
                }
            }
        }

        public DamageType GetDamageType()
        {
            return (passiveSkillSlot.skillDef == Bandit2.Skills.Gunslinger) ? DamageType.ResetCooldownsOnKill : DamageType.GiveSkullOnKill;
        }
    }
}
