using HarmonyLib;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace FantasyGoblinsUpdated
{
    [HarmonyPatch(typeof(HeadTypeDef), "GetGraphic")]
    public class PatchShaderUtilityGetSkinShader
    {
        /**
         * Replaces the shader for goblin heads for alive goblins.
         * Because the patched method looks in the HeadTypeDef.graphics cache for heads of a certain color and shader type, and we can't make it look for the shader we want, we have to loop over the cache a second time and add a second graphic to the cache.
         * Goblin HeadTypeDefs will have a cache that's up to twice as large per skin color as other heads, but that will not impact performance significantly.
         */
        static void Postfix(HeadTypeDef __instance, Pawn pawn, Color color, ref Graphic_Multi __result, ref List<KeyValuePair<Color, Graphic_Multi>> ___graphics)
        {
            //Log.Message("---------------------");
            //Log.Message("Getting Graphic for head " + __instance.defName + " of color " + color + ". " + ___graphics.Count + " in cache.");

            // Only do this for Goblin heads, skip other heads
            if (!__instance.defName.Contains("Goblin") || __result == null || pawn == null) return;

            //Log.Message("Getting goblin head Graphic.");

            // Only do this when the shader is one of these two, skip dead/mutant goblins
            if (__result.Shader != ShaderDatabase.CutoutSkin && __result.Shader != ShaderDatabase.CutoutSkinColorOverride) return;

            //Log.Message("CutoutSkin or CutoutSkinColorOverride.");

            // Try to retrieve from cache.
            for (int i = 0; i < ___graphics.Count; i++)
            {
                if (color.IndistinguishableFrom(___graphics[i].Key) && ___graphics[i].Value.Shader == ShaderDatabase.CutoutComplex)
                {
                    //Log.Message("Get from cache.");
                    // Return cached head Graphic
                    __result = ___graphics[i].Value;
                    return;
                }
            }

            //Log.Message("Not fixed yet, replace.");

            // A head of this type and color hasn't been generated yet, generate it, replace the __result, and put it in the cache for next time.
            __result = (Graphic_Multi)GraphicDatabase.Get<Graphic_Multi>(__result.GraphicPath, ShaderDatabase.CutoutComplex, Vector2.one, color);
            ___graphics.Add(new KeyValuePair<Color, Graphic_Multi>(color, __result));
        }
    }
}
