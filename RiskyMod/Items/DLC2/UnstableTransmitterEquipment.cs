using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace RiskyMod.Items.DLC2
{
    public class UnstableTransmitterEquipment
    {
        public static bool enabled;

        public UnstableTransmitterEquipment()
        {
            if (!enabled) return;
            DisableVanillaTransmitter();
        }

        private void DisableVanillaTransmitter()
        {
            //Prevent this from showing up without actually touching the Vanilla item.
            On.RoR2.UI.LogBook.LogBookController.CanSelectItemEntry += HideItemInLogbook;

            //Prevent it from being added to run's available items
            var item = Addressables.LoadAssetAsync<ItemDef>("43db984a982afff4485cd0420d8c18df").WaitForCompletion();
            var tags = item.tags.ToList();
            tags.Add(ItemTag.IgnoreForDropList);
            item.tags = tags.ToArray();

            //TODO: Suppress selection in Command.
        }

        private bool HideItemInLogbook(On.RoR2.UI.LogBook.LogBookController.orig_CanSelectItemEntry orig, ItemDef itemDef, Dictionary<RoR2.ExpansionManagement.ExpansionDef, bool> expansionAvailability)
        {
            var ret = orig(itemDef, expansionAvailability);

            if (itemDef == DLC2Content.Items.TeleportOnLowHealth)
            {
                return false;
            }

            return ret;
        }
    }
}
