using System.Runtime.CompilerServices;

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace MyFirstStS2Mod.Scripts;

internal static class BattleState
{
    private enum DrunkState
    {
        None,
        Locked,
        PenaltyActive
    }

    private sealed class CreatureState
    {
        public bool NextShaDealsDoubleDamage { get; set; }
        public int NextAttackExtraScorch { get; set; }
        public DrunkState Drunk { get; set; }
    }

    private static readonly ConditionalWeakTable<object, CreatureState> CreatureStates = new();

    public static bool HasPlayedShaThisTurn(CardModel card)
    {
        var combat = card.CombatState;
        var history = CombatManager.Instance?.History;
        var owner = card.Owner;
        if (combat is null || history is null || owner is null)
        {
            return false;
        }

        return history.CardPlaysFinished.Any(entry =>
            entry.HappenedThisTurn(combat)
            && entry.CardPlay.Card is Cards.ShaCard
            && entry.CardPlay.Card.Owner == owner);
    }

    public static void SetNextShaDealsDoubleDamage(object owner)
    {
        CreatureStates.GetOrCreateValue(owner).NextShaDealsDoubleDamage = true;
    }

    public static bool TryConsumeNextShaDealsDoubleDamage(object owner)
    {
        var state = CreatureStates.GetOrCreateValue(owner);
        if (!state.NextShaDealsDoubleDamage)
        {
            return false;
        }

        state.NextShaDealsDoubleDamage = false;
        return true;
    }

    public static void ApplyDrunk(object owner)
    {
        CreatureStates.GetOrCreateValue(owner).Drunk = DrunkState.Locked;
    }

    public static void ActivateDrunkPenalty(object owner)
    {
        var state = CreatureStates.GetOrCreateValue(owner);
        if (state.Drunk == DrunkState.Locked)
        {
            state.Drunk = DrunkState.PenaltyActive;
        }
    }

    public static bool IsDrunk(object owner)
    {
        return CreatureStates.GetOrCreateValue(owner).Drunk != DrunkState.None;
    }

    public static bool IsDrunkPenaltyActive(object owner)
    {
        return CreatureStates.GetOrCreateValue(owner).Drunk == DrunkState.PenaltyActive;
    }

    public static void ClearDrunk(object owner)
    {
        CreatureStates.GetOrCreateValue(owner).Drunk = DrunkState.None;
    }

    public static void SetNextAttackExtraScorch(object owner, int amount)
    {
        CreatureStates.GetOrCreateValue(owner).NextAttackExtraScorch = amount;
    }

    public static int TryConsumeNextAttackExtraScorch(object owner)
    {
        var state = CreatureStates.GetOrCreateValue(owner);
        var amount = state.NextAttackExtraScorch;
        state.NextAttackExtraScorch = 0;
        return amount;
    }
}
