using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class FengHuoLianYingPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task TriggerFromEnemyScorch(PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, null);
    }
}
