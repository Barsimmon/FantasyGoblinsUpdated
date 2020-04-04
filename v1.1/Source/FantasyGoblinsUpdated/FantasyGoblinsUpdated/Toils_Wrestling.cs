using RimWorld;
using Verse;
using Verse.AI;

namespace FantasyGoblinsUpdated
{
    class Toils_Wrestling
	{
		public static Toil FindRandomInsideReachableCell(TargetIndex insideInd, TargetIndex cellInd)
		{
			Toil findCell = new Toil();
			findCell.initAction = delegate
			{
				Pawn actor = findCell.actor;
				Job curJob = actor.CurJob;
				LocalTargetInfo target = curJob.GetTarget(insideInd);
				if (target.HasThing && (!target.Thing.Spawned || target.Thing.Map != actor.Map))
				{
					Log.Error(actor + " could not find standable cell inside " + target + " because this thing is either unspawned or spawned somewhere else.");
					actor.jobs.curDriver.EndJobWith(JobCondition.Errored);
				}
				else if (!target.HasThing)
				{
					Log.Error(actor + " could not find standable cell inside " + target + " because it is not a thing.");
					actor.jobs.curDriver.EndJobWith(JobCondition.Errored);
					return;
				}
				else
				{
					int num = 0;
					IntVec3 c;
					do
					{
						num++;
						if (num > 100)
						{
							Log.Error(actor + " could not find standable cell inside " + target);
							actor.jobs.curDriver.EndJobWith(JobCondition.Errored);
							return;
						}

						c = RandomInsideCell(target.Thing);
					}
					while (!c.Standable(actor.Map) || !actor.CanReserve(c) || !actor.CanReach(c, PathEndMode.OnCell, Danger.Deadly));
					curJob.SetTarget(cellInd, c);
				}
			};
			return findCell;
		}

		public static IntVec3 RandomInsideCell(Thing t)
		{
			CellRect cellRect = t.OccupiedRect();
			return cellRect.RandomCell;
		}
	}
}
