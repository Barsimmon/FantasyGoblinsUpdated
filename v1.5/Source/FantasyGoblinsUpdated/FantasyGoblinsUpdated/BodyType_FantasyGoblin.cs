﻿using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace FantasyGoblinsUpdated
{
    [HarmonyPatch(typeof(PawnRenderNode_Body), "GraphicFor")]
    public class PatchPawnRenderNodeBody
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
    }
}
