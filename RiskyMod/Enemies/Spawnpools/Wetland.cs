using R2API;
using System.Collections.Generic;

namespace RiskyMod.Enemies.Spawnpools
{
    public class Wetland
    {
        public static bool enabled = true;
        public Wetland()
        {
            if (!enabled) return;

            DirectorAPI.Helpers.RemoveExistingMonsterFromStage(DirectorAPI.Helpers.MonsterNames.LesserWisp, DirectorAPI.Stage.WetlandAspect);
        }
    }
}
