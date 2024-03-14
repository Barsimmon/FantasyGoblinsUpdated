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
            if (!__instance.pawn.story.headType.defName.Contains("Goblin"))
            {
                return;
            }

            bool changed = false;
            List<Gene> genesListForReading = __instance.GenesListForReading;

            for (int i = 0; i < genesListForReading.Count; i++)
            {
                // Goblin head is active
                if (genesListForReading[i].def.defName == "Fantasy_Goblin_Heads")
                {
                    //Log.Message("Found goblin head gene");
                    // Get info on overridden genes
                    GeneCompatibility geneCompatibility = genesListForReading[i].def.GetModExtension<GeneCompatibility>();
                    if (geneCompatibility == null) {
                        return;
                    }
                    for (int j = 0; j < genesListForReading.Count; j++)
                    {
                        // Goblin head overrides the other gene's exclusionTags or def name
                        if (OverridesTag(genesListForReading[j].def, geneCompatibility) || geneCompatibility.overrides.Contains(genesListForReading[j].def.defName))
                        {
                            //Log.Message("Should override" + genesListForReading[j].def.defName);
                            genesListForReading[j].OverrideBy(genesListForReading[i]);
                            changed = true;
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
