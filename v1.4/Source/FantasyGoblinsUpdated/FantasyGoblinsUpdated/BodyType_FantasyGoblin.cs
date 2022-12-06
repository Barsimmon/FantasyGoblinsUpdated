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
        }
    }

    [HarmonyPatch(typeof(PawnGraphicSet), "ResolveAllGraphics")]
    public class PatchPawnGraphicSetResolveAllGraphics
    {
        static void Postfix(PawnGraphicSet __instance)
        {
            List<Gene> genesListForReading = __instance.pawn.genes.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                //Log.Message(genesListForReading[i].def.defName);
                if (genesListForReading[i].Active && genesListForReading[i].def.defName == "Body_Fantasy_Goblin")
                {
                    //Log.Message("Goblin body gene detected. Old path: " + __instance.nakedGraphic.path);
                    String path = "Things/Goblin/Bodies/Naked_Male";
                    if (__instance.pawn.gender == Gender.Female)
                    {
                        path = "Things/Goblin/Bodies/Naked_Female";
                    }
                    __instance.nakedGraphic = GraphicDatabase.Get<Graphic_Multi>(path, ShaderUtility.GetSkinShader(__instance.pawn.story.SkinColorOverriden), Vector2.one, __instance.pawn.story.SkinColor);
                    //Log.Message("New path: " + __instance.nakedGraphic.path);
                }
            }
        }
    }

    /*[HarmonyPatch(typeof(Pawn_GeneTracker), "AddGene", new Type[] { typeof(Gene), typeof(bool) })]
    public class PatchPawnGeneTrackerAddGene
    {
        static void Postfix(Gene gene, bool addAsXenogene)
        {
            Log.Message("Adding gene " + gene.def.defName);
        }
    }*/
}
