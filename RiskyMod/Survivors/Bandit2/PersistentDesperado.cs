using MonoMod.Cil;
using RiskyMod.Survivors.Bandit2.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RiskyMod.Survivors.Bandit2
{
    public class PersistentDesperado
    {
        public static bool enabled = true;
        public static float damagePerBuff = 0.01f;

        public PersistentDesperado()
        {
            if (!enabled) return;

            On.RoR2.CharacterBody.OnClientBuffsChanged += CharacterBody_OnClientBuffsChanged;
            On.RoR2.CharacterBody.Start += CharacterBody_Start;

            IL.EntityStates.Bandit2.Weapon.FireSidearmSkullRevolver.ModifyBullet += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if(c.TryGotoNext(
                     x => x.MatchLdcR4(0.1f)
                    ))
                {
                    c.Next.Operand = damagePerBuff;
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: PersistentDesperado IL Hook failed");
                }
            };
        }

        private void CharacterBody_Start(On.RoR2.CharacterBody.orig_Start orig, CharacterBody self)
        {
            orig(self);
            UpdateDesperado(self);
        }

        private void CharacterBody_OnClientBuffsChanged(On.RoR2.CharacterBody.orig_OnClientBuffsChanged orig, CharacterBody self)
        {
            orig(self);
            UpdateDesperado(self);
        }

        private void UpdateDesperado(CharacterBody self)
        {
            if (self.bodyIndex == Bandit2Core.Bandit2Index && self.master)
            {
                DesperadoTracker dt = self.master.GetComponent<DesperadoTracker>();
                if (!dt) dt = self.master.gameObject.AddComponent<DesperadoTracker>();
                if (dt)
                {
                    int current = self.GetBuffCount(RoR2Content.Buffs.BanditSkull);
                    if (current > dt.count)
                    {
                        dt.count = current;
                    }
                    else
                    {
                        int diff = dt.count - current;
                        for (int i = 0; i < diff; i++)
                        {
                            self.AddBuff(RoR2Content.Buffs.BanditSkull);
                        }
                    }
                }
            }
        }

        public static float GetDesperadoMult(CharacterBody cb)
        {
            int buffCount = cb.GetBuffCount(RoR2Content.Buffs.BanditSkull.buffIndex);
            if (PersistentDesperado.enabled)
            {
                return 1f + buffCount * damagePerBuff;
            }
            else
            {
                return 1f + buffCount * 0.1f;
            }
        }
    }

    public class DesperadoTracker : MonoBehaviour
    {
        public int count = 0;
    }
}
