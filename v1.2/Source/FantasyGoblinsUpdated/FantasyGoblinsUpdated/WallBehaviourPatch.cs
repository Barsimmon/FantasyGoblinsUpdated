using Verse;
using RimWorld;
using HarmonyLib;

namespace FantasyGoblinsUpdated
{
    class WallBehaviourPatch
    {
        static WallBehaviourPatch()
        {
            var harmony = new Harmony("barsimmon.fantasy_goblins_updated.wall_behaviour_patch");
            harmony.PatchAll();
        }

        public static bool defNameIsGoblinWall(string defName)
        {
            if (defName == "Goblin_MudWall" || defName == "Goblin_Palisade" || defName == "Goblin_StoneWall")
            {
                return true;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(GenConstruct), "CanPlaceBlueprintOver")]
    public class PatchCanPlaceBluePrintOver
    {
        static void Postfix(BuildableDef newDef, ThingDef oldDef, ref bool __result)
        {
            if (__result == false)
            {
                ThingDef thingDef = newDef as ThingDef;
                BuildableDef buildableDef = GenConstruct.BuiltDefOf(oldDef);
                ThingDef thingDef2 = buildableDef as ThingDef;
                if (oldDef.category == ThingCategory.Building || oldDef.IsBlueprint || oldDef.IsFrame)
				{
					if (thingDef != null)
					{
						if (thingDef2 != null && WallBehaviourPatch.defNameIsGoblinWall(thingDef2.defName) && thingDef.building != null && thingDef.building.canPlaceOverWall)
						{
							__result = true;
						}
					}
				}
			}
        }
    }

    [HarmonyPatch(typeof(GenConstruct), "BlocksConstruction")]
    public class PatchBlocksConstruction
    {
        static void Postfix(Thing constructible, Thing t, ref bool __result)
        {
            if (__result == true)
            {
                ThingDef thingDef = (constructible is Blueprint) ? constructible.def : ((!(constructible is Frame)) ? constructible.def.blueprintDef : constructible.def.entityDefToBuild.blueprintDef);
                ThingDef thingDef2 = thingDef.entityDefToBuild as ThingDef;
                if (thingDef2 != null)
                {
                    if (WallBehaviourPatch.defNameIsGoblinWall(t.def.defName) && thingDef2.building != null && thingDef2.building.canPlaceOverWall)
                    {
                        __result = false;
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(GenSpawn), "SpawningWipes")]
    public class PatchSpawningWipes
    {
        static void Postfix(BuildableDef newEntDef, BuildableDef oldEntDef, ref bool __result)
        {
            if (__result == false)
            {
				ThingDef thingDef = newEntDef as ThingDef;
				ThingDef thingDef2 = oldEntDef as ThingDef;
                if (thingDef == null || thingDef2 == null)
                {
                    return;
                }
                ThingDef thingDef3 = thingDef.entityDefToBuild as ThingDef;
				if (thingDef2.IsBlueprint)
				{
					if (thingDef.IsBlueprint)
					{
                        ThingDef thingDef4 = thingDef2.entityDefToBuild as ThingDef;

                        if (thingDef4 != null && thingDef3 != null && thingDef3.building != null && thingDef3.building.canPlaceOverWall && thingDef2.entityDefToBuild is ThingDef && WallBehaviourPatch.defNameIsGoblinWall(thingDef4.defName))
						{
							__result = true;
						}
					}
				}
			}
        }
    }
}
