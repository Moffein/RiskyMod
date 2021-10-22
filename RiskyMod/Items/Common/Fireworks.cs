namespace Risky_Mod.Items.Common
{
	using RoR2;
	using UnityEngine;
    public class Fireworks
    {
		public static bool enabled = true;
		public static int maxRockets = 32;

        public static void Modify()
        {
			if (!enabled) return;

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
