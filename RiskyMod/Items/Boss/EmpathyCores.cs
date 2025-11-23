using RoR2;
using UnityEngine;
using System;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Items.Boss
{
    public class EmpathyCores
    {
        public static bool enabled = true;
        public static bool ignoreAllyCap = true;

        public static SpawnCard RedBuddyCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/RoboBallBuddy/cscRoboBallRedBuddy.asset").WaitForCompletion();
        public static SpawnCard GreenBuddyCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/RoboBallBuddy/cscRoboBallGreenBuddy.asset").WaitForCompletion();

        public EmpathyCores()
        {
            if (!enabled) return;
            IL.RoR2.DeployableMinionSpawner.SpawnMinion += DeployableMinionSpawner_SpawnMinion;
        }

        private void DeployableMinionSpawner_SpawnMinion(ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(
                 x => x.MatchCallvirt<DirectorCore>("TrySpawnObject")
                ))
            {
                c.Emit(OpCodes.Ldarg_0);    //DeployableMinionSpawner (self)
                c.Emit(OpCodes.Ldarg_1);    //SpawnCard
                c.EmitDelegate<Func<DirectorSpawnRequest, DeployableMinionSpawner, SpawnCard, DirectorSpawnRequest>>((directorSpawnRequest, self, spawnCard) =>
                {
                    if (spawnCard == RedBuddyCard || spawnCard == GreenBuddyCard)
                    {
                        directorSpawnRequest.ignoreTeamMemberLimit = EmpathyCores.ignoreAllyCap;
                    }
                    return directorSpawnRequest;
                });
            }
            else
            {
                UnityEngine.Debug.LogError("RiskyMod: EmpathyCores IL Hook failed");
            }
        }
    }
}
