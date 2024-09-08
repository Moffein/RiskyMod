using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;
using UnityEngine;

namespace RiskyMod.Survivors.Croco.Contagion.Components
{
    public class EpidemicDamageOverrideComponent : MonoBehaviour
    {
        public DamageAPI.ModdedDamageType damageType;
        private void Start()
        {
            if (!NetworkServer.active) return;
            ProjectileController pc = base.GetComponent<ProjectileController>();
            if (!pc || !pc.owner) return;
            CharacterBody ownerBody = pc.owner.GetComponent<CharacterBody>();
            if (!ownerBody || !ownerBody.skillLocator) return;
            if (!ContagionPassive.HasPassive(ownerBody.skillLocator)) return;
            ProjectileDamage pd = base.GetComponent<ProjectileDamage>();
            if (pd) pd.damageType = DamageType.Generic;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = base.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            if (!mdc) mdc = base.gameObject.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(damageType);
        }
    }
}
