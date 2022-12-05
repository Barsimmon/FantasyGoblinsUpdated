using HarmonyLib;
using Verse;
using RimWorld;
using System.Collections.Generic;

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
            var harmony = new Harmony("barsimmon.fantasy_goblins_updated.body_type_fantasy_goblin");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(GeneUtility), "ToBodyType")]
    public class PatchToBodyType
    {
        static void Postfix(GeneticBodyType bodyType, Pawn pawn, ref BodyTypeDef __result)
        {
            if (ModLister.BiotechInstalled && bodyType == GeneticBodyType.Standard)
            {
                List<Gene> genesListForReading = pawn.genes.GenesListForReading;
                for (int i = 0; i < genesListForReading.Count; i++)
                {
                    if (genesListForReading[i].Active && genesListForReading[i].def.defName == "Body_Fantasy_Goblin")
                    {
                        if (pawn.gender == Gender.Female)
                        {
                            __result = FantasyGoblinBodyTypeDefOf.Fantasy_Goblin_Female;
                        }
                        else
                        {
                            __result = FantasyGoblinBodyTypeDefOf.Fantasy_Goblin_Male;
                        }
                    }
                }
            }
        }
    }

    [DefOf]
    public static class FantasyGoblinBodyTypeDefOf
    {
        public static BodyTypeDef Fantasy_Goblin_Male;

        public static BodyTypeDef Fantasy_Goblin_Female;

        static FantasyGoblinBodyTypeDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(FantasyGoblinBodyTypeDefOf));
        }
    }
}
