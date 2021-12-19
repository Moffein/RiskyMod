using RoR2;
using UnityEngine;
using R2API;

namespace RiskyMod.Survivors.Croco
{
    public class ModifyPassives
    {
        public ModifyPassives()
        {
            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                bool isPoison = (damageInfo.damageType & DamageType.PoisonOnHit) == DamageType.PoisonOnHit;
                bool isBlight = (damageInfo.damageType & DamageType.BlightOnHit) == DamageType.BlightOnHit;
                if (isPoison || isBlight)
                {
                    if (damageInfo.attacker)
                    {
                        CrocoDamageTypeController cd = damageInfo.attacker.GetComponent<CrocoDamageTypeController>();
                        if (cd)
                        {
                            switch (cd.GetDamageType())
                            {
                                case DamageType.PoisonOnHit:    //Passive: Extended poison duration
                                    if (isBlight)
                                    {
                                        damageInfo.damageType &= ~DamageType.BlightOnHit;
                                        damageInfo.AddModdedDamageType(SharedDamageTypes.Blight7s);
                                    }
                                    break;
                                case DamageType.BlightOnHit:    //Passive: Poison spread on application
                                    if (isPoison)
                                    {
                                        damageInfo.damageType &= ~DamageType.PoisonOnHit;
                                        damageInfo.AddModdedDamageType(SharedDamageTypes.Poison7s);
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                orig(self, damageInfo);

            };
        }
    }
}
