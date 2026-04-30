using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib;

namespace MyFirstStS2Mod.Scripts.Cards;

internal static class AoHuiCurseRuntime
{
    private static IDisposable? _cardDrawnSubscription;

    public static void Initialize()
    {
        _cardDrawnSubscription ??= RitsuLibFramework.SubscribeLifecycle<CardDrawnEvent>(OnCardDrawn);
    }

    private static void OnCardDrawn(CardDrawnEvent evt)
    {
        if (evt.Card is not AoHuiCurse { Owner: Player owner })
        {
            return;
        }

        _ = MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<WeakPower>(owner.Creature, 1, owner.Creature, evt.Card);
    }
}
