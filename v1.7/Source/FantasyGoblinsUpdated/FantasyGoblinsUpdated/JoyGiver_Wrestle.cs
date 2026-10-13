using RimWorld;
using Verse;
using Verse.AI;

namespace FantasyGoblinsUpdated
{
    class JoyGiver_Wrestle : JoyGiver_InteractBuilding
	{
		protected override bool CanDoDuringGathering => true;

		protected override Job TryGivePlayJob(Pawn pawn, Thing t)
		{
			return JobMaker.MakeJob(def.jobDef, t);
		}
	}
}
