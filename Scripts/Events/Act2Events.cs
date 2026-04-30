using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstStS2Mod.Scripts;
using MyFirstStS2Mod.Scripts.Cards;
using MyFirstStS2Mod.Scripts.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace MyFirstStS2Mod.Scripts.Events;

[RegisterSharedEvent]
public sealed class ZhengZhaoRuWuEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState)
        && RuntimeReflection.RunHasRelic<ZhengZhaoLingRelic>(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, SuppressUprising, InitialOptionKey("SUPPRESS_UPRISING")),
        new EventOption(this, EscortSupplies, InitialOptionKey("ESCORT_SUPPLIES")),
        new EventOption(this, BribeYourWayOut, InitialOptionKey("BRIBE_YOUR_WAY_OUT")),
    ];

    private Task SuppressUprising()
    {
        SetEventFinished(PageDescription("SUPPRESS_UPRISING_TODO"));
        return Task.CompletedTask;
    }

    private Task EscortSupplies()
    {
        SetEventFinished(PageDescription("ESCORT_SUPPLIES_TODO"));
        return Task.CompletedTask;
    }

    private Task BribeYourWayOut()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -175))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return Task.CompletedTask;
        }

        RuntimeReflection.TryRemoveOwnedRelic<ZhengZhaoLingRelic>(Owner);
        SetEventFinished(PageDescription("BRIBE_YOUR_WAY_OUT_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class YiZhuangBiYuEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState)
        && !RuntimeReflection.RunHasRelic<HaoJieRelic>(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return Owner is not null && OtherRelicChecks.HasRelic<HaoJieYouYiRelic>(Owner)
            ? [new EventOption(this, BuryTheHero, InitialOptionKey("BURY_THE_HERO"))]
            : [new EventOption(this, MournSilently, InitialOptionKey("MOURN_SILENTLY"))];
    }

    private async Task BuryTheHero()
    {
        RuntimeReflection.TryRemoveOwnedRelic<HaoJieYouYiRelic>(Owner);
        await Act1EventHelpers.ChooseAndUpgradeAnyCard(Owner, "选择1张牌升级");
        await Act1EventHelpers.ChooseAndRemoveCardFromDeck(Owner, "选择1张牌移除");
        SetEventFinished(PageDescription("BURY_THE_HERO_RESULT"));
    }

    private Task MournSilently()
    {
        RuntimeReflection.TryModifyPlayerGold(Owner, 75);
        SetEventFinished(PageDescription("MOURN_SILENTLY_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class ShanZeiDeBaoFuEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState)
        && RuntimeReflection.RunHasRelic<HaoJieRelic>(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, EscapeWounded, InitialOptionKey("ESCAPE_WOUNDED")),
    ];

    private async Task EscapeWounded()
    {
        RuntimeReflection.TryRemoveOwnedRelic<HaoJieRelic>(Owner);
        RuntimeReflection.TryModifyPlayerGold(Owner, -75);
        if (Owner?.Creature is not null)
        {
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner.Creature, 10m, ValueProp.Unblockable, null, null);
        }

        SetEventFinished(PageDescription("ESCAPE_WOUNDED_RESULT"));
    }
}

[RegisterSharedEvent]
public sealed class HuangJinBuDaoEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, DrinkTheCharmWater, InitialOptionKey("DRINK_THE_CHARM_WATER")),
        new EventOption(this, RefuseAndTalk, InitialOptionKey("REFUSE_AND_TALK")),
        new EventOption(this, RebukeZhangJiao, InitialOptionKey("REBUKE_ZHANG_JIAO")),
    ];

    private Task DrinkTheCharmWater()
    {
        RuntimeReflection.TryRestoreToFullHp(Owner?.Creature);
        RuntimeReflection.TryAddCardToMasterDeck<YongHengCurse>(Owner);
        SetEventFinished(PageDescription("DRINK_THE_CHARM_WATER_RESULT"));
        return Task.CompletedTask;
    }

    private Task RefuseAndTalk()
    {
        RuntimeReflection.TryAddCardToMasterDeck<TaiPingYaoShu>(Owner);
        SetEventFinished(PageDescription("REFUSE_AND_TALK_RESULT"));
        return Task.CompletedTask;
    }

    private Task RebukeZhangJiao()
    {
        RuntimeReflection.TryModifyPlayerGold(Owner, 150);
        SetEventFinished(PageDescription("REBUKE_ZHANG_JIAO_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class ChuQiangFuRuoEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, HelpDistribute, InitialOptionKey("HELP_DISTRIBUTE")),
        new EventOption(this, BlendIntoTheCrowd, InitialOptionKey("BLEND_INTO_THE_CROWD")),
        new EventOption(this, DonateYourOwnGold, InitialOptionKey("DONATE_YOUR_OWN_GOLD")),
    ];

    private Task HelpDistribute()
    {
        RuntimeReflection.TryIncreaseMaxHp(Owner?.Creature, 6, alsoHeal: true);
        SetEventFinished(PageDescription("HELP_DISTRIBUTE_RESULT"));
        return Task.CompletedTask;
    }

    private Task BlendIntoTheCrowd()
    {
        RuntimeReflection.TryModifyPlayerGold(Owner, 125);
        SetEventFinished(PageDescription("BLEND_INTO_THE_CROWD_RESULT"));
        return Task.CompletedTask;
    }

    private Task DonateYourOwnGold()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -150))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return Task.CompletedTask;
        }

        RuntimeReflection.TryAddRelic<HuangJinShanYiRelic>(Owner);
        SetEventFinished(PageDescription("DONATE_YOUR_OWN_GOLD_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class HuangJinJieLueEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, InterveneNow, InitialOptionKey("INTERVENE_NOW")),
        new EventOption(this, WitnessAndLeave, InitialOptionKey("WITNESS_AND_LEAVE")),
    ];

    private Task InterveneNow()
    {
        SetEventFinished(PageDescription("INTERVENE_NOW_TODO"));
        return Task.CompletedTask;
    }

    private Task WitnessAndLeave()
    {
        RuntimeReflection.TryModifyPlayerGold(Owner, 250);
        RuntimeReflection.TryAddCardToMasterDeck<LengYanCurse>(Owner);
        SetEventFinished(PageDescription("WITNESS_AND_LEAVE_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class TaoYuanJieYiEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, PraiseTheirOath, InitialOptionKey("PRAISE_THEIR_OATH")),
        new EventOption(this, RemainUnmoved, InitialOptionKey("REMAIN_UNMOVED")),
    ];

    private Task PraiseTheirOath()
    {
        RuntimeReflection.TryAddCardToMasterDeck<TaoYuanJieYi>(Owner);
        SetEventFinished(PageDescription("PRAISE_THEIR_OATH_RESULT"));
        return Task.CompletedTask;
    }

    private async Task RemainUnmoved()
    {
        await Act1EventHelpers.ChooseAndRemoveCardFromDeck(Owner, "选择1张牌移除");
        SetEventFinished(PageDescription("REMAIN_UNMOVED_RESULT"));
    }
}

[RegisterSharedEvent]
public sealed class YiZheNanXunEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, TakeTheHerbs, InitialOptionKey("TAKE_THE_HERBS")),
        new EventOption(this, ReceiveAPotion, InitialOptionKey("RECEIVE_A_POTION")),
    ];

    private async Task TakeTheHerbs()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -60))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return;
        }

        await CreatureCmd.Heal(Owner!.Creature, 6, true);
        SetEventFinished(PageDescription("TAKE_THE_HERBS_RESULT"));
    }

    private async Task ReceiveAPotion()
    {
        await RewardsCmd.OfferCustom(Owner!, [new PotionReward(Owner!)]);
        SetEventFinished(PageDescription("RECEIVE_A_POTION_RESULT"));
    }
}

