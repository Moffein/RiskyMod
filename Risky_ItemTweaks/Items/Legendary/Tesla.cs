using UnityEngine;
using RoR2;
using RoR2.Orbs;
using UnityEngine.Networking;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace Risky_Mod.Items.Legendary
{
    public class Tesla
    {
        public static bool enabled = true;
        public static void Modify()
        {
            if (!enabled) return;

            IL.RoR2.CharacterBody.UpdateTeslaCoil += (il) =>
            {
                ILCursor c = new ILCursor(il);
                
                if (Risky_Mod.disableProcChains)
                {
                    c.GotoNext(
                        x => x.MatchStfld<RoR2.Orbs.LightningOrb>("procCoefficient")
                       );
                    c.Index--;
                    c.Next.Operand = 0f;
                }

                c.GotoNext(
                     x => x.MatchStfld<RoR2.Orbs.LightningOrb>("range")
                    );
                c.Index--;
                c.Next.Operand = 20f;
            };

            On.RoR2.ItemCatalog.Init += (orig) =>
			{
				orig();
				Risky_Mod.AddToAIBlacklist(RoR2Content.Items.ShockNearby.itemIndex);
			};
		}
    }
}
