using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib;

namespace MyFirstStS2Mod.Scripts.Relics;

public static class OtherRelicRuntime
{
    private static IDisposable? _cardPlayedSubscription;
    private static IDisposable? _cardDiscardedSubscription;
    private static IDisposable? _cardExhaustedSubscription;

    public static void Initialize()
    {
        _cardPlayedSubscription ??= RitsuLibFramework.SubscribeLifecycle<CardPlayedEvent>(OnCardPlayed);
        _cardDiscardedSubscription ??= RitsuLibFramework.SubscribeLifecycle<CardDiscardedEvent>(OnCardDiscarded);
        _cardExhaustedSubscription ??= RitsuLibFramework.SubscribeLifecycle<CardExhaustedEvent>(OnCardExhausted);
    }

    private static void OnCardPlayed(CardPlayedEvent evt)
    {
        if (evt.CardPlay.Card is not CardModel card || card.Owner is null)
        {
            return;
        }

        if (card is Cards.ShaCard)
        {
            OtherRelicState.MarkFeatherArrowConsumed(card.Owner);
        }
    }

    private static void OnCardDiscarded(CardDiscardedEvent evt)
    {
        if (evt.Card is not Cards.ShaCard sha || sha.Owner is null)
        {
            return;
        }

        if (!OtherRelicChecks.HasRelic<TigerTallyRelic>(sha.Owner))
        {
            return;
        }

        RuntimeReflection.TryMoveCardFromDiscardToDrawPile(sha.Owner, sha, shuffleIntoDrawPile: true);
    }

    private static void OnCardExhausted(CardExhaustedEvent evt)
    {
        if (evt.Card is not Cards.Jiu jiu || jiu.Owner is null)
        {
            return;
        }

        if (!OtherRelicChecks.HasRelic<WinePouchRelic>(jiu.Owner))
        {
            return;
        }

        RuntimeReflection.TryMoveCardFromExhaustToDiscard(jiu.Owner, jiu);
    }
}
