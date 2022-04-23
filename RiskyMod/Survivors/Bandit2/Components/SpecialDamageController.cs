using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Bandit2.Components
{
    public class SpecialDamageController : NetworkBehaviour
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

        public SkillDef GetPassiveSkillDef()
        {
            return passiveSkillSlot.skillDef;
        }

        [ClientRpc]
        public void RpcResetSpecial()
        {
            if (base.hasAuthority && sk)
            {
                sk.special.Reset();
            }
        }
    }
}
