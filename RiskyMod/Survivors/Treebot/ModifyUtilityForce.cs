using MonoMod.Cil;
using Mono.Cecil.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Survivors.Treebot
{
    public class ModifyUtilityForce
    {
        public static bool enabled = true;
        public ModifyUtilityForce()
        {
            if (!enabled) return;
            IL.EntityStates.Treebot.Weapon.FireSonicBoom.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);

                //Make BullseyeSearch start slightly behind you
                c.GotoNext(
                     x => x.MatchCallvirt<BullseyeSearch>("RefreshCandidates")
                    );
                c.EmitDelegate<Func<BullseyeSearch, BullseyeSearch>>(bullseye =>
                {
                    bullseye.searchOrigin -= bullseye.searchDirection;
                    bullseye.maxDistanceFilter += 1f;
                    return bullseye;
                });

                //Completely overwrite the vanilla force settings.
                c.GotoNext(
                     x => x.MatchCallvirt<HealthComponent>("TakeDamageForce")
                    );

                //Modify Force direction
                c.Index -= 6;
                c.Emit(OpCodes.Ldarg_0);   //FireSonicBoom
                c.Emit(OpCodes.Ldloc, 4);   //Hurtbox
                c.EmitDelegate<Func<Vector3, EntityStates.Treebot.Weapon.FireSonicBoom, HurtBox, Vector3>>((origForce, self, hurtBox) =>
                {
                    Ray aimRay = self.GetAimRay();
                    Vector3 newForce = 2800f * aimRay.direction;
                    CharacterBody body = hurtBox.healthComponent.body;
                    if (body.characterMotor && body.characterMotor.isGrounded)
                    {
                        newForce.y = Mathf.Max(newForce.y, 1200f);
                    }

                    Vector3 distance = hurtBox.transform.position - aimRay.origin;
                    float distMagnitude = distance.magnitude;

                    float distMult = Mathf.Lerp(1.1f, 0.2f, distance.magnitude / (self.maxDistance + 1f));

                    if (body.isChampion && body.characterMotor && body.characterMotor.isGrounded)
                    {
                        newForce.y /= 0.7f;
                    }

                    return newForce * distMult;
                });

                //Modify Force multiplier
                c.Index += 3;
                c.Emit(OpCodes.Ldloc, 4);   //Hurtbox
                c.EmitDelegate<Func<float, HurtBox, float>>((origForceMult, hurtBox) =>
                {
                    float mass = 1f;
                    CharacterBody body = hurtBox.healthComponent.body;
                    if (body.characterMotor)
                    {
                        mass = body.characterMotor.mass;
                    }
                    else if (body.rigidbody)
                    {
                        mass = body.rigidbody.mass;
                    }

                    float forceMult = Mathf.Max(mass / 100f, 1f);

                    if (body.isChampion && body.characterMotor && body.characterMotor.isGrounded)
                    {
                        forceMult *= 0.7f;
                    }

                    return forceMult;
                });
            };
        }
    }
}
