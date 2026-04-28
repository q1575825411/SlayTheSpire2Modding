using System.Collections;
using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace MyFirstStS2Mod.Scripts;

internal static class RuntimeReflection
{
    public static List<CardModel> GetHandCards(object? owner)
    {
        if (owner is null)
        {
            return [];
        }

        var playerCombatState = owner.GetType().GetProperty("PlayerCombatState")?.GetValue(owner);
        var hand = playerCombatState?.GetType().GetProperty("Hand")?.GetValue(playerCombatState);
        if (hand?.GetType().GetProperty("Cards")?.GetValue(hand) is not IEnumerable cards)
        {
            return [];
        }

        return cards.OfType<CardModel>().ToList();
    }

    public static bool HasCardInHand<TCard>(object? owner) where TCard : CardModel
    {
        return GetHandCards(owner).Any(card => card is TCard);
    }

    public static CardModel? FindFirstCardInHand<TCard>(object? owner, CardModel? excluding = null) where TCard : CardModel
    {
        return GetHandCards(owner).FirstOrDefault(card => card is TCard && card != excluding);
    }

    public static bool IsIntentAttack(object? creature)
    {
        if (creature is null)
        {
            return false;
        }

        var intent = creature.GetType().GetProperty("CurrentIntent")?.GetValue(creature)
            ?? creature.GetType().GetProperty("Intent")?.GetValue(creature)
            ?? creature.GetType().GetProperty("IntentModel")?.GetValue(creature);

        if (intent is null)
        {
            return false;
        }

        var name = intent.GetType().Name;
        if (name.Contains("Attack", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var attackProperty = intent.GetType().GetProperty("IsAttack");
        if (attackProperty?.GetValue(intent) is bool isAttack)
        {
            return isAttack;
        }

        return false;
    }

    public static IEnumerable<PowerModel> GetPositivePowers(object? creature)
    {
        if (creature is null)
        {
            return [];
        }

        var powers = creature.GetType().GetProperty("Powers")?.GetValue(creature) as IEnumerable;
        if (powers is null)
        {
            return [];
        }

        return powers.OfType<PowerModel>()
            .Where(power => power.Type == PowerType.Buff)
            .ToList();
    }

    public static decimal GetPowerAmount(PowerModel power)
    {
        return power.Amount;
    }

    public static async Task ApplyPowerByType(
        Type powerType,
        object target,
        decimal amount,
        object? applier,
        CardModel? cardSource)
    {
        var method = typeof(RuntimeReflection)
            .GetMethod(nameof(ApplyPowerGeneric), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(powerType);

        if (method.Invoke(null, [target, amount, applier, cardSource]) is Task task)
        {
            await task;
        }
    }

    private static Task ApplyPowerGeneric<TPower>(
        object target,
        decimal amount,
        object? applier,
        CardModel? cardSource) where TPower : PowerModel
    {
        return MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<TPower>(
            (dynamic)target,
            amount,
            (dynamic?)applier,
            cardSource);
    }
}
