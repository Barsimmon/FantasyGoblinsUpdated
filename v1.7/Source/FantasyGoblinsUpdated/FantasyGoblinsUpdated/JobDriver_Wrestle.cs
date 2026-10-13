using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.Sound;
using System;

namespace FantasyGoblinsUpdated
{
	/*
	 * This was based on the billiards JobDriver. Fighting noises are played instead of billiards and pawn location is inside the pit instead of around it.
	 */
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
					JoyTickCheckEndCompat(pawn, base.TargetThingA);
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

		private static System.Reflection.MethodInfo joyTickCheckEnd;
		private static System.Reflection.ParameterInfo[] joyTickParams;

		// JoyUtility.JoyTickCheckEnd signature changed in 1.7; resolve it at runtime.
		private static void JoyTickCheckEndCompat(Pawn pawn, Thing source)
		{
			if (joyTickCheckEnd == null)
			{
				foreach (var m in typeof(JoyUtility).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
				{
					if (m.Name == "JoyTickCheckEnd")
					{
						joyTickCheckEnd = m;
						joyTickParams = m.GetParameters();
						break;
					}
				}
				if (joyTickCheckEnd == null)
				{
					return;
				}
			}
			object[] args = new object[joyTickParams.Length];
			for (int i = 0; i < args.Length; i++)
			{
				var pt = joyTickParams[i].ParameterType;
				if (pt == typeof(Pawn)) args[i] = pawn;
				else if (pt == typeof(JoyTickFullJoyAction)) args[i] = JoyTickFullJoyAction.EndJob;
				else if (pt == typeof(float)) args[i] = 1f;
				else if (typeof(Thing).IsAssignableFrom(pt) && pt.IsInstanceOfType(source)) args[i] = source;
				else if (joyTickParams[i].HasDefaultValue) args[i] = joyTickParams[i].DefaultValue;
				else args[i] = pt.IsValueType ? System.Activator.CreateInstance(pt) : null;
			}
			joyTickCheckEnd.Invoke(null, args);
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
