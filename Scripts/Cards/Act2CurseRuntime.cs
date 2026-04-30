using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib;

namespace MyFirstStS2Mod.Scripts.Cards;

internal static class Act2CurseRuntime
{
    private static IDisposable? _cardDrawnSubscription;

    public static void Initialize()
    {
        _cardDrawnSubscription ??= RitsuLibFramework.SubscribeLifecycle<CardDrawnEvent>(OnCardDrawn);
    }

    private static void OnCardDrawn(CardDrawnEvent evt)
    {
        switch (evt.Card)
        {
            case YongHengCurse { Owner: Player owner }:
                _ = MegaCrit.Sts2.Core.Commands.CreatureCmd.Damage(
                    new MegaCrit.Sts2.Core.GameActions.Multiplayer.ThrowingPlayerChoiceContext(),
                    owner.Creature,
                    3m,
                    MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable,
                    null,
                    evt.Card);
                break;
            case LengYanCurse { Owner: Player owner }:
                _ = MegaCrit.Sts2.Core.Commands.PowerCmd.Apply<StrengthPower>(owner.Creature, -1, owner.Creature, evt.Card);
                break;
        }
    }
}
