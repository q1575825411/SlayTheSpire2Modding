using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstStS2Mod.Scripts;
using MyFirstStS2Mod.Scripts.Cards;
using MyFirstStS2Mod.Scripts.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MyFirstStS2Mod.Scripts.Events;

[RegisterSharedEvent]
public sealed class WenJiEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 0) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        CreateModRelicOption<JinNangCanYeRelic>(OnReceiveRelic, "INITIAL"),
        new EventOption(this, RoughQuestioning, InitialOptionKey("ROUGH_QUESTIONING")),
    ];

    private Task OnReceiveRelic()
    {
        SetEventFinished(PageDescription("RECEIVED_RELIC"));
        return Task.CompletedTask;
    }

    private Task RoughQuestioning()
    {
        RuntimeReflection.TryModifyPlayerGold(Owner, 125);
        RuntimeReflection.TryAddCardToMasterDeck<AoHuiCurse>(Owner);
        SetEventFinished(PageDescription("ROUGH_QUESTIONING_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class JiuSiEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 0) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, DrinkDeeply, InitialOptionKey("DRINK_DEEPLY")),
        new EventOption(this, SipLightly, InitialOptionKey("SIP_LIGHTLY")),
        new EventOption(this, Decline, InitialOptionKey("DECLINE")),
    ];

    private Task DrinkDeeply()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -125))
        {
            return Task.CompletedTask;
        }

        RuntimeReflection.TryAddCardToMasterDeck<Jiu>(Owner, upgraded: true);
        SetEventFinished(PageDescription("DRINK_DEEPLY_RESULT"));
        return Task.CompletedTask;
    }

    private Task SipLightly()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -50))
        {
            return Task.CompletedTask;
        }

        RuntimeReflection.TryAddCardToMasterDeck<Jiu>(Owner);
        SetEventFinished(PageDescription("SIP_LIGHTLY_RESULT"));
        return Task.CompletedTask;
    }

    private Task Decline()
    {
        SetEventFinished(PageDescription("DECLINE_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class LiuMinYingEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 0) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, DistributeSupplies, InitialOptionKey("DISTRIBUTE_SUPPLIES")),
        new EventOption(this, ProtectSupplies, InitialOptionKey("PROTECT_SUPPLIES")),
    ];

    private Task DistributeSupplies()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -75))
        {
            return Task.CompletedTask;
        }

        RuntimeReflection.TryIncreaseMaxHp(Owner?.Creature, 5, alsoHeal: true);
        SetEventFinished(PageDescription("DISTRIBUTE_SUPPLIES_RESULT"));
        return Task.CompletedTask;
    }

    private async Task ProtectSupplies()
    {
        RuntimeReflection.TryModifyPlayerGold(Owner, 50);
        if (Owner?.Creature is not null)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 5m, ValueProp.Unblockable, null, null);
        }

        SetEventFinished(PageDescription("PROTECT_SUPPLIES_RESULT"));
    }
}

