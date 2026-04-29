using System.Collections;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MyFirstStS2Mod.Scripts.Characters;

namespace MyFirstStS2Mod.Scripts;

internal static class RuntimeReflection
{
    public static object? GetPlayerCombatState(object? owner)
    {
        return owner?.GetType().GetProperty("PlayerCombatState")?.GetValue(owner);
    }

    public static object? GetCharacterModel(object? owner)
    {
        if (owner is null)
        {
            return null;
        }

        foreach (var target in new[] { owner, GetPlayerCombatState(owner) })
        {
            if (target is null)
            {
                continue;
            }

            foreach (var propertyName in new[] { "Character", "CharacterModel" })
            {
                var value = target.GetType().GetProperty(propertyName)?.GetValue(target);
                if (value is not null)
                {
                    return value;
                }
            }
        }

        return null;
    }

    public static bool IsOwnedByCharacterType<TCharacter>(object? owner)
    {
        return GetCharacterModel(owner) is TCharacter;
    }

    public static bool IsEquipmentEnabledForOwner(object? owner)
    {
        return IsOwnedByCharacterType<SoldierCharacter>(owner);
    }

    public static List<CardModel> GetHandCards(object? owner)
    {
        if (owner is null)
        {
            return [];
        }

        var playerCombatState = GetPlayerCombatState(owner);
        var hand = playerCombatState?.GetType().GetProperty("Hand")?.GetValue(playerCombatState);
        if (hand?.GetType().GetProperty("Cards")?.GetValue(hand) is not IEnumerable cards)
        {
            return [];
        }

        return cards.OfType<CardModel>().ToList();
    }

    public static List<RelicModel> GetOwnedRelics(object? owner)
    {
        if (owner is null)
        {
            return [];
        }

        if (owner.GetType().GetProperty("Relics")?.GetValue(owner) is not IEnumerable relics)
        {
            return [];
        }

        return relics.OfType<RelicModel>().ToList();
    }

    public static bool HasCardInHand<TCard>(object? owner) where TCard : CardModel
    {
        return GetHandCards(owner).Any(card => card is TCard);
    }

    public static CardModel? FindFirstCardInHand<TCard>(object? owner, CardModel? excluding = null) where TCard : CardModel
    {
        return GetHandCards(owner).FirstOrDefault(card => card is TCard && card != excluding);
    }

    public static List<CardModel> GetExhaustPileCards(object? owner)
    {
        var playerCombatState = GetPlayerCombatState(owner);
        if (playerCombatState is null)
        {
            return [];
        }

        foreach (var propertyName in new[] { "ExhaustPile", "ExhaustedPile", "Exhaust", "Exhausted" })
        {
            var pile = playerCombatState.GetType().GetProperty(propertyName)?.GetValue(playerCombatState);
            var cards = pile?.GetType().GetProperty("Cards")?.GetValue(pile) as IEnumerable;
            if (cards is not null)
            {
                return cards.OfType<CardModel>().ToList();
            }
        }

        return [];
    }

    public static int GetExhaustPileCount(object? owner)
    {
        return GetExhaustPileCards(owner).Count;
    }

    public static List<CardModel> GetDiscardPileCards(object? owner)
    {
        return GetCardsFromNamedPile(owner, "DiscardPile", "Discard");
    }

