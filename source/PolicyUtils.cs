﻿using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace WM.SmarterFoodSelection
{
	public static class PolicyUtils
	{
		internal static Policy GetPolicyAssignedTo(this Pawn eater, Pawn getter=null)
		{
#if DEBUG
			if (Config.debugNoPawnsRestricted)
				return Policies.Unrestricted;
#endif
			if (getter == null)
				getter = eater;

			Policy policy = GetHardcodedPolicy(eater);

			if (policy != null)
				return policy;

			return WorldDataStore_PawnPolicies.GetPawnEntry(eater);

		//nopolicy:

		//	Log.Warning("Found no active policy for " + eater + ". Using default policy.");
		//	return Policies.Unrestricted;
		}

		public static bool CanHaveFoodPolicy(this Pawn pawn)
		{
			if (HasHardcodedPolicy(pawn))
				return true;

			if (pawn.isWildAnimal())
				return false;

			if (pawn.Faction != Faction.OfPlayer && pawn.HostFaction != Faction.OfPlayer)
				return false;

			//null checked in isWildAnimal()
			if (pawn.Faction != null && pawn.Faction.HostileTo(Faction.OfPlayer))
				return false;

			if (pawn.needs.food == null)
				return false;

			return true;
		}

		internal static Policy GetHardcodedPolicy(this Pawn eater)
		{
			if (eater.isWildAnimal())
			{
				return Policies.Wild;
			}

			if (!eater.Faction.IsPlayer && !eater.Faction.RelationWith(Faction.OfPlayer).hostile && !eater.IsPrisonerOfColony)
			{
				if (eater.RaceProps.Animal)
					return Policies.FriendlyPets;
				else
					return Policies.Friendly;
			}

			return null;
		}
		internal static bool HasHardcodedPolicy(this Pawn pawn)
		{
			return pawn.GetHardcodedPolicy() != null;
		}
		internal static IEnumerable<PawnMask> GetAllMasksFor(this Pawn pawn)
		{
			return Policies.AllPawnMasks.Where((PawnMask arg) => arg.MatchesPawn(pawn));
		}
	}
}