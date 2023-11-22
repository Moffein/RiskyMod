using RoR2;
using UnityEngine.Networking;
using UnityEngine;

namespace RiskyMod.Survivors.Bandit2.Components
{
    [RequireComponent(typeof(CharacterBody))]
    public class DesperadoTracker : MonoBehaviour
    {
        private CharacterBody characterBody;
        private DesperadoPersist desperadoPersist;
        public void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            if (!characterBody) Destroy(this);
        }

        public void Start()
        {
            if (NetworkServer.active && characterBody.master)
            {
                desperadoPersist = characterBody.master.GetComponent<DesperadoPersist>();
                if (desperadoPersist)
                {
                    while (characterBody.GetBuffCount(RoR2Content.Buffs.BanditSkull.buffIndex) < desperadoPersist.stacks)
                    {
                        characterBody.AddBuff(RoR2Content.Buffs.BanditSkull.buffIndex);
                    }
                }
                else
                {
                    desperadoPersist = characterBody.master.gameObject.AddComponent<DesperadoPersist>();
                }
            }
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active && characterBody.healthComponent && characterBody.healthComponent.alive)
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
