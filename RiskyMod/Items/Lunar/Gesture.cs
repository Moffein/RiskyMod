using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.Artifacts;
using System;

namespace RiskyMod.Items.Lunar
{
    public class Gesture
    {
        public static bool enabled = true;
        public Gesture()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;

            IL.RoR2.EquipmentSlot.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Items), "AutoCastEquipment")
                    ))
                {
                    //Why doesn't this work here?
                    //c.Remove();
                    //c.Emit<RiskyMod>(OpCodes.Ldsfld, nameof(RiskyMod.emptyItemDef));

                    c.Index += 2;
                    c.EmitDelegate<Func<int, int>>(orig => 0);
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Gesture IL Hook failed");
                }
            };

            EquipmentSlot.onServerEquipmentActivated += EquipmentSlot_onServerEquipmentActivated;
        }

        private static void EquipmentSlot_onServerEquipmentActivated(EquipmentSlot equipmentSlot, EquipmentIndex equipmentIndex)
        {
            if (equipmentSlot.characterBody.inventory.GetItemCount(RoR2Content.Items.AutoCastEquipment) > 0)
            {
                EnigmaArtifactManager.OnServerEquipmentActivated(equipmentSlot, equipmentIndex);
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, RoR2Content.Items.AutoCastEquipment);
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemPickups, RoR2Content.Items.AutoCastEquipment);
        }
    }
}
