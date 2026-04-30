using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib;
using MyFirstStS2Mod.Scripts.Cards;
using MyFirstStS2Mod.Scripts;

namespace MyFirstStS2Mod.Scripts.Relics;

public static class EventRelicRuntime
{
    private static IDisposable? _cardPlayedSubscription;
    private static IDisposable? _combatVictorySubscription;

    public static void Initialize()
    {
        _cardPlayedSubscription ??= RitsuLibFramework.SubscribeLifecycle<CardPlayedEvent>(OnCardPlayed);
        _combatVictorySubscription ??= RitsuLibFramework.SubscribeLifecycle<CombatVictoryEvent>(OnCombatVictory);
    }

    private static void OnCardPlayed(CardPlayedEvent evt)
    {
        if (evt.CardPlay.Card is not CardModel card || card.Owner is not Player owner)
        {
            return;
        }

        _ = HandleCardPlayed(card, owner);
    }

    private static async Task HandleCardPlayed(CardModel card, Player owner)
    {
        try
        {
            if (IsJinNangCard(card)
                && OtherRelicChecks.HasRelic<JinNangCanYeRelic>(owner)
                && EventRelicState.TryConsumeFirstSkill(owner))
            {
                await CardPileCmd.Draw(new ThrowingPlayerChoiceContext(), 1, owner);
            }

            if (card.Type == CardType.Attack
                && OtherRelicChecks.HasRelic<HaoJieRelic>(owner)
                && EventRelicState.TryConsumeFirstAttack(owner))
            {
                await PowerCmd.Apply<StrengthPower>(owner.Creature, 1, owner.Creature, null);
            }
        }
        catch (Exception ex)
        {
            Entry.Logger.LogError($"EventRelicRuntime.HandleCardPlayed failed: {ex}");
        }
    }

    private static bool IsJinNangCard(CardModel card)
    {
        return card.Type == CardType.Skill
            && card is not Shan
            && card is not Tao
            && card is not Jiu
            && card is not AoHuiCurse
            && card is not EquipmentCard;
    }

    private static void OnCombatVictory(CombatVictoryEvent evt)
    {
        foreach (var player in evt.RunState.Players)
        {
            if (!OtherRelicChecks.HasRelic<ZhengZhaoLingRelic>(player))
            {
                continue;
            }

            RuntimeReflection.TryModifyPlayerGold(player, 10);
        }
    }
}
