using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RiskyMod.Tweaks.Artifact
{
    public class Sacrifice
    {
        public static bool enabled = true;
        public static float dropChance = 10f;
        public Sacrifice()
        {
            if (!enabled) return;

            IL.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += (il) =>
            {
                ILCursor c = new ILCursor(il);
                //Change base drop chance
                if (c.TryGotoNext(MoveType.After, x => x.MatchLdcR4(5f)))
                {
                    c.EmitDelegate<Func<float, float>>(orig =>
                    {
                        return RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef) ? dropChance * 0.5f : dropChance;
                    });
                }
                else
                {
                    Debug.LogError("SacrificeTweaks: Change Base Drop Chance IL hook failed.");
                }

                //Clamp final drop chance
                if (c.TryGotoNext(MoveType.After, x => x.MatchCall(typeof(RoR2.Util), "GetExpAdjustedDropChancePercent")))
                {
                    c.EmitDelegate<Func<float, float>>(orig =>
                    {
                        float finalDropChance = orig;

                        if (orig > 0f)
                        {
                            bool swarmsEnabled = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef);

                            float baseChance = swarmsEnabled ? dropChance * 0.5f : dropChance;
                            float maxChance = baseChance;

                            if (finalDropChance < baseChance)
                            {
                                finalDropChance = baseChance;
                            }

                            if (finalDropChance > maxChance)
                            {
                                finalDropChance = maxChance;
                            }
                        }

                        return finalDropChance;
                    });
                }
                else
                {
                    Debug.LogError("SacrificeTweaks: Clamp Final drop chance IL Hook failed.");
                }
            };
        }
    }
}
