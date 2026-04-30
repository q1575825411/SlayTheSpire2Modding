using System;
using System.Linq;
using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using STS2RitsuLib;

namespace MyFirstStS2Mod.Scripts.Cards;

internal static class QiQiaoRuntime
{
    private static readonly MethodInfo? OnPlayMethod = typeof(CardModel)
        .GetMethod("OnPlay", BindingFlags.Instance | BindingFlags.NonPublic);

    private static IDisposable? _cardPlayedSubscription;

    public static void Initialize()
    {
        _cardPlayedSubscription ??= RitsuLibFramework.SubscribeLifecycle<CardPlayedEvent>(OnCardPlayed);
    }

    private static void OnCardPlayed(CardPlayedEvent evt)
    {
        if (evt.CardPlay.Card is not CardModel card
            || card is QiQiao
            || card.Type != CardType.Skill
            || card.Owner is not Player owner)
        {
            return;
        }

        var qiQiaoPower = owner.Creature.Powers.OfType<Powers.QiQiaoPower>().FirstOrDefault();
        if (qiQiaoPower is null || OnPlayMethod is null)
        {
            return;
        }

        _ = ReplaySkillAndConsume(card, evt.CardPlay, qiQiaoPower);
    }

    private static async Task ReplaySkillAndConsume(CardModel card, CardPlay originalPlay, Powers.QiQiaoPower power)
    {
        try
        {
            if (OnPlayMethod.Invoke(card, [new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(), originalPlay]) is Task task)
            {
                await task;
            }

            await MegaCrit.Sts2.Core.Commands.PowerCmd.Remove(power);
        }
        catch (Exception ex)
        {
            Entry.Logger.LogError($"QiQiaoRuntime replay failed: {ex}");
        }
    }
}
