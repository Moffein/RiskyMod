

using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;

namespace RiskyMod.Fixes
{
    public class RemoveLevelCap
    {
        public static bool enabled = true;
		public static float maxLevel = 1000f;
		public static float stopSound = 300f;
        public RemoveLevelCap()
        {
			//Remove level capping when calculating monster level
			IL.RoR2.Run.RecalculateDifficultyCoefficentInternal += (il) =>
			{
				ILCursor c = new ILCursor(il);
				c.GotoNext(
					x => x.MatchLdsfld<Run>(nameof(Run.ambientLevelCap))
					);
				c.Remove();
				c.Emit<RemoveLevelCap>(OpCodes.Ldsfld, nameof(RemoveLevelCap.maxLevel));
			};

			On.RoR2.LevelUpEffectManager.OnRunAmbientLevelUp += (orig, run) =>
			{
				if (run.ambientLevel <= stopSound)
				{
					orig(run);
				}
			};
		}
    }
}
