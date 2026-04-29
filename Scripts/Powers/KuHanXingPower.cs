using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class KuHanXingPower : ModPowerTemplate
{
    private bool _armed;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            _armed = true;
        }

        await Task.CompletedTask;
    }

    public bool TryApplyTo(CardModel card)
    {
        if (!_armed || card.Owner != Owner || card.Type != CardType.Attack)
        {
            return false;
        }

        RuntimeReflection.TrySetCardCostForTurn(card, RuntimeReflection.GetCardBaseCost(card) + 1);
        _armed = false;
        _ = PowerCmd.Remove(this);
        return true;
    }
}
