using RoR2;
using RoR2.Artifacts;
using System.Collections.Generic;

namespace RiskyMod.Fixes
{
    public class FixEnigmaBlacklist
    {
        public static bool enabled = true;
        public FixEnigmaBlacklist()
        {
            if (!enabled) return;
            On.RoR2.Artifacts.EnigmaArtifactManager.OnRunStartGlobal += (orig, run) =>
            {
                orig(run);

                //EnigmaArtifactManager.validEquipment
                List<EquipmentIndex> toRemove = new List<EquipmentIndex>();
                foreach (EquipmentIndex ei in EnigmaArtifactManager.validEquipment)
                {
                    if (!Run.instance.availableEquipment.Contains(ei))
                    {
                        toRemove.Add(ei);
                    }
                }

                foreach (EquipmentIndex ei in toRemove)
                {
                    EnigmaArtifactManager.validEquipment.Remove(ei);
                }
            };
        }
    }
}