[RegisterSharedEvent]
public sealed class LuYuHaoJieEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 0) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        CreateModRelicOption<HaoJieRelic>(OnTravelTogether, "INITIAL"),
        CreateModRelicOption<HaoJieYouYiRelic>(OnDeclineRespectfully, "INITIAL"),
        new EventOption(this, SparOnce, InitialOptionKey("SPAR_ONCE")),
    ];

    private Task OnTravelTogether()
    {
        SetEventFinished(PageDescription("TRAVEL_TOGETHER_RESULT"));
        return Task.CompletedTask;
    }

    private Task OnDeclineRespectfully()
    {
        SetEventFinished(PageDescription("DECLINE_RESPECTFULLY_RESULT"));
        return Task.CompletedTask;
    }

    private Task SparOnce()
    {
        RuntimeReflection.TryUpgradeRandomCardInMasterDeck(Owner);
        SetEventFinished(PageDescription("SPAR_ONCE_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class GuanFuZhaoMuEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 0) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        CreateModRelicOption<ZhengZhaoLingRelic>(OnAcceptRecruitment, "INITIAL"),
        new EventOption(this, RefuseRecruitment, InitialOptionKey("REFUSE_RECRUITMENT")),
    ];

    private Task OnAcceptRecruitment()
    {
        SetEventFinished(PageDescription("ACCEPT_RECRUITMENT_RESULT"));
        return Task.CompletedTask;
    }

    private Task RefuseRecruitment()
    {
        RuntimeReflection.TryModifyPlayerGold(Owner, 125);
        SetEventFinished(PageDescription("REFUSE_RECRUITMENT_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class BingQiPuEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 0) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, PickOldWeapon, InitialOptionKey("PICK_OLD_WEAPON")),
        new EventOption(this, PrepareForBattle, InitialOptionKey("PREPARE_FOR_BATTLE")),
        new EventOption(this, Leave, InitialOptionKey("LEAVE")),
    ];

    private async Task PickOldWeapon()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -50))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return;
        }

        await Act1EventHelpers.ChooseAndAddCardToDeck(
            Owner,
            Act1EventHelpers.GetCommonWeaponCards,
            "选择1张旧兵器加入牌组",
            3);
        SetEventFinished(PageDescription("PICK_OLD_WEAPON_RESULT"));
    }

    private async Task PrepareForBattle()
    {
        if (Owner?.Creature is not null)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 6m, ValueProp.Unblockable, null, null);
        }

        await Act1EventHelpers.ChooseAndUpgradeAttackCard(Owner, "选择1张攻击牌升级");
        SetEventFinished(PageDescription("PREPARE_FOR_BATTLE_RESULT"));
    }

    private Task Leave()
    {
        SetEventFinished(PageDescription("LEAVE_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class GuZhanChangEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 0) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, PickWeapon, InitialOptionKey("PICK_WEAPON")),
        new EventOption(this, RepairArmor, InitialOptionKey("REPAIR_ARMOR")),
    ];

    private async Task PickWeapon()
    {
        if (Owner?.Creature is not null)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 5m, ValueProp.Unblockable, null, null);
        }

        Act1EventHelpers.AddRandomCardToDeck(Owner, Act1EventHelpers.GetCommonWeaponCards);
        SetEventFinished(PageDescription("PICK_WEAPON_RESULT"));
    }

    private async Task RepairArmor()
    {
        if (Owner?.Creature is not null)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 5m, ValueProp.Unblockable, null, null);
        }

        Act1EventHelpers.AddRandomCardToDeck(Owner, Act1EventHelpers.GetCommonArmorCards);
        SetEventFinished(PageDescription("REPAIR_ARMOR_RESULT"));
    }
}

[RegisterSharedEvent]
public sealed class LuYuShanZeiEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 0) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, PayToll, InitialOptionKey("PAY_TOLL")),
        new EventOption(this, ForceThrough, InitialOptionKey("FORCE_THROUGH")),
        new EventOption(this, StallForTime, InitialOptionKey("STALL_FOR_TIME")),
    ];

    private Task PayToll()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -50))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return Task.CompletedTask;
        }

        SetEventFinished(PageDescription("PAY_TOLL_RESULT"));
        return Task.CompletedTask;
    }

    private async Task ForceThrough()
    {
        if (Owner?.Creature is not null)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 6m, ValueProp.Unblockable, null, null);
        }

        SetEventFinished(PageDescription("FORCE_THROUGH_RESULT"));
    }

    private Task StallForTime()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -25))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return Task.CompletedTask;
        }

        Act1EventHelpers.AddRandomCardToDeck(Owner, Act1EventHelpers.GetCommonSoldierCards);
        SetEventFinished(PageDescription("STALL_FOR_TIME_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class LuanShiFengBoEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 0) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, WatchChaos, InitialOptionKey("WATCH_CHAOS")),
    ];

    private async Task WatchChaos()
    {
        var roll = Random.Shared.NextDouble();
        if (roll < 0.3d)
        {
            RuntimeReflection.TryUpgradeRandomCardInMasterDeck(Owner);
            SetEventFinished(PageDescription("UPGRADE_RESULT"));
            return;
        }

        if (roll < 0.6d)
        {
            await Act1EventHelpers.ChooseAndRemoveCardFromDeck(Owner, "选择1张牌移除");
            SetEventFinished(PageDescription("REMOVE_RESULT"));
            return;
        }

        RuntimeReflection.TryAddCardToMasterDeck<AoHuiCurse>(Owner);
        SetEventFinished(PageDescription("CURSE_RESULT"));
    }
}

