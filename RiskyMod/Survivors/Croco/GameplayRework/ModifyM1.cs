using RoR2;
using R2API;
using RoR2.Projectile;
using UnityEngine;
using RoR2.Orbs;
using System.Collections.Generic;
using System.Linq;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyM1
    {
        public ModifyM1()
        {
            On.EntityStates.Croco.Slash.AuthorityModifyOverlapAttack += (orig, self, overlapAttack) =>
            {
                orig(self, overlapAttack);
                if (self.isComboFinisher)
                {
                    overlapAttack.damageType = DamageType.PoisonOnHit;
                }
            };

            On.EntityStates.Croco.Slash.OnMeleeHitAuthority += (orig, self) =>
            {
                orig(self);

                if (self.isComboFinisher)
                {
                    CrocoDamageTypeController cd = self.GetComponent<CrocoDamageTypeController>();
                    if (cd && cd.GetDamageType() == DamageType.BlightOnHit)
                    {
                        if (self.characterBody)
                        {
                            CharacterBody body = self.characterBody;
                            DamageType damageType = DamageType.PoisonOnHit;

                            LightningOrb lightningOrb = new LightningOrb();
                            lightningOrb.bouncedObjects = new List<HealthComponent>();
                            lightningOrb.targetsToFindPerBounce = 1;
                            lightningOrb.canBounceOnSameTarget = false;
                            lightningOrb.attacker = body.gameObject;
                            lightningOrb.inflictor = body.gameObject;
                            lightningOrb.teamIndex = body.teamComponent.teamIndex;
                            lightningOrb.damageValue = 1f;
                            lightningOrb.isCrit = false;
                            lightningOrb.origin = body.corePosition;
                            lightningOrb.bouncesRemaining = 3;
                            lightningOrb.lightningType = LightningOrb.LightningType.CrocoDisease;
                            lightningOrb.procCoefficient = 1f;
                            lightningOrb.target = lightningOrb.PickNextTarget(body.corePosition);
                            lightningOrb.damageColorIndex = DamageColorIndex.Poison;
                            lightningOrb.damageType = damageType;
                            lightningOrb.procCoefficient = 1f;
                            OrbManager.instance.AddOrb(lightningOrb);
                        }
                    }
                }
            };
        }
    }
}
