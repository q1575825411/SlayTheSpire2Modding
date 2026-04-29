using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using MyFirstStS2Mod.Scripts.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class ScorchPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Amount <= 0)
        {
            return;
        }

        var damageAmount = Amount;
        if (CombatState is not null)
        {
            foreach (var opponent in CombatState.GetOpponentsOf(Owner))
            {
                if (OtherRelicChecks.HasRelic<FireOilRelic>(opponent))
                {
                    damageAmount += 1;
                    break;
                }
            }
        }

        await CreatureCmd.Damage(
            choiceContext,
            [Owner],
            damageAmount,
            ValueProp.Unpowered | ValueProp.Unblockable,
            null);

        if (CombatState is not null)
        {
            foreach (var ally in CombatState.GetOpponentsOf(Owner).Where(creature => creature.IsAlive))
            {
                foreach (var power in ally.Powers.OfType<FengHuoLianYingPower>())
                {
                    await power.TriggerFromEnemyScorch(choiceContext);
                }
            }
        }

        var retainedAmount = Math.Floor(Amount / 2m);
        var delta = retainedAmount - Amount;
        await PowerCmd.ModifyAmount(this, delta, null, null);
    }
}
