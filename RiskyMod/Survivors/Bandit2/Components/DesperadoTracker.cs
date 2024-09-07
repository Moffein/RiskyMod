using RoR2;
using UnityEngine.Networking;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2.Components
{
    [RequireComponent(typeof(CharacterBody))]
    public class DesperadoTracker : MonoBehaviour
    {
        private CharacterBody characterBody;
        private DesperadoPersistComponent desperadoPersist;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
        }

        public void OnEnable()
        {
            if (!characterBody || !characterBody.master)
            {
                return;
            }
            if (NetworkServer.active)
            {
                desperadoPersist = characterBody.master.GetComponent<DesperadoPersistComponent>();
                if (desperadoPersist)
                {
                    while (characterBody.GetBuffCount(RoR2Content.Buffs.BanditSkull.buffIndex) < desperadoPersist.stacks)
                    {
                        characterBody.AddBuff(RoR2Content.Buffs.BanditSkull.buffIndex);
                    }
                }
                else
                {
                    desperadoPersist = characterBody.master.gameObject.AddComponent<DesperadoPersistComponent>();
                }
            }
        }

        public void FixedUpdate()
        {
            if (!characterBody || !desperadoPersist) return;
            if (NetworkServer.active && desperadoPersist && characterBody.healthComponent && characterBody.healthComponent.alive)
            {
                int desperadoCount = characterBody.GetBuffCount(RoR2Content.Buffs.BanditSkull.buffIndex);
                if (desperadoCount > desperadoPersist.stacks)
                {
                    desperadoPersist.stacks = desperadoCount;
                }
            }
        }
    }
}
