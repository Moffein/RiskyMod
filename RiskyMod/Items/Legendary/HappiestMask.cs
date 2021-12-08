using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RiskyMod.SharedHooks;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Items.Legendary
{
    public class HappiestMask
    {
        public static bool enabled = true;
        public static BuffDef GhostCooldown;
        public static BuffDef GhostReady;

        public HappiestMask()
        {
            if (!enabled) return;
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.GhostOnKill);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.GhostOnKill);

            //Remove vanilla effect
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "GhostOnKill")
                    );
                c.Remove();
                c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));
            };

            GhostCooldown = ScriptableObject.CreateInstance<BuffDef>();
            GhostCooldown.buffColor = new Color(88f/255f, 91f/255f, 98f/255f);
            GhostCooldown.canStack = true;
            GhostCooldown.isDebuff = true;
            GhostCooldown.name = "RiskyMod_GhostCooldownDebuff";
            GhostCooldown.iconSprite = RoR2Content.Buffs.BanditSkull.iconSprite;
            BuffAPI.Add(new CustomBuff(GhostCooldown));

            GhostReady = ScriptableObject.CreateInstance<BuffDef>();
            GhostReady.buffColor = new Color(0.9f, 0.9f, 0.9f);
            GhostReady.canStack = false;
            GhostReady.isDebuff = false;
            GhostReady.name = "RiskyMod_GhostReadyBuff";
            GhostReady.iconSprite = RoR2Content.Buffs.BanditSkull.iconSprite;
            BuffAPI.Add(new CustomBuff(GhostReady));

            OnCharacterDeath.OnCharacterDeathInventoryActions += SpawnGhost;
            On.RoR2.CharacterBody.OnInventoryChanged += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    self.AddItemBehavior<GhostOnKillBehavior>(self.inventory.GetItemCount(RoR2Content.Items.GhostOnKill));
                }
            };
        }

        private static void SpawnGhost(GlobalEventManager self, DamageReport damageReport, CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody)
        {
            int itemCount = attackerInventory.GetItemCount(RoR2Content.Items.GhostOnKill);
            if (itemCount > 0)
            {
                if (attackerBody.HasBuff(GhostReady.buffIndex))
                {
                    Util.TryToCreateGhost(victimBody, attackerBody, itemCount * 30);
                    for (int i = 1; i <= 10; i++)
                    {
                        attackerBody.AddTimedBuff(GhostCooldown.buffIndex, i);
                    }
                }
            }
        }

        public class GhostOnKillBehavior : CharacterBody.ItemBehavior
        {
            private void FixedUpdate()
            {
                bool flag = this.body.HasBuff(GhostCooldown.buffIndex);
                bool flag2 = this.body.HasBuff(GhostReady.buffIndex);
                if (!flag && !flag2)
                {
                    this.body.AddBuff(GhostReady.buffIndex);
                }
                if (flag2 && flag)
                {
                    this.body.RemoveBuff(GhostReady.buffIndex);
                }
            }
        }
    }
}
