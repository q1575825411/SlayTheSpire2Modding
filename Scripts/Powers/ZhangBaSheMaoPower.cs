using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace MyFirstStS2Mod.Scripts.Powers;

[RegisterPower]
public class ZhangBaSheMaoPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner)
        {
            return;
        }

        if (!await RuntimeReflection.TryDiscardCards(choiceContext, Owner, 2))
        {
            return;
        }

        var target = RuntimeReflection.GetLivingOpponents(Owner).FirstOrDefault();
        if (target is null)
        {
            return;
        }

        BattleState.RecordVirtualShaUse(Owner);
        await DamageCmd.Attack(8)
            .Targeting(target)
            .Execute(choiceContext);
    }
}
