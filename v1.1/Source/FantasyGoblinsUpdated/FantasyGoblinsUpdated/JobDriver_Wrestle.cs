using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;
using System;

namespace FantasyGoblinsUpdated
{
    class JobDriver_Wrestle : JobDriver
	{
		private const int MoveDuration = 300;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return pawn.Reserve(job.targetA, job, job.def.joyMaxParticipants, 0, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.EndOnDespawnedOrNull(TargetIndex.A);
			Toil chooseCell = Toils_Wrestling.FindRandomInsideReachableCell(TargetIndex.A, TargetIndex.B);
			yield return chooseCell;
			yield return Toils_Reserve.Reserve(TargetIndex.B);
			yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				job.locomotionUrgency = LocomotionUrgency.Jog;
			};
			toil.tickAction = delegate
			{
				pawn.rotationTracker.FaceCell(base.TargetA.Thing.OccupiedRect().CenterCell);
				if (ticksLeftThisToil == 150)
				{
					Random random = new System.Random();
					if (random.NextDouble() > 0.7)
					{
						SoundDefOf.Corpse_Drop.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
					}
					else
					{
						SoundDefOf.Pawn_Melee_Punch_HitPawn.PlayOneShot(new TargetInfo(pawn.Position, pawn.Map));
					}
				}
				if (Find.TickManager.TicksGame > startTick + job.def.joyDuration)
				{
					EndJobWith(JobCondition.Succeeded);
				}
				else
				{
					JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.EndJob, 1f, (Building)base.TargetThingA);
				}
			};
			toil.handlingFacing = true;
			toil.socialMode = RandomSocialMode.SuperActive;
			toil.defaultCompleteMode = ToilCompleteMode.Delay;
			toil.defaultDuration = MoveDuration;
			toil.AddFinishAction(delegate
			{
				JoyUtility.TryGainRecRoomThought(pawn);
			});
			yield return toil;
			yield return Toils_Reserve.Release(TargetIndex.B);
			yield return Toils_Jump.Jump(chooseCell);
		}

		public override object[] TaleParameters()
		{
			return new object[2]
			{
			pawn,
			base.TargetA.Thing.def
			};
		}
	}

}
