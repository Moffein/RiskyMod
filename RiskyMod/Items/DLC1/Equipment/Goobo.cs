using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace RiskyMod.Items.DLC1.Equipment
{
    public class Goobo
    {
        public static bool enabled = true;
        public Goobo()
        {
            if (!enabled) return;

            On.RoR2.EquipmentCatalog.Init += UpdateDesc;
            On.RoR2.Projectile.GummyCloneProjectile.SpawnGummyClone += SetStats;
            IL.RoR2.Projectile.GummyCloneProjectile.SpawnGummyClone += GiveAmbientLevel;
        }

        private void GiveAmbientLevel(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdloc(3)//MasterCopySpawnCard
                    ))
            {
                c.Emit(OpCodes.Ldloc_2);    //Projectile Owner's CharacterBody
                c.EmitDelegate<Func<MasterCopySpawnCard, CharacterBody, MasterCopySpawnCard>>((spawnCard, ownerBody) =>
                {
                    if (Goobo.enabled) spawnCard.GiveItem(RoR2Content.Items.UseAmbientLevel);   //Is this needed?

                    return spawnCard;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("RiskyMod: Goobo IL Hook failed");
            }
        }

        private void SetStats(On.RoR2.Projectile.GummyCloneProjectile.orig_SpawnGummyClone orig, RoR2.Projectile.GummyCloneProjectile self)
        {
            self.hpBoostCount = 50;
            self.damageBoostCount = 20;
            orig(self);
        }

        private void UpdateDesc(On.RoR2.EquipmentCatalog.orig_Init orig)
        {
            orig();
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, DLC1Content.Equipment.GummyClone);
        }
    }
}
