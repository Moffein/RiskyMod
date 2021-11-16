using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using MonoMod.Cil;
using System;
using System.Runtime.CompilerServices;
using RoR2.CharacterAI;

namespace RiskyMod.Drones
{
    public class DronesCore
    {
        public static bool enabled = true;
        public DronesCore()
        {
            if (!enabled) return;
            FixBackupScaling();
            new DroneScaling();
            new DroneTargeting();
            new IncreaseShotRadius();
            TweakDrones();
        }

        private void TweakDrones()
        {
            new MegaDrone();
        }

        //Makes backup drones scale with ambient level like all other drones.
        private void FixBackupScaling()
        {
            IL.RoR2.EquipmentSlot.FireDroneBackup += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                    x => x.MatchStfld<MasterSuicideOnTimer>("lifeTimer")
                    );
                c.Index -= 7;
                c.EmitDelegate<Func<CharacterMaster, CharacterMaster>>((master) =>
                {
                    if (master.inventory)
                    {
                        master.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                    }
                    return master;
                });
            };
        }
    }
}
