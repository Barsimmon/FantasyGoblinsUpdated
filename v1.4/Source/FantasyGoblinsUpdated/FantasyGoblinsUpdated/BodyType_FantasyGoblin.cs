﻿using HarmonyLib;
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

            GeneDef testGene = DefDatabase<GeneDef>.GetNamed("Skin_Goblin_Green", false);
            ThingCategoryDef testThingCategory = DefDatabase<ThingCategoryDef>.GetNamed("alienCorpseCategory");

            if (testGene == null && testThingCategory == null)
            {
                Log.Error("Missing dependency! Fantasy Goblins Updated requires either the official Biotech expansion OR the Humanoid Alien Races mod. Make sure at least one of these is active.");
            }

            //Dialog_MessageBox messageBox = new Dialog_MessageBox("Fantasy Goblins Updated requires either the official Biotech expansion OR the Humanoid Alien Races mod. Make sure at least one of these is active.");
            //Find.WindowStack.Add(messageBox);
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
            for (int i = 0; i < genesListForReading.Count; i++) {
                //Log.Message(genesListForReading[i].def.defName);
                if (genesListForReading[i].def.defName == "Body_Fantasy_Goblin") {
                    goblinBodyPresent = true;
                }
                if (genesListForReading[i].def.defName == "Body_Standard" && genesListForReading[i].Active) {
                    standardBodyActive = true;
                }
            }

            // Only apply patch if goblin body gene is present and standard body gene is not present or inactive
            // This allows a xenotype with both the standard and goblin body types to be functional
            if (!goblinBodyPresent || standardBodyActive) {
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
    }
}
