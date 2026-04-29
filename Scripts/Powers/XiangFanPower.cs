using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MyFirstStS2Mod.Scripts.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class XiangFanPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || CombatState is null)
        {
            return;
        }

        foreach (var enemy in CombatState.GetOpponentsOf(Owner).Where(creature => creature.IsAlive))
        {
            await PowerCmd.Apply<ColdPower>(enemy, OtherRelicChecks.ModifyColdAmount(Owner, enemy, (int)Amount), Owner, null);
        }
    }
}
