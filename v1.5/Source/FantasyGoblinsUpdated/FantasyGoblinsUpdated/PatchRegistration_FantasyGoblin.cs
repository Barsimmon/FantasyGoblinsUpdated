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
    public static class PatchRegistration_FantasyGoblin
    {
        static PatchRegistration_FantasyGoblin()
        {
            Log.Message("Registering Fantasy Goblins Updated harmony patch.");
            var harmony = new Harmony("barsimmon.fantasy_goblins_updated");
            harmony.PatchAll();

            GeneDef testGene = DefDatabase<GeneDef>.GetNamed("Skin_Goblin_Green", false);// Def added by this mod when Biotech is active
            ThingCategoryDef testThingCategory = DefDatabase<ThingCategoryDef>.GetNamed("alienCorpseCategory", false);// Def added by HAR

            if (testGene == null && testThingCategory == null)
            {
                Log.Error("Missing dependency! Fantasy Goblins Updated requires either the official Biotech expansion OR the Humanoid Alien Races mod in order to actually add goblins. Make sure at least one of these is active.");
            }
        }
    }
}
