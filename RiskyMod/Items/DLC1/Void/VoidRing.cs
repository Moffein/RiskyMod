using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Void
{
    class VoidRing
    {
        public static bool enabled = true;
        public VoidRing()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            GameObject prefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/ElementalRingVoidBlackHole");
            ProjectileExplosion pe = prefab.GetComponent<ProjectileExplosion>();
            pe.falloffModel = BlastAttack.FalloffModel.None;
            pe.blastProcCoefficient = 0f;   //Not sure if this is even needed

            IL.RoR2.GlobalEventManager.OnHitEnemy += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(DLC1Content.Buffs), "ElementalRingVoidReady")
                    );

                //Reduce cooldown
                c.GotoNext(
                     x => x.MatchLdcR4(20f)
                    );
                c.Next.Operand = 15f;
            };
        }

        private void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC1Content.Items.ElementalRingVoid);
        }
    }
}
