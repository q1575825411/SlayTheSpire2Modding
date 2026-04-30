using System;
using System.Linq;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MyFirstStS2Mod.Scripts;
using MyFirstStS2Mod.Scripts.Cards;
using MyFirstStS2Mod.Scripts.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MyFirstStS2Mod.Scripts.Events;

[RegisterSharedEvent]
public sealed class YiZhanEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 2) && IsFirstVisit(runState)
        && RuntimeReflection.RunHasRelic<ZhengZhaoLingRelic>(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, AcceptSupplies, InitialOptionKey("ACCEPT_SUPPLIES")),
        new EventOption(this, RefuseOrders, InitialOptionKey("REFUSE_ORDERS")),
    ];

    private async Task AcceptSupplies()
    {
        await Act1EventHelpers.ChooseAndAddCardToDeck(Owner, Act3EventHelpers.GetAllEquipmentCards, "选择1张整备装备", 4);
        await Act1EventHelpers.ChooseAndUpgradeAnyCard(Owner, "选择1张牌升级");
        await Act1EventHelpers.ChooseAndRemoveCardFromDeck(Owner, "选择1张牌移除");
        RuntimeReflection.TryRestoreToFullHp(Owner?.Creature);
        RuntimeReflection.TryRemoveOwnedRelic<ZhengZhaoLingRelic>(Owner);
        SetEventFinished(PageDescription("ACCEPT_SUPPLIES_RESULT"));
    }

    private async Task RefuseOrders()
    {
        if (Owner?.Creature is not null)
        {
            await MegaCrit.Sts2.Core.Commands.CreatureCmd.Heal(Owner.Creature, 5, true);
        }

        SetEventFinished(PageDescription("REFUSE_ORDERS_RESULT"));
    }
}

[RegisterSharedEvent]
public sealed class DianPingEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 2) && IsFirstVisit(runState)
        && RuntimeReflection.RunHasRelic<JinNangCanYeRelic>(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, AcceptExchange, InitialOptionKey("ACCEPT_EXCHANGE")),
        new EventOption(this, DeclineExchange, InitialOptionKey("DECLINE_EXCHANGE")),
    ];

    private Task AcceptExchange()
    {
        RuntimeReflection.TryRemoveOwnedRelic<JinNangCanYeRelic>(Owner);
        RuntimeReflection.TryAddCardToMasterDeck<QiQiao>(Owner);
        SetEventFinished(PageDescription("ACCEPT_EXCHANGE_RESULT"));
        return Task.CompletedTask;
    }

    private Task DeclineExchange()
    {
        RuntimeReflection.TryModifyPlayerGold(Owner, 150);
        SetEventFinished(PageDescription("DECLINE_EXCHANGE_RESULT"));
        return Task.CompletedTask;
    }
}

internal static class Act3EventHelpers
{
    public static List<CardModel> GetAllEquipmentCards()
    {
        return typeof(MyFirstCard).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(EquipmentCard).IsAssignableFrom(type))
            .Select(type => Activator.CreateInstance(type) as CardModel)
            .Where(card => card is not null)
            .Select(card => card!)
            .ToList();
    }
}
