namespace RiskyMod.Items.Common
{
	using RoR2;
	using UnityEngine;
    using UnityEngine.AddressableAssets;
	using R2API;

    public class Fireworks
    {
		public static bool enabled = true;
		public static int maxRockets = 32;

		public static GameObject projectilePrefab;

        public Fireworks()
        {
			if (!enabled) return;

			if (Uncommon.ElementalBands.enabled)
			{
				projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Firework/FireworkProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyMod_Fireworks", true);
				DamageAPI.ModdedDamageTypeHolderComponent mdc = projectilePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
				mdc.Add(SharedDamageTypes.DontTriggerBands);
			}


			On.RoR2.FireworkLauncher.FixedUpdate += (orig, self) =>
			{
				SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
				if (sd && sd.baseSceneName.Equals("bazaar"))
				{
					Object.Destroy(self.gameObject);
					return;
				}
				else
				{
					if (self.remaining > maxRockets)
					{
						float newDamage = self.damageCoefficient * self.remaining / maxRockets;
						self.remaining = maxRockets;
						self.damageCoefficient = newDamage;
					}
					orig(self);
				}
			};
		}
	}
}
