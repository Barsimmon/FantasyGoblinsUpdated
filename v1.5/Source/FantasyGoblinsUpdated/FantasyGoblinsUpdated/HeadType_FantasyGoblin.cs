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
    [HarmonyPatch(typeof(HeadTypeDef), "GetGraphic")]
    public class PatchShaderUtilityGetSkinShader
    {
        static void Postfix(HeadTypeDef __instance, Pawn pawn, Color color, ref Graphic_Multi __result, ref List<KeyValuePair<Color, Graphic_Multi>> ___graphics)
        {
            Log.Message("---------------------");
            Log.Message("Getting head Graphic.");

            if (!__instance.defName.Contains("Goblin") || __result == null || pawn == null) return;

            Log.Message("Getting goblin head Graphic.");

            if (__result.Shader != ShaderDatabase.CutoutSkinColorOverride && __result.Shader != ShaderDatabase.CutoutSkin) return;

            Log.Message("Doesn't exist yet, doing the thing.");

            __result = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(__result.GraphicPath, ShaderDatabase.CutoutComplex, Vector2.one, color);

            ___graphics[___graphics.Count - 1] = new KeyValuePair<Color, Graphic_Multi>(color, __result);
        }
    }
}
