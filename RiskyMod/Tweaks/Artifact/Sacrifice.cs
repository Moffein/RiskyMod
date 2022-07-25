using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;

namespace RiskyMod.Tweaks.Artifact
{
    public class Sacrifice
    {
        public static bool enabled = true;
        public Sacrifice()
        {
            if (!enabled) return;

            IL.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);

                //Change base drop chance
                if (c.TryGotoNext(MoveType.After,
                    x => x.MatchLdcR4(5f)
                    ))
                {
                    c.EmitDelegate<Func<float, float>>(orig =>
                    {
                        return RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef) ? 5f : 10f;
                    });

                    //Clamp final drop chance
                    if(c.TryGotoNext(
                        x => x.MatchStloc(0) //Called after GetExpAdjustedDropChancePercent
                        ))
                    {
                        c.EmitDelegate<Func<float, float>>(orig =>
                        {
                            float finalDropChance = orig;
                            bool swarmsEnabled = RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.swarmsArtifactDef);

                            float baseChance = 10f;
                            float maxChance = 10f;

                            if (swarmsEnabled)
                            {
                                baseChance = 5f;
                                maxChance = 5f;
                            }

                            if (finalDropChance < baseChance)
                            {
                                finalDropChance = baseChance;
                            }

                            if (finalDropChance > maxChance)
                            {
                                finalDropChance = maxChance;
                            }

                            return finalDropChance;
                        });
                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("RiskyMod: Sacrifice IL Hook failed");
                }
            };
        }
    }
}
