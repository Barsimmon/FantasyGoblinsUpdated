using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verse;

namespace FantasyGoblinsUpdated
{
    public static class FantasyGoblinsGeneOverrideUtility
    {
        public static bool Overrides(GeneDef gene, GeneDef other, bool isXenogene, bool otherIsXenogene)
        {
            GeneCompatibility geneCompatibility = gene.GetModExtension<GeneCompatibility>();
            GeneCompatibility otherCompatibility = other.GetModExtension<GeneCompatibility>();

            if (geneCompatibility == null && otherCompatibility == null)
            {
                return false;
            }

            Log.Message("FantasyGoblinsGeneOverrideUtility.Overrides " + gene.defName + " " + other.defName);

            if (geneCompatibility != null)
            {
                if (geneCompatibility.overrides != null && geneCompatibility.overrides.Contains(other))
                {
                    return true;
                }
                if (geneCompatibility.overridesTag != null && other.exclusionTags != null && geneCompatibility.overridesTag.Any(x => other.exclusionTags.Any(y => y == x)))
                {
                    return true;
                }
            }
            if (otherCompatibility != null)
            {
                if (otherCompatibility.overriddenBy != null && otherCompatibility.overriddenBy.Contains(gene))
                {
                    return true;
                }
                if (otherCompatibility.overriddenByTag != null && gene.exclusionTags != null && otherCompatibility.overriddenByTag.Any(x => gene.exclusionTags.Any(y => y == x)))
                {
                    return true;
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(GeneDefWithType), "Overrides", new Type[] { typeof(GeneDefWithType) })]
    public static class PatchGeneDefWithTypeOverrides
    {
        static void Postfix(GeneDefWithType __instance, GeneDefWithType other, ref bool __result)
        {
            __result = __result || FantasyGoblinsGeneOverrideUtility.Overrides(__instance.geneDef, other.geneDef, __instance.isXenogene, other.isXenogene);
        }
    }

    [HarmonyPatch(typeof(Pawn_GeneTracker), "CheckForOverrides")]
    public static class PatchPawnGeneTrackerCheckForOverrides
    {
        static void Postfix(Pawn_GeneTracker __instance)
        {
            bool changed = false;
            List<Gene> genesListForReading = __instance.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                for (int j = i + 1; j < genesListForReading.Count; j++)
                {
                    if (genesListForReading[i].overriddenByGene != null && !genesListForReading[i].overriddenByGene.def.Equals(genesListForReading[j]))
                    {
                        // already overriden by other gene
                        continue;
                    }
                    if (FantasyGoblinsGeneOverrideUtility.Overrides(genesListForReading[i].def, genesListForReading[j].def, __instance.IsXenogene(genesListForReading[i]), __instance.IsXenogene(genesListForReading[j])))
                    {
                        genesListForReading[i].OverrideBy(null);
                        genesListForReading[j].OverrideBy(genesListForReading[i]);
                        changed = true;
                    }
                }
            }

            if (changed)
            {
                __instance.pawn.skills?.Notify_GenesChanged();
                __instance.pawn.Notify_DisabledWorkTypesChanged();
            }
        }
    }
}
