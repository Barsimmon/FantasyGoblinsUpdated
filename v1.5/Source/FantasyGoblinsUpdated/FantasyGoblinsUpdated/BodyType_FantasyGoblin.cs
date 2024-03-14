using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace FantasyGoblinsUpdated
{

    /// <summary>
    /// This class makes sure the goblin body gene causes the female and male bodies to be assigned correctly
    /// </summary>
    [StaticConstructorOnStartup]
    public static class BodyType_FantasyGoblin
    {
        public static bool patchingHeadShader = false;

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

    [HarmonyPatch(typeof(PawnRenderNode_Body), "GraphicFor")]
    public class PatchPawnGraphicSetResolveAllGraphics
    {
        static void Postfix(PawnRenderNode_Body __instance, Pawn pawn, ref Graphic __result)
        {
            if (__result == null)
            {
                return;
            }

            if (__result.path != "Things/Pawn/Humanlike/Bodies/Naked_Female" && __result.path != "Things/Pawn/Humanlike/Bodies/Naked_Male")
            {
                return;
            }

            // Only execute patch for pawn with genes
            if (pawn == null || pawn.genes == null)
            {
                return;
            }

            //Log.Message("------------------");
            //Log.Message(pawn.Name.ToStringShort);

            // See which genes are present
            Boolean goblinBodyActive = false;
            List<Gene> genesListForReading = pawn.genes.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                if (genesListForReading[i].def.defName == "Body_Fantasy_Goblin" && genesListForReading[i].Active) {
                    goblinBodyActive = true;
                    break;
                }
            }

            // Only apply patch if goblin body gene is active
            if (!goblinBodyActive)
            {
                return;
            }

            //Log.Message("Goblin body active. Old path: " + __result.path);

            String path = null;
            if (__result.path == "Things/Pawn/Humanlike/Bodies/Naked_Female")
            {
                path = "Things/Goblin/Bodies/Naked_Female";
            }
            else if (__result.path == "Things/Pawn/Humanlike/Bodies/Naked_Male")
            {
                path = "Things/Goblin/Bodies/Naked_Male";
            }

            if (path == null) return;

            __result = GraphicDatabase.Get<Graphic_Multi>(path, __result.Shader, Vector2.one, __result.Color);
        }
        /*
        static void FixHeadGraphic(PawnGraphicSet pawnGraphicSet)
        {
            // Only execute patch for pawn
            if (pawnGraphicSet.pawn == null)
            {
                return;
            }

            HeadTypeDef headType = pawnGraphicSet.pawn.story?.headType;

            if (headType == null || !headType.defName.Contains("Goblin"))
            {
                return;
            }

            // TODO cache
            pawnGraphicSet.headGraphic = GraphicDatabase.Get<Graphic_Multi>(headType.graphicPath, ShaderDatabase.CutoutComplex, Vector2.one, pawnGraphicSet.pawn.story.SkinColor);
        }*/
    }
}
