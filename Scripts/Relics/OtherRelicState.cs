using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Models;

namespace MyFirstStS2Mod.Scripts.Relics;

internal static class OtherRelicState
{
    private sealed class OwnerState
    {
        public object? CombatStateRef { get; set; }
        public bool FeatherArrowConsumed { get; set; }
        public bool ColdIronConsumed { get; set; }
    }

    private static readonly ConditionalWeakTable<object, OwnerState> States = new();

    public static bool CanUseFeatherArrow(object owner)
    {
        return !GetState(owner).FeatherArrowConsumed;
    }

    public static void MarkFeatherArrowConsumed(object owner)
    {
        GetState(owner).FeatherArrowConsumed = true;
    }

    public static bool TryConsumeColdIron(object owner)
    {
        var state = GetState(owner);
        if (state.ColdIronConsumed)
        {
            return false;
        }

        state.ColdIronConsumed = true;
        return true;
    }

    private static OwnerState GetState(object owner)
    {
        var state = States.GetOrCreateValue(owner);
        var combatState = RuntimeReflection.GetCombatState(owner);
        if (!ReferenceEquals(state.CombatStateRef, combatState))
        {
            state.CombatStateRef = combatState;
            state.FeatherArrowConsumed = false;
            state.ColdIronConsumed = false;
        }

        return state;
    }
}
