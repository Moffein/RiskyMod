using RoR2;
using UnityEngine;
using R2API;
using MonoMod.Cil;
using System;

namespace RiskyMod.Survivors.Toolbot
{
    public class ToolbotCore
    {
        public static bool enabled = true;
        public static bool enableNailgunChanges = true;

        public ToolbotCore()
        {
            if (!enabled) return;

            NailgunChanges();

            //Titan kill times
            //Nailgun takes 26.14s
                //19s with Shift included
                //13.3s power mode

            //Rebar takes 65.8s
                //37.6 with quickswap

            //Scrap Launcher takes 37.8s
                //25.34s with quickswap
                //19.75s with power mode

            //Vanilla primaries are balanced around the assumption you will always take Nailgun. The either lack DPS for fulltime use, or have some crippling flaw.
            //Goals:
                //Make all primaries worth maining.
                //In terms of DPS goal is, Saw > Nailgun >= Scrap > Rebar
                //Make Scrap Launcher reload scale with attack speed
                //Give Saw barrier on hit.
                //Boost Rebar DPS so that it's not just a Wisp poking tool.
                //Find a way to make power mode Dual Nailgun weaker. Maybe a hardcoded AtkSpd reduction while in power mode could be warranted.
                //Fix Nailgun shotgun not triggering on most skill cancels.
        }

        private void NailgunChanges()
        {
            if (!enableNailgunChanges) return;
            SneedUtils.SneedUtils.SetEntityStateField("EntityStates.Toolbot.FireNailgun", "baseRefireInterval", "0.1");
            IL.EntityStates.Toolbot.BaseNailgunState.FireBullet += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BulletAttack>("Fire")
                     );
                c.EmitDelegate<Func<BulletAttack, BulletAttack>>(bulletAttack =>
                {
                    bulletAttack.radius = 0.2f;
                    bulletAttack.smartCollision = true;
                    return bulletAttack;
                });
            };
            new FixNailgunBurst();
        }
    }
}
