using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class BaiYiDuJiangBurstPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner || CombatState is null)
        {
            return;
        }

        foreach (var enemy in CombatState.GetOpponentsOf(Owner).Where(creature => creature.IsAlive))
        {
            var cold = RuntimeReflection.GetPower<ColdPower>(enemy);
            if (cold is not null && cold.Amount > 0)
            {
                await CreatureCmd.Damage(choiceContext, [enemy], cold.Amount * Amount, ValueProp.Unpowered | ValueProp.Move, Owner);
            }
        }

        await RuntimeReflection.RemovePowersOfType(Owner, typeof(BaiYiDuJiangLockPower));
        await PowerCmd.Remove(this);
    }
}
