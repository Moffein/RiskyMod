using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskyMod.Survivors.DLC1.Railgunner
{
    class RailgunnerCore
    {
        public static bool enabled = true;
        public static bool fixBungus = true;

        public static bool slowFieldChanges = true;
        //public static GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/RailgunnerBody");

        public static float polarFieldDamageReduction = 0.333333333f;

        public RailgunnerCore()
        {
            if (!enabled) return;

            if (slowFieldChanges)
            {
                On.RoR2.Projectile.SlowDownProjectiles.OnTriggerEnter += (orig, self, other) =>
                {
                    if (self.gameObject.name == "RailgunnerMineAltDetonated(Clone)")
                    {
                        ProjectileDamage pd = other.GetComponent<ProjectileDamage>();
                        if (pd)
                        {
                            pd.damage *= polarFieldDamageReduction;
                        }
                    }
                    orig(self, other);
                };

                On.RoR2.Projectile.SlowDownProjectiles.OnTriggerExit += (orig, self, other) =>
                {
                    if (self.gameObject.name == "RailgunnerMineAltDetonated(Clone)")
                    {
                        ProjectileDamage pd = other.GetComponent<ProjectileDamage>();
                        if (pd && other.GetComponent<ProjectileSimple>())
                        {
                            pd.damage /= polarFieldDamageReduction;
                        }
                    }
                    orig(self, other);
                };
            }
        }
    }
}
