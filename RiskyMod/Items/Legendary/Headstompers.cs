using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.Legendary
{
    public class Headstompers
    {
        public static bool enabled = true;
        public Headstompers()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.FallBoots);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.FallBoots);

            On.EntityStates.Headstompers.BaseHeadstompersState.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.bodyMotor) self.bodyMotor.airControl = 1f; //Shouldnt this be adding to the bodyMotor, instead of assigning the value? Just wondering
            };

            On.EntityStates.Headstompers.BaseHeadstompersState.OnExit += (orig, self) =>
            {
                if (self.bodyMotor) self.bodyMotor.airControl = 0.25f; //Same here
                orig(self);
            };

            //Damage per stack
            IL.EntityStates.Headstompers.HeadstompersFall.DoStompExplosionAuthority += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<RoR2.BlastAttack>("Fire")
                    );
                c.Emit(OpCodes.Ldloc_0);   //inventory
                c.EmitDelegate<Func<BlastAttack, Inventory, BlastAttack>>((blastAttack, inventory) =>
                {
                    int itemCount = inventory.GetItemCount(RoR2Content.Items.FallBoots);
                    blastAttack.baseDamage += blastAttack.baseDamage * 0.6f * (itemCount - 1);
                    return blastAttack;
                });
            };
        }
    }
}
