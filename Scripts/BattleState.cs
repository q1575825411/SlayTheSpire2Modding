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
        public int TurnToken { get; set; } = int.MinValue;
        public bool NextShaDealsDoubleDamage { get; set; }
        public int NextShaFlatDamageBonus { get; set; }
        public int NextAttackExtraScorch { get; set; }
        public int ManualShaUsesThisTurn { get; set; }
        public int ExtraShaUsesThisTurn { get; set; }
        public bool QingLongTriggeredThisTurn { get; set; }
        public DrunkState Drunk { get; set; }
    }

    private static readonly ConditionalWeakTable<object, CreatureState> CreatureStates = new();

    private static CreatureState GetState(object owner, object? combatState)
    {
        var state = CreatureStates.GetOrCreateValue(owner);
        var turnToken = RuntimeReflection.GetCombatTurnToken(combatState);
        if (state.TurnToken != turnToken)
        {
            state.TurnToken = turnToken;
            state.NextShaDealsDoubleDamage = false;
            state.NextShaFlatDamageBonus = 0;
            state.NextAttackExtraScorch = 0;
            state.ManualShaUsesThisTurn = 0;
            state.ExtraShaUsesThisTurn = 0;
            state.QingLongTriggeredThisTurn = false;
        }

        return state;
    }

    public static bool HasPlayedShaThisTurn(CardModel card)
    {
        var combat = card.CombatState;
        var history = CombatManager.Instance?.History;
        var owner = card.Owner;
        if (combat is null || history is null || owner is null)
        {
            return false;
        }

        var state = GetState(owner, combat);

        var playsThisTurn = state.ManualShaUsesThisTurn + history.CardPlaysFinished.Count(entry =>
            entry.HappenedThisTurn(combat)
            && entry.CardPlay.Card is Cards.ShaCard
            && entry.CardPlay.Card.Owner == owner);

        return playsThisTurn >= 1 + state.ExtraShaUsesThisTurn;
    }

    public static void SetNextShaDealsDoubleDamage(object owner)
    {
        GetState(owner, RuntimeReflection.GetCombatState(owner)).NextShaDealsDoubleDamage = true;
    }

    public static void AddNextShaFlatDamage(object owner, int amount)
    {
        GetState(owner, RuntimeReflection.GetCombatState(owner)).NextShaFlatDamageBonus += amount;
    }

    public static int TryConsumeNextShaFlatDamage(object owner)
    {
        var state = GetState(owner, RuntimeReflection.GetCombatState(owner));
        var amount = state.NextShaFlatDamageBonus;
        state.NextShaFlatDamageBonus = 0;
        return amount;
    }

    public static bool TryConsumeNextShaDealsDoubleDamage(object owner)
    {
        var state = GetState(owner, RuntimeReflection.GetCombatState(owner));
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
        GetState(owner, RuntimeReflection.GetCombatState(owner)).NextAttackExtraScorch = amount;
    }

    public static int TryConsumeNextAttackExtraScorch(object owner)
    {
        var state = GetState(owner, RuntimeReflection.GetCombatState(owner));
        var amount = state.NextAttackExtraScorch;
        state.NextAttackExtraScorch = 0;
        return amount;
    }

    public static void RecordVirtualShaUse(object owner)
    {
        GetState(owner, RuntimeReflection.GetCombatState(owner)).ManualShaUsesThisTurn++;
    }

    public static void AddExtraShaUsesThisTurn(object owner, int amount)
    {
        GetState(owner, RuntimeReflection.GetCombatState(owner)).ExtraShaUsesThisTurn += amount;
    }

    public static bool TryConsumeQingLongTrigger(object owner)
    {
        var state = GetState(owner, RuntimeReflection.GetCombatState(owner));
        if (state.QingLongTriggeredThisTurn)
        {
            return false;
        }

        state.QingLongTriggeredThisTurn = true;
        return true;
    }
}
