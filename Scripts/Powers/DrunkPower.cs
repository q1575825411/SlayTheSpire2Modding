using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class DrunkPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner)
        {
            return;
        }

        BattleState.ActivateDrunkPenalty(Owner);
        await Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != Owner || !BattleState.IsDrunkPenaltyActive(Owner))
        {
            return;
        }

        if (result.UnblockedDamage <= 0 || dealer?.Side == Owner.Side)
        {
            return;
        }

        await CreatureCmd.Damage(
            choiceContext,
            [target],
            1m,
            ValueProp.Unpowered | ValueProp.Unblockable,
            dealer ?? Owner);
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || !BattleState.IsDrunkPenaltyActive(Owner))
        {
            return;
        }

        BattleState.ClearDrunk(Owner);
        await PowerCmd.Remove(this);
    }
}