[RegisterSharedEvent]
public sealed class JunHuoFanZiEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, BuyWeapon, InitialOptionKey("BUY_WEAPON")),
        new EventOption(this, BuyArmor, InitialOptionKey("BUY_ARMOR")),
        new EventOption(this, BuyMount, InitialOptionKey("BUY_MOUNT")),
        new EventOption(this, Leave, InitialOptionKey("LEAVE")),
    ];

    private async Task BuyWeapon()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -200))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return;
        }

        await Act1EventHelpers.ChooseAndAddCardToDeck(Owner, Act2EventHelpers.GetUncommonWeaponCards, "选择1张银色武器牌", 3);
        SetEventFinished(PageDescription("BUY_WEAPON_RESULT"));
    }

    private async Task BuyArmor()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -200))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return;
        }

        await Act1EventHelpers.ChooseAndAddCardToDeck(Owner, Act2EventHelpers.GetUncommonArmorCards, "选择1张银色防具牌", 3);
        SetEventFinished(PageDescription("BUY_ARMOR_RESULT"));
    }

    private async Task BuyMount()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -200))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return;
        }

        await Act1EventHelpers.ChooseAndAddCardToDeck(Owner, Act2EventHelpers.GetUncommonMountCards, "选择1张银色坐骑牌", 3);
        SetEventFinished(PageDescription("BUY_MOUNT_RESULT"));
    }

    private Task Leave()
    {
        SetEventFinished(PageDescription("LEAVE_RESULT"));
        return Task.CompletedTask;
    }
}

