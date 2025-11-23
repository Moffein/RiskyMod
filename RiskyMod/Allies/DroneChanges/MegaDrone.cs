using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Allies.DroneChanges
{
    public class MegaDrone
    {
		public static bool allowRepair = true;

		public MegaDrone()
		{
			GameObject megaDroneBodyObject = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/MegaDroneBody.prefab").WaitForCompletion();

            
			if (allowRepair)
            {
				CharacterDeathBehavior cdb = megaDroneBodyObject.GetComponent<CharacterDeathBehavior>();
				//cdb.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Drone.DeathState));
				cdb.deathState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.RiskyMod.MegaDrone.MegaDroneDeathState));
			}
			Content.Content.entityStates.Add(typeof(EntityStates.RiskyMod.MegaDrone.MegaDroneDeathState));
		}
	}
}