internal static class Act1EventHelpers
{
    public static async Task ChooseAndAddCardToDeck(
        Player? owner,
        Func<List<CardModel>> candidateFactory,
        string prompt,
        int optionCount)
    {
        var cards = TakeRandomDistinctCards(candidateFactory(), optionCount);
        if (owner is null || cards.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(prompt, 1);
        var chosen = (await CardSelectCmd.FromSimpleGrid(new ThrowingPlayerChoiceContext(), cards, owner, prefs)).FirstOrDefault();
        if (chosen is not null)
        {
            RuntimeReflection.TryAddCardToMasterDeck(chosen, owner);
        }
    }

    public static async Task ChooseAndUpgradeAttackCard(object? owner, string prompt)
    {
        var candidates = RuntimeReflection.GetMasterDeckSnapshot(owner)
            .Where(card => card.Type == CardType.Attack && !RuntimeReflection.IsCardUpgraded(card))
            .ToList();
        if (owner is null || candidates.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(prompt, 1);
        var chosen = (await CardSelectCmd.FromSimpleGrid(new ThrowingPlayerChoiceContext(), candidates, owner, prefs)).FirstOrDefault();
        if (chosen is not null)
        {
            RuntimeReflection.TryUpgradeCard(chosen);
        }
    }

    public static async Task ChooseAndUpgradeAnyCard(object? owner, string prompt)
    {
        var candidates = RuntimeReflection.GetMasterDeckSnapshot(owner)
            .Where(card => !RuntimeReflection.IsCardUpgraded(card))
            .ToList();
        if (owner is null || candidates.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(prompt, 1);
        var chosen = (await CardSelectCmd.FromSimpleGrid(new ThrowingPlayerChoiceContext(), candidates, owner, prefs)).FirstOrDefault();
        if (chosen is not null)
        {
            RuntimeReflection.TryUpgradeCard(chosen);
        }
    }

    public static async Task ChooseAndRemoveCardFromDeck(object? owner, string prompt)
    {
        var candidates = RuntimeReflection.GetMasterDeckSnapshot(owner);
        candidates = candidates
            .Where(card => card is not YongHengCurse)
            .ToList();
        if (owner is null || candidates.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(prompt, 1);
        var chosen = (await CardSelectCmd.FromSimpleGrid(new ThrowingPlayerChoiceContext(), candidates, owner, prefs)).FirstOrDefault();
        if (chosen is not null)
        {
            RuntimeReflection.TryRemoveCardFromMasterDeck(owner, chosen);
        }
    }

    public static void AddRandomCardToDeck(object? owner, Func<List<CardModel>> candidateFactory)
    {
        var candidates = candidateFactory();
        if (candidates.Count == 0)
        {
            return;
        }

        RuntimeReflection.TryAddCardToMasterDeck(candidates[Random.Shared.Next(candidates.Count)], owner);
    }

    public static async Task ChooseAndAddUpgradedCardToDeck(
        Player? owner,
        Func<List<CardModel>> candidateFactory,
        string prompt,
        int optionCount)
    {
        var cards = TakeRandomDistinctCards(candidateFactory(), optionCount);
        foreach (var card in cards)
        {
            RuntimeReflection.TryUpgradeCard(card);
        }

        if (owner is null || cards.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(prompt, 1);
        var chosen = (await CardSelectCmd.FromSimpleGrid(new ThrowingPlayerChoiceContext(), cards, owner, prefs)).FirstOrDefault();
        if (chosen is not null)
        {
            RuntimeReflection.TryAddCardToMasterDeck(chosen, owner);
        }
    }

    public static List<CardModel> GetCommonWeaponCards() => GetCardsByFilter(card => card is WeaponCard && card.Rarity == CardRarity.Common);

    public static List<CardModel> GetCommonArmorCards() => GetCardsByFilter(card => card is ArmorCard && card.Rarity == CardRarity.Common);

    public static List<CardModel> GetCommonSoldierCards() =>
        GetCardsByFilter(card => card.Rarity == CardRarity.Common && card is not AoHuiCurse);

    private static List<CardModel> TakeRandomDistinctCards(List<CardModel> cards, int count)
    {
        return cards
            .OrderBy(_ => Random.Shared.Next())
            .Take(count)
            .ToList();
    }

    private static List<CardModel> GetCardsByFilter(Func<CardModel, bool> predicate)
    {
        return typeof(MyFirstCard).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(MyFirstCard).IsAssignableFrom(type))
            .Select(type => Activator.CreateInstance(type) as CardModel)
            .Where(card => card is not null)
            .Select(card => card!)
            .Where(predicate)
            .ToList();
    }
}
