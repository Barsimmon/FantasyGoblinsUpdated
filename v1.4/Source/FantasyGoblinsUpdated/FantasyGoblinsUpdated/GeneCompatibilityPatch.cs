using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verse;

namespace FantasyGoblinsUpdated
{
    [HarmonyPatch(typeof(Pawn_GeneTracker), "CheckForOverrides")]
    public static class PatchPawnGeneTrackerCheckForOverrides
    {
        static void Postfix(Pawn_GeneTracker __instance)
        {
            bool changed = false;
            List<Gene> genesListForReading = __instance.GenesListForReading;

            for (int i = 0; i < genesListForReading.Count; i++)
            {
                // Goblin head is active
                if (genesListForReading[i].def.defName == "Fantasy_Goblin_Heads" && __instance.pawn.story.headType.defName.Contains("Goblin"))
                {
                    // Get info on overridden genes
                    GeneCompatibility geneCompatibility = genesListForReading[i].def.GetModExtension<GeneCompatibility>();
                    if (geneCompatibility == null) {
                        return;
                    }
                    for (int j = i + 1; j < genesListForReading.Count; j++)
                    {
                        // Golbin head overrides the other gene's exclusionTags or def name
                        if (OverridesTag(genesListForReading[j].def, geneCompatibility) || geneCompatibility.overrides.Contains(genesListForReading[j].def))
                        {
                            genesListForReading[j].OverrideBy(genesListForReading[i]);
                            changed = true;
                            continue;
                        }
                    }
                    break;
                }
            }

            if (changed)
            {
                __instance.pawn.skills?.Notify_GenesChanged();
                __instance.pawn.Notify_DisabledWorkTypesChanged();
            }
        }

        private static bool OverridesTag(GeneDef geneDef, GeneCompatibility geneCompatibility)
        {
            return geneDef.exclusionTags != null && geneCompatibility.overridesTag.Any(x => geneDef.exclusionTags.Any(y => y == x));
        }
    }
}
