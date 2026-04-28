using System.Collections;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

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

    public static object? GetCombatState(object? owner)
    {
        return owner?.GetType().GetProperty("CombatState")?.GetValue(owner);
    }

    public static int GetCombatTurnToken(object? combatState)
    {
        if (combatState is null)
        {
            return 0;
        }

        foreach (var propertyName in new[] { "Turn", "TurnNumber", "TurnIndex", "Round", "RoundNumber" })
        {
            var value = combatState.GetType().GetProperty(propertyName)?.GetValue(combatState);
            if (value is int intValue)
            {
                return intValue;
            }

            if (value is decimal decimalValue)
            {
                return (int)decimalValue;
            }
        }

        return 0;
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

    public static decimal GetCurrentBlock(object? creature)
    {
        if (creature is null)
        {
            return 0m;
        }

        foreach (var propertyName in new[] { "CurrentBlock", "Block", "CurrentShield" })
        {
            var value = creature.GetType().GetProperty(propertyName)?.GetValue(creature);
            if (value is decimal decimalValue)
            {
                return decimalValue;
            }

            if (value is int intValue)
            {
                return intValue;
            }
        }

        return 0m;
    }

    public static List<Creature> GetLivingOpponents(object? owner)
    {
        var combatState = GetCombatState(owner);
        var method = combatState?.GetType().GetMethod("GetOpponentsOf");
        if (combatState is null || owner is null || method is null)
        {
            return [];
        }

        if (method.Invoke(combatState, [owner]) is not IEnumerable opponents)
        {
            return [];
        }

        return opponents.OfType<Creature>()
            .Where(creature => creature.IsAlive)
            .ToList();
    }

    public static async Task<bool> TryDiscardCards(
        PlayerChoiceContext choiceContext,
        object? owner,
        int count,
        CardModel? excluding = null)
    {
        var cards = GetHandCards(owner)
            .Where(card => card != excluding)
            .Take(count)
            .ToList();

        if (cards.Count < count)
        {
            return false;
        }

        foreach (var card in cards)
        {
            if (!await TryDiscardSingleCard(choiceContext, card))
            {
                return false;
            }
        }

        return true;
    }

    public static async Task DiscardAllHandCards(PlayerChoiceContext choiceContext, object? owner)
    {
        foreach (var card in GetHandCards(owner).ToList())
        {
            await TryDiscardSingleCard(choiceContext, card);
        }
    }

    public static async Task RetainUpToCardsInHand(object? owner, int count)
    {
        foreach (var card in GetHandCards(owner).Take(count))
        {
            TrySetCardRetain(card, true);
        }

        await Task.CompletedTask;
    }

    public static async Task RemovePowersOfType(object? creature, Type powerType)
    {
        if (creature is null)
        {
            return;
        }

        var powers = creature.GetType().GetProperty("Powers")?.GetValue(creature) as IEnumerable;
        if (powers is null)
        {
            return;
        }

        foreach (var power in powers.OfType<PowerModel>().Where(power => power.GetType() == powerType).ToList())
        {
            await PowerCmd.Remove((dynamic)power);
        }
    }

    public static async Task RemoveDebuffPowers(object? creature)
    {
        if (creature is null)
        {
            return;
        }

        var powers = creature.GetType().GetProperty("Powers")?.GetValue(creature) as IEnumerable;
        if (powers is null)
        {
            return;
        }

        foreach (var power in powers.OfType<PowerModel>().Where(power => power.Type == PowerType.Debuff).ToList())
        {
            await PowerCmd.Remove((dynamic)power);
        }
    }

    public static int GetCurrentEnergy(object? playerCombatState)
    {
        if (playerCombatState is null)
        {
            return 0;
        }

        foreach (var propertyName in new[] { "Energy", "CurrentEnergy", "EnergyCount" })
        {
            var value = playerCombatState.GetType().GetProperty(propertyName)?.GetValue(playerCombatState);
            if (value is int intValue)
            {
                return intValue;
            }

            if (value is decimal decimalValue)
            {
                return (int)decimalValue;
            }
        }

        return 0;
    }

    public static void TrySetCardCostForTurn(CardModel card, int cost)
    {
        foreach (var propertyName in new[] { "Cost", "CostForTurn", "CurrentCost" })
        {
            var property = card.GetType().GetProperty(propertyName);
            if (property?.CanWrite == true)
            {
                property.SetValue(card, cost);
                return;
            }
        }
    }

    public static void TrySetCardRetain(CardModel card, bool retain)
    {
        foreach (var propertyName in new[] { "Retain", "ShouldRetain", "RetainedThisTurn" })
        {
            var property = card.GetType().GetProperty(propertyName);
            if (property?.CanWrite == true && property.PropertyType == typeof(bool))
            {
                property.SetValue(card, retain);
                return;
            }
        }
    }

    public static void TrySetCardExhaust(CardModel card, bool exhaust)
    {
        foreach (var propertyName in new[] { "Exhaust", "ShouldExhaust", "Exhausts" })
        {
            var property = card.GetType().GetProperty(propertyName);
            if (property?.CanWrite == true && property.PropertyType == typeof(bool))
            {
                property.SetValue(card, exhaust);
                return;
            }
        }
    }

    public static void TrySetCardPlayable(CardModel card, bool playable)
    {
        foreach (var propertyName in new[] { "Playable", "CanPlay", "CanBePlayed" })
        {
            var property = card.GetType().GetProperty(propertyName);
            if (property?.CanWrite == true && property.PropertyType == typeof(bool))
            {
                property.SetValue(card, playable);
                return;
            }
        }
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

    private static async Task<bool> TryDiscardSingleCard(PlayerChoiceContext choiceContext, CardModel card)
    {
        var cardCmdType = typeof(CardCmd);
        foreach (var methodName in new[] { "Discard", "MoveToDiscard" })
        {
            var method = cardCmdType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(candidate => candidate.Name == methodName && candidate.GetParameters().Length == 2);

            if (method is null)
            {
                continue;
            }

            if (method.Invoke(null, [choiceContext, card]) is Task task)
            {
                await task;
                return true;
            }
        }

        return false;
    }
}