    public static List<CardModel> GetDrawPileCards(object? owner)
    {
        return GetCardsFromNamedPile(owner, "DrawPile", "Draw");
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

    public static decimal GetCurrentHp(object? creature)
    {
        if (creature is null)
        {
            return 0m;
        }

        foreach (var propertyName in new[] { "CurrentHp", "CurrentHealth", "Hp", "Health" })
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

    public static decimal GetMaxHp(object? creature)
    {
        if (creature is null)
        {
            return 0m;
        }

        foreach (var propertyName in new[] { "MaxHp", "MaxHealth", "MaxLife", "HealthMax" })
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

    public static bool TryIncreaseMaxHp(object? creature, int amount, bool alsoHeal = true)
    {
        if (creature is null || amount == 0)
        {
            return false;
        }

        var updated = false;
        foreach (var propertyName in new[] { "MaxHp", "MaxHealth", "MaxLife", "HealthMax" })
        {
            var property = creature.GetType().GetProperty(propertyName);
            if (property?.CanWrite != true)
            {
                continue;
            }

            var currentValue = property.GetValue(creature);
            if (currentValue is decimal decimalValue)
            {
                property.SetValue(creature, decimalValue + amount);
                updated = true;
                break;
            }

            if (currentValue is int intValue)
            {
                property.SetValue(creature, intValue + amount);
                updated = true;
                break;
            }
        }

        if (!updated)
        {
            return false;
        }

        if (alsoHeal)
        {
            foreach (var propertyName in new[] { "CurrentHp", "CurrentHealth", "Hp", "Health" })
            {
                var property = creature.GetType().GetProperty(propertyName);
                if (property?.CanWrite != true)
                {
                    continue;
                }

                var currentValue = property.GetValue(creature);
                if (currentValue is decimal decimalValue)
                {
                    property.SetValue(creature, decimalValue + amount);
                    break;
                }

                if (currentValue is int intValue)
                {
                    property.SetValue(creature, intValue + amount);
                    break;
                }
            }
        }

        return true;
    }

    public static int GetCurrentActIndex(object? owner)
    {
        var runState = owner?.GetType().GetProperty("RunState")?.GetValue(owner)
            ?? owner?.GetType().GetProperty("Run")?.GetValue(owner);

        if (runState is null)
        {
            return -1;
        }

        foreach (var propertyName in new[] { "CurrentActIndex", "ActIndex", "CurrentAct", "ActNumber" })
        {
            var value = runState.GetType().GetProperty(propertyName)?.GetValue(runState);
            if (value is int intValue)
            {
                return intValue;
            }

            if (value is decimal decimalValue)
            {
                return (int)decimalValue;
            }
        }

        return -1;
    }

    public static int GetCombatRoundNumber(object? combatState)
    {
        if (combatState is null)
        {
            return 0;
        }

        foreach (var propertyName in new[] { "RoundNumber", "Round", "Turn", "TurnNumber", "TurnIndex" })
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

    public static int CountCardsOwnedBy<TCard>(object? owner) where TCard : CardModel
    {
        var masterDeck = GetMasterDeckCards(owner);
        if (masterDeck.Count > 0)
        {
            return masterDeck.Count(card => card is TCard);
        }

        return GetHandCards(owner).Concat(GetDrawPileCards(owner)).Concat(GetDiscardPileCards(owner)).Concat(GetExhaustPileCards(owner))
            .Count(card => card is TCard);
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

    public static TPower? GetPower<TPower>(object? creature) where TPower : PowerModel
    {
        if (creature is null)
        {
            return null;
        }

        var powers = creature.GetType().GetProperty("Powers")?.GetValue(creature) as IEnumerable;
        return powers?.OfType<TPower>().FirstOrDefault();
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

    public static int GetCardDamageBaseValue(CardModel? card)
    {
        if (card is null)
        {
            return 0;
        }

        var dynamicVars = card.GetType().GetProperty("DynamicVars")?.GetValue(card);
        var damageVar = dynamicVars?.GetType().GetProperty("Damage")?.GetValue(dynamicVars);
        foreach (var propertyName in new[] { "BaseValue", "IntValue", "Value" })
        {
            var value = damageVar?.GetType().GetProperty(propertyName)?.GetValue(damageVar);
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

    public static int GetCardBaseCost(CardModel? card)
    {
        if (card is null)
        {
            return 0;
        }

        var energyCost = card.GetType().GetProperty("EnergyCost")?.GetValue(card);
        foreach (var propertyName in new[] { "BaseValue", "IntValue", "Value" })
        {
            var value = energyCost?.GetType().GetProperty(propertyName)?.GetValue(energyCost);
            if (value is int intValue)
            {
                return intValue;
            }

            if (value is decimal decimalValue)
            {
                return (int)decimalValue;
            }
        }

        foreach (var propertyName in new[] { "BaseCost", "OriginalCost", "Cost" })
        {
            var value = card.GetType().GetProperty(propertyName)?.GetValue(card);
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

    public static void TryReduceCardCostForTurn(CardModel card, int amount)
    {
        var currentCost = GetCardBaseCost(card);
        TrySetCardCostForTurn(card, Math.Max(0, currentCost - amount));
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

    public static bool IsCardUpgraded(CardModel card)
    {
        foreach (var propertyName in new[] { "IsUpgraded", "Upgraded" })
        {
            var value = card.GetType().GetProperty(propertyName)?.GetValue(card);
            if (value is bool boolValue)
            {
                return boolValue;
            }
        }

        return false;
    }

    public static void TryUpgradeCard(CardModel card)
    {
        foreach (var methodName in new[] { "Upgrade", "TryUpgrade" })
        {
            var method = card.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (method is not null && method.GetParameters().Length == 0)
            {
                method.Invoke(card, null);
                return;
            }
        }
    }

    public static bool TryAddCardCopyToHand(CardModel source, object? owner, bool exhaust = false, bool retain = false)
    {
        CardModel? copy;
        try
        {
            copy = Activator.CreateInstance(source.GetType()) as CardModel;
        }
        catch
        {
            return false;
        }

        if (copy is null)
        {
            return false;
        }

        if (IsCardUpgraded(source))
        {
            TryUpgradeCard(copy);
        }

        return TryAddCardToHand(copy, owner, exhaust, retain);
    }

    public static bool TryAddNewCardToHand<TCard>(object? owner, bool upgraded = false, bool exhaust = false, bool retain = false)
        where TCard : CardModel, new()
    {
        var card = new TCard();
        if (upgraded)
        {
            TryUpgradeCard(card);
        }

        return TryAddCardToHand(card, owner, exhaust, retain);
    }

    public static bool TryAddCardToHand(CardModel card, object? owner, bool exhaust = false, bool retain = false)
    {
        if (owner is null)
        {
            return false;
        }

        var playerCombatState = GetPlayerCombatState(owner);
        var combatState = GetCombatState(owner);
        TrySetWritableProperty(card, "Owner", owner);
        TrySetWritableProperty(card, "CreatureOwner", owner);
        TrySetWritableProperty(card, "CombatState", combatState);
        TrySetWritableProperty(card, "PlayerCombatState", playerCombatState);
        TrySetCardExhaust(card, exhaust);
        TrySetCardRetain(card, retain);

        var hand = playerCombatState?.GetType().GetProperty("Hand")?.GetValue(playerCombatState);
        if (hand is not null)
        {
            foreach (var methodName in new[] { "AddCard", "Add", "InsertCard" })
            {
                var method = hand.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(candidate =>
                        candidate.Name == methodName
                        && candidate.GetParameters().Length == 1
                        && candidate.GetParameters()[0].ParameterType.IsAssignableFrom(card.GetType()));

                if (method is not null)
                {
                    method.Invoke(hand, [card]);
                    return true;
                }
            }

            if (hand.GetType().GetProperty("Cards")?.GetValue(hand) is IList list)
            {
                list.Add(card);
                return true;
            }
        }

        return false;
    }

    public static bool TryMoveCardFromDiscardToDrawPile(object? owner, CardModel card, bool shuffleIntoDrawPile)
    {
        return TryMoveCardBetweenPiles(owner, card, new[] { "DiscardPile", "Discard" }, new[] { "DrawPile", "Draw" }, shuffleIntoDrawPile);
    }

    public static bool TryMoveCardFromExhaustToDiscard(object? owner, CardModel card)
    {
        return TryMoveCardBetweenPiles(owner, card, new[] { "ExhaustPile", "ExhaustedPile", "Exhaust", "Exhausted" }, new[] { "DiscardPile", "Discard" }, false);
    }

    public static bool IsMerchantEntryPurchased(object? entry)
    {
        if (entry is null)
        {
            return false;
        }

        foreach (var propertyName in new[] { "IsSoldOut", "SoldOut", "IsPurchased", "Purchased", "WasPurchased" })
        {
            var value = entry.GetType().GetProperty(propertyName)?.GetValue(entry);
            if (value is bool boolValue && boolValue)
            {
                return true;
            }
        }

        return false;
    }

    public static bool TryEndTurn(PlayerChoiceContext choiceContext, object? owner)
    {
        foreach (var target in new[] { GetPlayerCombatState(owner), GetCombatState(owner), owner })
        {
            if (target is null)
            {
                continue;
            }

            foreach (var methodName in new[] { "EndTurn", "TryEndTurn", "RequestEndTurn" })
            {
                var methods = target.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(candidate => candidate.Name == methodName);

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();
                    if (parameters.Length == 0)
                    {
                        method.Invoke(target, null);
                        return true;
                    }

                    if (parameters.Length == 1 && parameters[0].ParameterType.IsInstanceOfType(choiceContext))
                    {
                        method.Invoke(target, [choiceContext]);
                        return true;
                    }
                }
            }
        }

        return false;
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

    private static void TrySetWritableProperty(object target, string propertyName, object? value)
    {
        var property = target.GetType().GetProperty(propertyName);
        if (property?.CanWrite == true)
        {
            property.SetValue(target, value);
        }
    }

    private static List<CardModel> GetCardsFromNamedPile(object? owner, params string[] propertyNames)
    {
        var playerCombatState = GetPlayerCombatState(owner);
        if (playerCombatState is null)
        {
            return [];
        }

        foreach (var propertyName in propertyNames)
        {
            var pile = playerCombatState.GetType().GetProperty(propertyName)?.GetValue(playerCombatState);
            var cards = pile?.GetType().GetProperty("Cards")?.GetValue(pile) as IEnumerable;
            if (cards is not null)
            {
                return cards.OfType<CardModel>().ToList();
            }
        }

        return [];
    }

    private static List<CardModel> GetMasterDeckCards(object? owner)
    {
        foreach (var target in new[] { owner, owner?.GetType().GetProperty("RunState")?.GetValue(owner) })
        {
            if (target is null)
            {
                continue;
            }

            foreach (var propertyName in new[] { "MasterDeck", "Deck", "Cards" })
            {
                var container = target.GetType().GetProperty(propertyName)?.GetValue(target);
                if (container is IEnumerable direct)
                {
                    var cards = direct.OfType<CardModel>().ToList();
                    if (cards.Count > 0)
                    {
                        return cards;
                    }
                }

                if (container?.GetType().GetProperty("Cards")?.GetValue(container) is IEnumerable nested)
                {
                    var cards = nested.OfType<CardModel>().ToList();
                    if (cards.Count > 0)
                    {
                        return cards;
                    }
                }
            }
        }

        return [];
    }

    private static bool TryMoveCardBetweenPiles(
        object? owner,
        CardModel card,
        IReadOnlyList<string> sourcePropertyNames,
        IReadOnlyList<string> destinationPropertyNames,
        bool shuffleIntoDestination)
    {
        var playerCombatState = GetPlayerCombatState(owner);
        if (playerCombatState is null)
        {
            return false;
        }

        IList? sourceList = null;
        foreach (var propertyName in sourcePropertyNames)
        {
            var pile = playerCombatState.GetType().GetProperty(propertyName)?.GetValue(playerCombatState);
            if (pile?.GetType().GetProperty("Cards")?.GetValue(pile) is IList list && list.Contains(card))
            {
                sourceList = list;
                break;
            }
        }

        if (sourceList is null)
        {
            return false;
        }

        IList? destinationList = null;
        foreach (var propertyName in destinationPropertyNames)
        {
            var pile = playerCombatState.GetType().GetProperty(propertyName)?.GetValue(playerCombatState);
            if (pile?.GetType().GetProperty("Cards")?.GetValue(pile) is IList list)
            {
                destinationList = list;
                break;
            }
        }

        if (destinationList is null)
        {
            return false;
        }

        sourceList.Remove(card);
        if (shuffleIntoDestination)
        {
            destinationList.Insert(Random.Shared.Next(destinationList.Count + 1), card);
        }
        else
        {
            destinationList.Add(card);
        }

        return true;
    }
}
