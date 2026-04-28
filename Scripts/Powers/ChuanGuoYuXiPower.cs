using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MyFirstStS2Mod.Scripts.Equipment;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class ChuanGuoYuXiPower : ModPowerTemplate
{
    private int _turnsActive;
    private int _freeCardsThisTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public bool HasFreeCardAvailable()
    {
        return _freeCardsThisTurn < 2;
    }

    public void ConsumeFreeCard()
    {
        if (_freeCardsThisTurn < 2)
        {
            _freeCardsThisTurn++;
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner)
        {
            return;
        }

        _turnsActive++;
        _freeCardsThisTurn = 0;
        await CardPileCmd.Draw(choiceContext, 1, Owner);

        if (_turnsActive >= 2)
        {
            await EquipmentState.UnequipSlot(Owner, EquipmentSlotType.Treasure);
            await PowerCmd.Apply<WeakPower>(Owner, 3, Owner, null);
            await PowerCmd.Apply<VulnerablePower>(Owner, 3, Owner, null);
        }
    }
}
