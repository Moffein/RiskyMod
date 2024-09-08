using RoR2;
using UnityEngine;

namespace RiskyMod.Survivors.Croco2.Contagion
{

    public class EpidemicVFXController : MonoBehaviour
    {
        CharacterBody body;
        BurnEffectController burnEffectController;

        private void Awake()
        {
            body = GetComponent<CharacterBody>();
            burnEffectController = null;
            if (!body)
            {
                Destroy(this);
            }
        }

        private void FixedUpdate()
        {
            if (body)
            {
                if (body.HasBuff(ModifySpecial.EpidemicDebuff.buffIndex))
                {
                    if (!burnEffectController)
                    {
                        if (body.modelLocator && body.modelLocator.modelTransform)
                        {
                            burnEffectController = gameObject.AddComponent<BurnEffectController>();
                            burnEffectController.effectType = BurnEffectController.poisonEffect;
                            burnEffectController.target = body.modelLocator.modelTransform.gameObject;
                        }
                    }
                }
                else
                {
                    if (burnEffectController)
                    {
                        Destroy(burnEffectController);
                        burnEffectController = null;
                    }
                }
            }
            else
            {
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (burnEffectController)
            {
                Destroy(burnEffectController);
                burnEffectController = null;
            }
        }
    }
}
