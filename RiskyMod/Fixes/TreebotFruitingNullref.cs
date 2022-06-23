using MonoMod.Cil;
using UnityEngine;
namespace RiskyMod.Fixes
{
    public class TreebotFruitingNullref
    {
        public TreebotFruitingNullref()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdstr("Prefabs/Effects/TreebotFruitDeathEffect.prefab")
                    ))
                {
                    c.Next.Operand = "Prefabs/Effects/TreebotFruitDeathEffect";
                }
                else
                {
                    Debug.LogError("RiskyMod: TreebotFruitingNullref IL Hook failed");
                }
            };
        }
    }
}
