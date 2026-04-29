using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MyFirstStS2Mod.Scripts.Relics;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class ChiShouPower : ModPowerTemplate
{
    private int _triggersThisTurn;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, MegaCrit.Sts2.Core.Combat.CombatSide side)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await PowerCmd.Remove(this);
    }

    public async Task TriggerFromBlockGain(PlayerChoiceContext choiceContext)
    {
        if (_triggersThisTurn >= 3 || CombatState is null)
        {
            return;
        }

        var enemies = CombatState.GetOpponentsOf(Owner).Where(creature => creature.IsAlive).ToList();
        if (enemies.Count == 0)
        {
            return;
        }

        var enemy = enemies[Random.Shared.Next(enemies.Count)];
        _triggersThisTurn++;
        await PowerCmd.Apply<ScorchPower>(enemy, OtherRelicChecks.ModifyScorchAmount(Owner, enemy, (int)Amount), Owner, null);
    }
}
