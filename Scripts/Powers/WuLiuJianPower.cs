using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class WuLiuJianPower : ModPowerTemplate
{
    private bool _pendingBonus;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public void NotifyShaPlayed()
    {
        _pendingBonus = true;
    }

    public override decimal ModifyDamageAdditive(
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!_pendingBonus || dealer != Owner || cardSource is not { Type: CardType.Attack } || cardSource is Cards.ShaCard)
        {
            return 0m;
        }

        return 6m;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!_pendingBonus || dealer != Owner || cardSource is not { Type: CardType.Attack } || cardSource is Cards.ShaCard)
        {
            return;
        }

        _pendingBonus = false;
        await Task.CompletedTask;
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        if (_pendingBonus)
        {
            _pendingBonus = false;
            await CreatureCmd.GainBlock(Owner, 4m, ValueProp.Move, null);
        }
    }
}
