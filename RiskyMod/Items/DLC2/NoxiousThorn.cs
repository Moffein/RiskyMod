using MonoMod.Cil;
using RoR2;
using System;
using RoR2.Orbs;
using UnityEngine;

namespace RiskyMod.Items.DLC2
{
    public class NoxiousThorn
    {
        public static bool enabled = true;

        public NoxiousThorn()
        {
            if (!enabled) return;
            ItemsCore.ModifyItemDefActions += ModifyItem;
            IL.RoR2.CharacterBody.TriggerEnemyDebuffs += CharacterBody_TriggerEnemyDebuffs;
        }

        private void CharacterBody_TriggerEnemyDebuffs(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(x => x.MatchCallvirt<SphereSearch>("RefreshCandidates")))
            {
                c.EmitDelegate<Func<SphereSearch, SphereSearch>>(search =>
                {
                    search.radius = 25f;
                    return search;
                });
            }
            else
            {
                Debug.LogError("RiskyMod: NoxiousThorn IL hook failed.");
            }
        }

        private static void ModifyItem()
        {
            HG.ArrayUtils.ArrayAppend(ref ItemsCore.changedItemDescs, DLC2Content.Items.TriggerEnemyDebuffs);
        }
    }
}
