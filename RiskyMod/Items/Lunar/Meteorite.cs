using RoR2;
using System;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using UnityEngine;
using R2API;

namespace RiskyMod.Items.Lunar
{
    public class Meteorite
    {
        public static bool enabled = true;
        public Meteorite()
        {
            if (!enabled) return;

            IL.RoR2.MeteorStormController.DetonateMeteor += MeteorStormController_DetonateMeteor;
		}

        private void MeteorStormController_DetonateMeteor(ILContext il)
        {
			ILCursor c = new ILCursor(il);
			if (c.TryGotoNext(x => x.MatchCallvirt(typeof(BlastAttack), "Fire")))
			{
				c.EmitDelegate<Func<BlastAttack, BlastAttack>>(orig =>
				{
					orig.falloffModel = BlastAttack.FalloffModel.SweetSpot;
					orig.AddModdedDamageType(SharedDamageTypes.SweetSpotModifier);
                    return orig;
				});
			}
			else
			{
				Debug.LogError("RiskyMod: Meteor IL Hook failed");
			}
        }
    }
}
