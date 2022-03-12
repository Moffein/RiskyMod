using RoR2;
using RoR2.Artifacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Tweaks
{
    public class EnigmaBlacklist
    {
        public static bool enabled = true;
        public EnigmaBlacklist()
        {
            if (!enabled) return;

            On.RoR2.Artifacts.EnigmaArtifactManager.OnRunStartGlobal += (orig, run) =>
            {
                orig(run);

                List<EquipmentIndex> toRemove = new List<EquipmentIndex>();
                foreach (EquipmentIndex i in EnigmaArtifactManager.validEquipment)
                {
                    EquipmentDef ed = EquipmentCatalog.GetEquipmentDef(i);
                    if (ed)
                    {
                        if (ed.isLunar)
                        {
                            toRemove.Add(i);
                        }
                    }
                }

                foreach (EquipmentIndex i in toRemove)
                {
                    EnigmaArtifactManager.validEquipment.Remove(i);
                }

                EnigmaArtifactManager.validEquipment.Remove(RoR2Content.Equipment.Recycle.equipmentIndex);
            };
        }
    }
}
