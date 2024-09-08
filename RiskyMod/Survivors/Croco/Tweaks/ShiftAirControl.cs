using EntityStates.Croco;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using UnityEngine;

namespace RiskyMod.Survivors.Croco.Tweaks
{
    public class ShiftAirControl
    {
        public static bool enabled = true;
        public ShiftAirControl()
        {
            if (!enabled) return;
            IL.EntityStates.Croco.BaseLeap.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchStfld<RoR2.CharacterMotor>("airControl")
                    ))
                {
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<float, BaseLeap, float>>((airControl, self) =>
                    {
                        //Debug.Log(airControl);//0.15
                        if (self.characterBody)
                        {
                            float moveSpeedCoeff = self.characterBody.moveSpeed / (self.characterBody.baseMoveSpeed * (!self.characterBody.isSprinting ? 1f : self.characterBody.sprintingSpeedMultiplier));
                            moveSpeedCoeff = Mathf.Min(moveSpeedCoeff, 3f);
                            airControl *= moveSpeedCoeff;
                        }
                        return airControl;
                    });
                }
                else
                {
                    Debug.LogError("RiskyMod: Croco ShiftAirControl IL Hook failed");
                }
            };
        }
    }
}
