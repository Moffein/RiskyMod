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
            if (enabled)
            {
                On.RoR2.EquipmentCatalog.Init += (orig) =>
                {
                    orig();
                    HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedEquipDescs, DLC1Content.Equipment.GummyClone);
                };

                On.RoR2.Projectile.GummyCloneProjectile.SpawnGummyClone += (orig, self) =>
                {
                    self.hpBoostCount = 50;
                    self.damageBoostCount = 20;
                    orig(self);
                };
            }

            IL.RoR2.Projectile.GummyCloneProjectile.SpawnGummyClone += (il) =>
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
                        if (ownerBody.teamComponent && ownerBody.teamComponent.teamIndex == TeamIndex.Player)
                        {
                            spawnCard.GiveItem(Allies.AllyItems.AllyMarkerItem);
                            //spawnCard.GiveItem(Allies.AllyItems.AllyScalingItem);
                            //spawnCard.GiveItem(Allies.AllyItems.AllyRegenItem, 40);
                            spawnCard.GiveItem(Allies.AllyItems.AllyAllowOverheatDeathItem);
                            spawnCard.GiveItem(Allies.AllyItems.AllyAllowVoidDeathItem);
                        }
                        return spawnCard;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Goobo IL Hook failed");
                }
            };
        }
    }
}
