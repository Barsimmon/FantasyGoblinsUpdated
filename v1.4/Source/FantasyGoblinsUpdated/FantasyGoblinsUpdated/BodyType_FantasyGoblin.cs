using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace FantasyGoblinsUpdated
{

    /// <summary>
    /// This class makes sure the goblin body gene causes the female and male bodies to be assigned correctly
    /// </summary>
    [StaticConstructorOnStartup]
    public static class BodyType_FantasyGoblin
    {
        static BodyType_FantasyGoblin()
        {
            Log.Message("Registering Fantasy Goblins Updated body patch.");
            var harmony = new Harmony("barsimmon.fantasy_goblins_updated.body_type_fantasy_goblin");
            harmony.PatchAll();

            GeneDef testGene = DefDatabase<GeneDef>.GetNamed("Skin_Goblin_Green", false);// Def added by this mod when Biotech is active
            ThingCategoryDef testThingCategory = DefDatabase<ThingCategoryDef>.GetNamed("alienCorpseCategory", false);// Def added by HAR

            if (testGene == null && testThingCategory == null)
            {
                Log.Error("Missing dependency! Fantasy Goblins Updated requires either the official Biotech expansion OR the Humanoid Alien Races mod in order to actually add goblins. Make sure at least one of these is active.");
            }
        }
    }

    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
    public class PatchPawnGraphicSetResolveAllGraphics
    {
        static void Postfix(PawnGraphicSet __instance)
        {
            // Only execute patch for pawn with genes
            if (__instance.pawn == null || __instance.pawn.genes == null)
            {
                return;
            }

            // Only execute patch for pawn with standard body type
            if (!__instance.pawn.story.bodyType.Equals(BodyTypeDefOf.Female) && !__instance.pawn.story.bodyType.Equals(BodyTypeDefOf.Male))
            {
                return;
            }
            //Log.Message("------------------");
            //Log.Message(__instance.pawn.Name.ToStringShort);

            // See which genes are present
            Boolean goblinBodyPresent = false;
            Boolean standardBodyActive = false;
            List<Gene> genesListForReading = __instance.pawn.genes.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                //Log.Message(genesListForReading[i].def.defName);
                if (genesListForReading[i].def.defName == "Body_Fantasy_Goblin")
                {
                    goblinBodyPresent = true;
                }
                if (genesListForReading[i].def.defName == "Body_Standard" && genesListForReading[i].Active)
                {
                    standardBodyActive = true;
                }
            }

            // Only apply patch if goblin body gene is present and standard body gene is not present or inactive
            // This allows a xenotype with both the standard and goblin body types to be functional
            if (!goblinBodyPresent || standardBodyActive)
            {
                return;
            }

            //Log.Message("Goblin body gene detected. Old path: " + __instance.nakedGraphic.path);

            String path = null;
            if (__instance.pawn.gender == Gender.Female && __instance.nakedGraphic.path == "Things/Pawn/Humanlike/Bodies/Naked_Female")
            {
                path = "Things/Goblin/Bodies/Naked_Female";
            }
            else if (__instance.pawn.gender == Gender.Male && __instance.nakedGraphic.path == "Things/Pawn/Humanlike/Bodies/Naked_Male")
            {
                path = "Things/Goblin/Bodies/Naked_Male";
            }

            if (path != null)
            {
                __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderUtility.GetSkinShader(__instance.pawn.story.SkinColorOverriden), Vector2.one, __instance.pawn.story.SkinColor);
                //Log.Message("New path: " + __instance.nakedGraphic.path);
            }
        }

        [HarmonyPatch(typeof(BackCompatibility), "BackCompatibleDefName")]
        public class ReplaceOldHeads
        {
            private static bool logged = false;

            private static Dictionary<string, string> oldHeads = new Dictionary<string, string>()
            {
                { "Goblin_Male_AverageNormal", "Male_AverageNormal" },
                { "Goblin_Male_AverageWide", "Male_AverageWide" },
                { "Goblin_Male_AveragePointy", "Male_AveragePointy" },
                { "Goblin_Female_AverageNormal", "Female_AverageNormal" },
                { "Goblin_Female_AverageWide", "Female_AverageWide" },
                { "Goblin_Female_AveragePointy", "Female_AveragePointy" }
            };

            static void Postfix(ref string __result)
            {
                if (oldHeads.ContainsKey(__result))
                {
                    __result = oldHeads.TryGetValue(__result);
                    if (!logged)
                    {
                        Log.Message("Fantasy Goblins: replaced old head with new head.");
                        logged = true;
                    }
                }
            }
        }
    }
}
