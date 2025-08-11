﻿using MonoMod.Cil;
using System;
using System.Collections.Generic;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;

namespace RiskyMod.Enemies.Mobs
{
    public class Bison
    {
        public static bool enabled = true;
        public static List<string> SnowyStageList = new List<string> { "snowyforest", "itsnowyforest", "frozenwall", "itfrozenwall" };

        public Bison()
        {
            if (!enabled) return;
            CharacterBody cb = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bison/BisonBody.prefab").WaitForCompletion().GetComponent<CharacterBody>();
            cb.baseDamage = 15f;
            cb.levelDamage = cb.baseDamage * 0.2f;

            //Disble snowy material on non snowy stages
            IL.EntityStates.Bison.SpawnState.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchLdsfld<EntityStates.Bison.SpawnState>("snowyMaterial")
                    ))
                {
                    c.EmitDelegate<Func<Material, Material>>(snowyMaterial =>
                    {
                        SceneDef sd = RoR2.SceneCatalog.GetSceneDefForCurrentScene();
                        if (sd && !SnowyStageList.Contains(sd.baseSceneName))
                        {
                            snowyMaterial = null;
                        }
                        return snowyMaterial;
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("RiskyMod: Bison IL Hook failed");
                }
            };

            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Bison/EntityStates.Bison.Charge.asset", "chargeMovementSpeedCoefficient", "12"); //8 vanilla
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Bison/EntityStates.Bison.Charge.asset", "turnSpeed", "600"); //300 vanilla
            SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Bison/EntityStates.Bison.Charge.asset", "selfStunDuration", "1"); //3 vanilla
        }
    }
}
