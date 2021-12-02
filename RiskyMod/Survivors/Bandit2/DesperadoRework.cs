using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Bandit2
{
    public class DesperadoRework
    {
        public static bool enabled = true;
        public static float damagePerBuff = 0.02f;

        public DesperadoRework()
        {
            if (!enabled) return;
            RoR2Content.Survivors.Bandit2.bodyPrefab.AddComponent<DesperadoTracker>();

            IL.EntityStates.Bandit2.Weapon.FireSidearmSkullRevolver.ModifyBullet += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdcR4(0.1f)
                    );
                c.Next.Operand = damagePerBuff;
            };
        }

        public static float GetDesperadoMult(CharacterBody cb)
        {
            int buffCount = cb.GetBuffCount(RoR2Content.Buffs.BanditSkull.buffIndex);
            if (!DesperadoRework.enabled)
            {
                return 1f + buffCount * 0.1f;
            }
            else
            {
                return 1f + buffCount * damagePerBuff;
            }
        }
    }

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
            if (NetworkServer.active)
            {
                int desperadoCount = characterBody.GetBuffCount(RoR2Content.Buffs.BanditSkull.buffIndex);
                if (desperadoCount > desperadoPersist.stacks)
                {
                    desperadoPersist.stacks = desperadoCount;
                }
            }
        }
    }
    public class DesperadoPersist : MonoBehaviour
    {
        public int stacks = 0;
    }
}
