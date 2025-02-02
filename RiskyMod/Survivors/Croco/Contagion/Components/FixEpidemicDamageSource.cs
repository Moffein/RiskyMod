using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RiskyMod.Survivors.Croco.Contagion.Components
{
    public class FixEpidemicDamageSource : MonoBehaviour
    {
        private void Start()
        {
            ProjectileDamage pd = base.GetComponent<ProjectileDamage>();
            if (pd && pd.damageType.damageSource == DamageSource.Secondary)
            {
                pd.damageType.damageSource = DamageSource.Special;
            }
            Destroy(this);
        }
    }
}
