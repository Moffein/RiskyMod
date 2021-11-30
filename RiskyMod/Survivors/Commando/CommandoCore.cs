using EntityStates;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.Skills;
using System;

namespace RiskyMod.Survivors.Commando
{
    public class CommandoCore
    {
        public static bool enabled = true;
        public static bool enablePrimarySkillChanges = true;
        public static bool enableSecondarySkillChanges = true;
        public static bool enableUtilitySkillChanges = true;
        public static bool enableSpecialSkillChanges = true;
        public CommandoCore()
        {
            if (!enabled) return;
            ModifySkills(RoR2Content.Survivors.Commando.bodyPrefab.GetComponent<SkillLocator>());
        }

        private void ModifySkills(SkillLocator sk)
        {
            ModifyPrimaries(sk);
        }

        private void ModifyPrimaries(SkillLocator sk)
        {
            if (!enablePrimarySkillChanges) return;

            //Removes the reload state completely since this messes with attack speed lategame.
            IL.EntityStates.Commando.CommandoWeapon.FirePistol2.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcI4(0)
                    );
                c.Index++;
                c.EmitDelegate<Func<int, int>>(zero =>
                {
                    return -1000;
                });
            };
        }
    }
}
