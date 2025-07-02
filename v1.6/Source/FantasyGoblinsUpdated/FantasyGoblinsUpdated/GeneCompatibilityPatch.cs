using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace FantasyGoblinsUpdated
{
    [HarmonyPatch(typeof(Pawn_GeneTracker), "CheckForOverrides")]
    public static class PatchPawnGeneTrackerCheckForOverrides
    {
        /**
         * The goblin head is graphically incompatible with certain genes, we need to make sure these conflictrs are handled properly.
         * This patch looks for the goblin head. If the pawn has the goblin head, the other genes are checked.
         * If they match one of the genes or gene tags that are in the lists defined in the CML for the goblin heads gene, the gene gets overridden by the goblin head gene.
         * 
         * We can't rely on the goblin head gene's Active or Overridden properties to determine if the goblin head is used, since pawn head selection does not seem to be affected by this.
         * 
         * Example xml config:
         * <modExtensions>
         *     <li Class="FantasyGoblinsUpdated.GeneCompatibility">
         *         <overridesTag>
         *             <li>EyeColor</li>
         *             <li>Nose</li>
         *             <li>Headbone</li>
         *         </overridesTag>
         *         <overrides>
         *             <li>Brow_Heavy</li>
         *             <li>FacialRidges</li>
         *         </overrides>
         *     </li>
         * </modExtensions>
         */
        static void Postfix(Pawn_GeneTracker __instance)
        {
            //Log.Message("CheckForOverrides");

            HeadTypeDef headTypeDef = __instance.pawn?.story?.headType;

            // Only do this when the pawns head is a goblin head.
            if (headTypeDef == null || !headTypeDef.defName.Contains("Goblin")) return;

            //Log.Message("Found goblin head");

            List<Gene> genesListForReading = __instance.GenesListForReading;
            Gene goblinHeadGene = getGoblinHeadGeneFromList(genesListForReading);

            if (goblinHeadGene == null) return;

            //Log.Message("Found goblin head gene. Active: " + goblinHeadGene.Active + ". Overridden: " + goblinHeadGene.Overridden);

            // Get info on overridden genes
            GeneCompatibility geneCompatibility = goblinHeadGene.def.GetModExtension<GeneCompatibility>();

            if (geneCompatibility == null) return;

            bool changed = false;
            for (int j = 0; j < genesListForReading.Count; j++)
            {
                // Goblin head overrides the other gene's exclusionTags or def name
                if (OverridesTag(genesListForReading[j].def, geneCompatibility) || geneCompatibility.overrides.Contains(genesListForReading[j].def.defName))
                {
                    //Log.Message("Override" + genesListForReading[j].def.defName);
                    genesListForReading[j].OverrideBy(goblinHeadGene);
                    changed = true;
                }
            }

            if (!changed) return;
            
            __instance.pawn.skills?.DirtyAptitudes();
            __instance.pawn.Notify_DisabledWorkTypesChanged();
        }

        private static Gene getGoblinHeadGeneFromList(List<Gene> genesListForReading)
        {
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                // Goblin head is active
                if (genesListForReading[i].def.defName == "Fantasy_Goblin_Heads")
                {
                    return genesListForReading[i];
                }
            }

            return null;
        }

        private static bool OverridesTag(GeneDef geneDef, GeneCompatibility geneCompatibility)
        {
            return geneDef.exclusionTags != null && geneCompatibility.overridesTag.Any(x => geneDef.exclusionTags.Any(y => y == x));
        }
    }
}
