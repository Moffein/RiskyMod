using Mono.Cecil.Cil;
using MonoMod.Cil;
using RiskyMod.SharedHooks;
using RoR2;
using RoR2.Projectile;
using System;
using UnityEngine;

namespace RiskyMod.Items.DLC4.Boss
{
    public class MushShot
    {
        public static bool enabled;

        public MushShot()
        {
            if (!enabled) return;
            SharedHooks.LanguageModifiers.ModifyLanguageTokenActions += ModifyLang;
            IL.EntityStates.MushShot.MushShotFall.DoStompExplosionAuthority += MushShotFall_DoStompExplosionAuthority;
        }

        private void MushShotFall_DoStompExplosionAuthority(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);

            if (c.TryGotoNext(x => x.MatchCallvirt<ProjectileManager>("FireProjectile")))
            {
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<FireProjectileInfo, EntityStates.MushShot.MushShotFall, FireProjectileInfo>>((fpi, self) =>
                {
                    if (self.body && self.body.inventory)
                    {
                        int itemCount = Mathf.Max(1, self.body.inventory.GetItemCountEffective(DLC4Content.Items.MushShot));
                        float newDamage = self.body.damage * (10f + (6f * (itemCount - 1)));
                        fpi.damage = newDamage;
                    }

                    return fpi;
                });
            }
            else
            {
                Debug.LogError("RiskyMod: MushShot IL hook failed.");
            }
        }

        private void ModifyLang(LanguageModifiers.LanguageModifier langMod)
        {
            if (langMod.token == "ITEM_MUSHSHOT_DESC")
            {
                langMod.local = langMod.local.Replace("200", "600");
            }
        }
    }
}
