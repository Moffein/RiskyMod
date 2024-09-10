using UnityEngine;
using RoR2;
using BepInEx.Configuration;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using RoR2.Skills;

namespace RiskyMod.Survivors.Merc
{
    public class MercCore
    {
        public static bool enabled = true;

        public static bool buffDefaultShift;

        public MercCore()
        {
            if (!enabled) return;

            if (buffDefaultShift)
            {
                Addressables.LoadAssetAsync<MercDashSkillDef>("RoR2/Base/Merc/MercBodyAssaulter.asset").WaitForCompletion().skillDescriptionToken = "MERC_UTILITY_DESCRIPTION_RISKYMOD";
                SneedUtils.SneedUtils.SetAddressableEntityStateField("RoR2/Base/Merc/EntityStates.Merc.Assaulter2.asset", "damageCoefficient", "4.5");
            }
        }
    }
}
