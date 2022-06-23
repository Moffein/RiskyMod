using UnityEngine;
using RoR2;
using RoR2.Orbs;
using UnityEngine.Networking;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace RiskyMod.Items.Legendary
{
    public class Tesla
    {
        public static bool enabled = true;
        public Tesla()
        {
            if (!enabled) return;

            IL.RoR2.Items.ShockNearbyBodyBehavior.FixedUpdate += (il) =>
            {
                bool error = false;
                ILCursor c = new ILCursor(il);
                
                if (RiskyMod.disableProcChains)
                {
                    if(c.TryGotoNext(
                        x => x.MatchStfld<RoR2.Orbs.LightningOrb>("procCoefficient")
                       ))
                    {
                        c.Index--;
                        c.Next.Operand = 0f;
                    }
                    else
                    {
                        error = true;
                    }
                }

                if (c.TryGotoNext(
                     x => x.MatchStfld<RoR2.Orbs.LightningOrb>("range")
                    ))
                {
                    c.Index--;
                    c.Next.Operand = 20f;
                }
                else
                {
                    error = true;
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Tesla IL Hook failed");
                }
            };
        }
    }
}