[RegisterSharedEvent]
public sealed class TaoBingEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, LetThemGo, InitialOptionKey("LET_THEM_GO")),
        new EventOption(this, AcceptTheirOldThings, InitialOptionKey("ACCEPT_THEIR_OLD_THINGS")),
    ];

    private async Task LetThemGo()
    {
        await Act1EventHelpers.ChooseAndRemoveCardFromDeck(Owner, "选择1张牌移除");
        SetEventFinished(PageDescription("LET_THEM_GO_RESULT"));
    }

    private async Task AcceptTheirOldThings()
    {
        await Act1EventHelpers.ChooseAndAddUpgradedCardToDeck(Owner, Act2EventHelpers.GetSoldierCardsWithoutCurses, "选择1张升级过的牌", 3);
        SetEventFinished(PageDescription("ACCEPT_THEIR_OLD_THINGS_RESULT"));
    }
}

[RegisterSharedEvent]
public sealed class WenYiHengXingEvent : MyFirstEvent
{
    public override bool IsAllowed(IRunState runState) => IsRunInAct(runState, 1) && IsFirstVisit(runState);

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        new EventOption(this, EnterTheVillage, InitialOptionKey("ENTER_THE_VILLAGE")),
        new EventOption(this, GoAround, InitialOptionKey("GO_AROUND")),
    ];

    private Task EnterTheVillage()
    {
        RuntimeReflection.TryIncreaseMaxHp(Owner?.Creature, -5, alsoHeal: false);
        SetEventFinished(PageDescription("ENTER_THE_VILLAGE_RESULT"));
        return Task.CompletedTask;
    }

    private Task GoAround()
    {
        if (!RuntimeReflection.TryModifyPlayerGold(Owner, -50))
        {
            SetEventFinished(PageDescription("NOT_ENOUGH_GOLD"));
            return Task.CompletedTask;
        }

        SetEventFinished(PageDescription("GO_AROUND_RESULT"));
        return Task.CompletedTask;
    }
}

internal static class Act2EventHelpers
{
    public static List<CardModel> GetUncommonWeaponCards() => GetCardsByFilter(card => card is WeaponCard && card.Rarity == CardRarity.Uncommon);

    public static List<CardModel> GetUncommonArmorCards() => GetCardsByFilter(card => card is ArmorCard && card.Rarity == CardRarity.Uncommon);

    public static List<CardModel> GetUncommonMountCards() => GetCardsByFilter(card => card is MountCard && card.Rarity == CardRarity.Uncommon);

    public static List<CardModel> GetSoldierCardsWithoutCurses() =>
        GetCardsByFilter(card => card is not AoHuiCurse && card is not YongHengCurse && card is not LengYanCurse);

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
