using Verse;
using RimWorld;
using HarmonyLib;

namespace FantasyGoblinsUpdated.MudPitFix
{
    /// <summary>
	/// This class attempts to fix problems caused by the Mud_Pit/squirrel meat bugfix in existing saves.
    /// Can be removed when no one probably still has an active save from before 2020-04-03.
	/// </summary>
    [StaticConstructorOnStartup]
    public static class MudPitFix
    {
        static MudPitFix()
        {
            var harmony = new Harmony("barsimmon.fantasy_goblins_updated.mud_pit_fix");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(BackCompatibility), "BackCompatibleDefName")]
    public class ReplaceMudPit
    {
        private static bool logged = false;

        static void Postfix(ref string __result)
        {
            if (__result == "Mud_Pit")
            {
                __result = "Goblin_Mud_Pit";
                if (!logged)
                {
                    Log.Message("Fantasy Goblins: replaced Mud_Pit with Goblin_Mud_Pit.");
                    logged = true;
                }
            }
        }
    }

    [HarmonyPatch(typeof(BackCompatibility), "BackCompatibleThingDefWithShortHash_Force")]
    public class ReplaceMineableSteel
    {
        private static bool logged = false;

        static void Postfix(ushort hash, ref ThingDef __result)
        {
            if (hash == (ushort)27294)
            {
                __result = ThingDefOf.MineableSteel;
                if (!logged)
                {
                    Log.Message("Fantasy Goblins: Fixed MineableSteel that was broken because of mud pit fix.");
                    logged = true;
                }
            }
        }
    }
}
