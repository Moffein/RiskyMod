using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;

namespace RiskyMod.Survivors.Toolbot.Components
{
    public class GrenadeStockController : MonoBehaviour
    {
        public static float graceDuration = 0.4f;    //Used when there's still stocks in the mag
        public static float baseDuration = 1.5f;

        private CharacterBody body;
        private SkillLocator skills;

        private List<SkillStatus> skillStatuses;

        private class SkillStatus
        {
            public GenericSkill skill;
            public float reloadStopwatch = 0f;
            public float delayStopwatch = 0f;
            
            public SkillStatus(GenericSkill skill)
            {
                this.skill = skill;
            }
        }

        private void Awake()
        {
            body = base.GetComponent<CharacterBody>();
            skills = base.GetComponent<SkillLocator>();

            skillStatuses = new List<SkillStatus>();
        }

        private void FixedUpdate()
        {
            foreach (SkillStatus s in skillStatuses)
            {
                if (s.skill.stock < s.skill.maxStock)
                {
                    if (s.skill.stock <= 0) s.delayStopwatch = 0f;
                    if (s.delayStopwatch > 0f)
                    {
                        s.delayStopwatch -= Time.fixedDeltaTime;
                    }
                    else
                    {
                        s.reloadStopwatch -= Time.fixedDeltaTime;
                        if (s.reloadStopwatch <= 0f)
                        {
                            s.reloadStopwatch = baseDuration / body.attackSpeed;

                            s.skill.stock = s.skill.maxStock;
                            Util.PlaySound("Play_captain_m1_reload", base.gameObject);
                        }
                    }
                }
                else
                {
                    s.reloadStopwatch = baseDuration / body.attackSpeed;
                }
            }
        }

        public void FireSkill(GenericSkill g)
        {
            bool set = false;
            foreach (SkillStatus s in skillStatuses)
            {
                if (s.skill == g)
                {
                    set = true;
                    s.delayStopwatch = graceDuration;
                    s.reloadStopwatch = baseDuration / body.attackSpeed;
                }
            }

            if (!set)
            {
                SkillStatus s = new SkillStatus(g);
                s.delayStopwatch = graceDuration;
                s.reloadStopwatch = baseDuration / body.attackSpeed;

                skillStatuses.Add(s);
            }
        }
    }
}
