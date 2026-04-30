using System.Runtime.CompilerServices;
using MyFirstStS2Mod.Scripts;

namespace MyFirstStS2Mod.Scripts.Relics;

internal static class EventRelicState
{
    private sealed class OwnerState
    {
        public object? CombatStateRef { get; set; }
        public bool UsedFirstSkill { get; set; }
        public bool UsedFirstAttack { get; set; }
    }

    private static readonly ConditionalWeakTable<object, OwnerState> States = new();

    public static bool TryConsumeFirstSkill(object owner)
    {
        var state = GetState(owner);
        if (state.UsedFirstSkill)
        {
            return false;
        }

        state.UsedFirstSkill = true;
        return true;
    }

    public static bool TryConsumeFirstAttack(object owner)
    {
        var state = GetState(owner);
        if (state.UsedFirstAttack)
        {
            return false;
        }

        state.UsedFirstAttack = true;
        return true;
    }

    private static OwnerState GetState(object owner)
    {
        var state = States.GetOrCreateValue(owner);
        var combatState = RuntimeReflection.GetCombatState(owner);
        if (!ReferenceEquals(state.CombatStateRef, combatState))
        {
            state.CombatStateRef = combatState;
            state.UsedFirstSkill = false;
            state.UsedFirstAttack = false;
        }

        return state;
    }
}
