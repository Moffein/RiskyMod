using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.Legendary
{
    public class Headstompers
    {
        public static bool enabled = true;

        public static BuffDef HeadstompersActive;
        public static BuffDef HeadstompersPending;
        public static BuffDef HeadstompersCooldown;

        public Headstompers()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            Headstompers.HeadstompersActive = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyModHeadstompersActive",
                false,
                false,
                false,
                Color.white,
                Content.Assets.BuffIcons.HeadstomperActive
                );

            Headstompers.HeadstompersPending = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyModHeadstompersPending",
                false,
                false,
                false,
                Color.white,
                Content.Assets.BuffIcons.HeadstomperActive
                );

            Headstompers.HeadstompersCooldown = SneedUtils.SneedUtils.CreateBuffDef(
                "RiskyModHeadstompersCooldown",
                true,
                true,
                false,
                new Color(0.8f, 0.8f, 0.8f),
                Content.Assets.BuffIcons.HeadstomperCooldown
                );

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

            SetupCooldownBehavior();

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

        private void SetupCooldownBehavior()
        {
            On.EntityStates.Headstompers.HeadstompersCooldown.OnEnter += (orig, self) =>
            {
                orig(self);
                self.duration = EntityStates.Headstompers.HeadstompersCooldown.baseDuration;
                if (NetworkServer.active && self.body)
                {
                    if (self.body.HasBuff(HeadstompersActive)) self.body.RemoveBuff(HeadstompersActive);
                    if (self.body.HasBuff(HeadstompersPending)) self.body.RemoveBuff(HeadstompersPending);

                    int buffCount = 0;
                    float durationCount = self.duration;
                    while (durationCount > 0f)
                    {
                        float newDuration = 1f;
                        if (durationCount > newDuration)
                        {
                            durationCount -= newDuration;
                        }
                        else
                        {
                            newDuration = durationCount;
                            durationCount = 0f;
                        }

                        float buffDuration = buffCount + newDuration;
                        self.body.AddTimedBuff(HeadstompersCooldown, buffDuration);
                        buffCount++;
                    }
                }
            };

            //Handle Blast Shower
            On.EntityStates.Headstompers.HeadstompersCooldown.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (self.body)
                {
                    if (!self.body.HasBuff(HeadstompersCooldown))
                    {
                        if (self.isAuthority)
                        {
                            if (self.body.HasBuff(HeadstompersPending))
                            {
                                self.outer.SetNextState(new EntityStates.Headstompers.HeadstompersIdle());
                            }
                        }
                        else if (NetworkServer.active)
                        {
                            if (!self.body.HasBuff(HeadstompersPending))
                            {
                                self.body.AddBuff(HeadstompersPending);
                            }
                        }
                    }
                    else
                    {
                        if (NetworkServer.active && self.body.HasBuff(HeadstompersActive)) self.body.RemoveBuff(HeadstompersActive);
                    }
                }
            };

            On.EntityStates.Headstompers.BaseHeadstompersState.OnExit += (orig, self) =>
            {
                if (NetworkServer.active && self.body)
                {
                    if (self.body.HasBuff(HeadstompersCooldown)) self.body.ClearTimedBuffs(HeadstompersCooldown);
                    if (self.body.HasBuff(HeadstompersActive)) self.body.RemoveBuff(HeadstompersActive);
                    if (self.body.HasBuff(HeadstompersPending)) self.body.RemoveBuff(HeadstompersPending);
                }
                orig(self);
            };

            On.EntityStates.Headstompers.HeadstompersIdle.FixedUpdate += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active && self.body && self.body.inventory)
                {
                    if (self.body.inventory.GetItemCount(RoR2Content.Items.FallBoots) > 0)
                    {
                        if (!self.body.HasBuff(HeadstompersActive)) self.body.AddBuff(HeadstompersActive);
                    }
                    else
                    {
                        if (self.body.HasBuff(HeadstompersActive)) self.body.RemoveBuff(HeadstompersActive);
                    }
                }
            };
        }
        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.FallBoots);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.FallBoots);
        }
    }
}
