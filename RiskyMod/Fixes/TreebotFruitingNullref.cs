using MonoMod.Cil;
namespace RiskyMod.Fixes
{
    public class TreebotFruitingNullref
    {
        public TreebotFruitingNullref()
        {
            IL.RoR2.GlobalEventManager.OnCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchLdstr("Prefabs/Effects/TreebotFruitDeathEffect.prefab")
                    );
                c.Next.Operand = "Prefabs/Effects/TreebotFruitDeathEffect";
            };
        }
    }
}
