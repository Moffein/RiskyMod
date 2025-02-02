namespace RiskyMod.Items.Common
{
	using RoR2;
	using UnityEngine;
    using UnityEngine.AddressableAssets;
	using R2API;
    using RoR2.Projectile;

    public class Fireworks
    {
		public static bool enabled = true;
		public static int maxRockets = 32;

		public static GameObject projectilePrefab;

        public Fireworks()
        {
			if (!enabled) return;

			projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Firework/FireworkProjectile.prefab").WaitForCompletion().InstantiateClone("RiskyMod_Fireworks", true);
			ProjectileDamage pd = projectilePrefab.GetComponent<ProjectileDamage>();
			pd.damageType.AddModdedDamageType(SharedDamageTypes.DontTriggerBands);

			Content.Content.projectilePrefabs.Add(projectilePrefab);

			On.RoR2.FireworkLauncher.FixedUpdate += (orig, self) =>
			{
				self.projectilePrefab = Fireworks.projectilePrefab;	//Need to find a better place to put this
				if (RiskyMod.inBazaar)
				{
					Object.Destroy(self.gameObject);
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
