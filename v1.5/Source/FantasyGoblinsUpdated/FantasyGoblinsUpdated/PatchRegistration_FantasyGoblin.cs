using HarmonyLib;
using Verse;

namespace FantasyGoblinsUpdated
{
    [StaticConstructorOnStartup]
    public static class PatchRegistration_FantasyGoblin
    {
        /**
         * Registers this mod's patches
         */
        static PatchRegistration_FantasyGoblin()
        {
            Log.Message("Registering Fantasy Goblins Updated harmony patches.");
            var harmony = new Harmony("barsimmon.fantasy_goblins_updated");
            harmony.PatchAll();

            checkForDependencies();
        }

        /**
         * Checks if either Biotech or HAR is active. If neither is active, generates an error message
         */
        private static void checkForDependencies()
        {
            GeneDef testGene = DefDatabase<GeneDef>.GetNamed("Skin_Goblin_Green", false);// Def added by this mod when Biotech is active
            ThingCategoryDef testThingCategory = DefDatabase<ThingCategoryDef>.GetNamed("alienCorpseCategory", false);// Def added by HAR

            if (testGene == null && testThingCategory == null)
            {
                Log.Error("Missing dependency! Fantasy Goblins Updated requires either the official Biotech expansion OR the Humanoid Alien Races mod in order to actually add goblins. Make sure at least one of these is active.");
            }
        }
    }
}
