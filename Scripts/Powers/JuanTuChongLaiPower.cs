using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class JuanTuChongLaiPower : ModPowerTemplate
{
    private bool _triggeredThisTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature == Owner)
        {
            _triggeredThisTurn = false;
        }

        await Task.CompletedTask;
    }

    public void NotifyCardExhausted(CardModel card)
    {
        if (_triggeredThisTurn || card.Owner != Owner)
        {
            return;
        }

        _triggeredThisTurn = true;
        RuntimeReflection.TryAddCardCopyToHand(card, Owner, exhaust: true, retain: Amount >= 2);
    }
}
