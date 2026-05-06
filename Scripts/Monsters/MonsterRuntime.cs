using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;
using MyFirstStS2Mod.Scripts.Powers;
using STS2RitsuLib;

namespace MyFirstStS2Mod.Scripts.Monsters;

internal static class MonsterRuntime
{
    private sealed class HuangMenState
    {
        public int StolenGold { get; set; }
        public bool Escaped { get; set; }
        public bool Refunded { get; set; }
    }

    private static readonly Dictionary<Creature, HuangMenState> HuangMenStates = [];
    private static IDisposable? _cardPlayedSubscription;
    private static IDisposable? _creatureDiedSubscription;
    private static IDisposable? _combatVictorySubscription;

    public static void Initialize()
    {
        _cardPlayedSubscription ??= RitsuLibFramework.SubscribeLifecycle<CardPlayedEvent>(OnCardPlayed);
        _creatureDiedSubscription ??= RitsuLibFramework.SubscribeLifecycle<CreatureDiedEvent>(OnCreatureDied);
        _combatVictorySubscription ??= RitsuLibFramework.SubscribeLifecycle<CombatVictoryEvent>(OnCombatVictory);
    }

    public static void RecordHuangMenStolenGold(Creature creature, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        GetOrCreateHuangMenState(creature).StolenGold += amount;
    }

    public static void MarkHuangMenEscaped(Creature creature)
    {
        GetOrCreateHuangMenState(creature).Escaped = true;
    }

    private static void OnCardPlayed(CardPlayedEvent evt)
    {
        if (evt.CardPlay.Card is not CardModel { Owner: Player owner } card)
        {
            return;
        }

        _ = HandleCardPlayed(card, owner);
    }

    private static async Task HandleCardPlayed(CardModel card, Player owner)
    {
        try
        {
            if (card.Type != CardType.Attack)
            {
                foreach (var qingYi in RuntimeReflection.GetLivingOpponents(owner)
                             .Where(creature => RuntimeReflection.GetCreatureModel(creature) is QingYiKuiShou))
                {
                    if (RuntimeReflection.GetPower<QingTanPower>(qingYi) is { } qingTan)
                    {
                        await qingTan.RegisterCardPlayed();
                    }
                }

                return;
            }

            foreach (var enemy in RuntimeReflection.GetLivingOpponents(owner))
            {
                if (RuntimeReflection.GetCreatureModel(enemy) is JinWuWei
                    && RuntimeReflection.GetPower<JinWuWeiJieYanPower>(enemy) is not null)
                {
                    await PowerCmd.Apply<StrengthPower>(enemy, 2, enemy, null);
                    await PowerCmd.Apply<DexterityPower>(enemy, 1, enemy, null);
                }

                if (RuntimeReflection.GetCreatureModel(enemy) is QingYiKuiShou
                    && RuntimeReflection.GetPower<QingTanPower>(enemy) is { } qingTan)
                {
                    await qingTan.RegisterCardPlayed();
                }
            }
        }
        catch (Exception ex)
        {
            Entry.Logger.LogError($"MonsterRuntime.HandleCardPlayed failed: {ex}");
        }
    }

    private static void OnCreatureDied(CreatureDiedEvent evt)
    {
        if (evt.Creature is not Creature creature || !IsHuangMenVariant(RuntimeReflection.GetCreatureModel(creature)))
        {
            return;
        }

        RefundHuangMenGold(creature, RuntimeReflection.GetRunState(evt.CombatState));
    }

    private static void OnCombatVictory(CombatVictoryEvent evt)
    {
        try
        {
            foreach (var creature in RuntimeReflection.GetCombatCreatures(evt.CombatState)
                         .Where(creature => IsHuangMenVariant(RuntimeReflection.GetCreatureModel(creature))))
            {
                RefundHuangMenGold(creature, evt.RunState);
            }
        }
        finally
        {
            var combatState = evt.CombatState;
            foreach (var creature in HuangMenStates.Keys
                         .Where(creature => combatState is null || RuntimeReflection.GetCombatState(creature) == combatState)
                         .ToList())
            {
                HuangMenStates.Remove(creature);
            }
        }
    }

    private static void RefundHuangMenGold(Creature creature, object? ownerOrRunState)
    {
        if (!HuangMenStates.TryGetValue(creature, out var state) || state.Refunded || state.Escaped || state.StolenGold <= 0)
        {
            return;
        }

        RuntimeReflection.TryModifyPlayerGold(ownerOrRunState, state.StolenGold);
        state.Refunded = true;
    }

    private static HuangMenState GetOrCreateHuangMenState(Creature creature)
    {
        if (!HuangMenStates.TryGetValue(creature, out var state))
        {
            state = new HuangMenState();
            HuangMenStates[creature] = state;
        }

        return state;
    }

    private static bool IsHuangMenVariant(object? model)
    {
        return model is HuangMenXiaoGuiLeft or HuangMenXiaoGuiMiddle or HuangMenXiaoGuiRight;
    }
}
