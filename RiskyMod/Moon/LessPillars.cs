using MonoMod.Cil;
using UnityEngine.Networking;
using RoR2;
using System;
using UnityEngine.Events;
using EntityStates.Missions.Moon;
using EntityStates.MoonElevator;

namespace RiskyMod.Moon
{
    public class LessPillars
    {
        public static bool enabled = true;
        public LessPillars()
        {
            if (!enabled) return;
            On.RoR2.MoonBatteryMissionController.Awake += (orig, self) =>
            {
                orig(self);
				self._numRequiredBatteries = 2;
			};

            //Prevent pillars from deactivating
            On.RoR2.MoonBatteryMissionController.OnBatteryCharged += (orig, self, holdoutZone) =>
            {
				self.Network_numChargedBatteries = self._numChargedBatteries + 1;
				if (self._numChargedBatteries >= self._numRequiredBatteries && NetworkServer.active)
				{
					for (int i = 0; i < self.batteryHoldoutZones.Length; i++)
					{
						if (self.batteryHoldoutZones[i].enabled)
						{
							self.batteryHoldoutZones[i].FullyChargeHoldoutZone();
							self.batteryHoldoutZones[i].onCharged.RemoveListener(new UnityAction<HoldoutZoneController>(self.OnBatteryCharged));
						}
					}
					/*self.batteryHoldoutZones = new HoldoutZoneController[0];
					for (int j = 0; j < self.batteryStateMachines.Length; j++)
					{
						if (!(self.batteryStateMachines[j].state is MoonBatteryComplete))
						{
							self.batteryStateMachines[j].SetNextState(new MoonBatteryDisabled());
						}
					}*/
					for (int k = 0; k < self.elevatorStateMachines.Length; k++)
					{
						self.elevatorStateMachines[k].SetNextState(new InactiveToReady());
					}
				}
			};
        }
    }
}
