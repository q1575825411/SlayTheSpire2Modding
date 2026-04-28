using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MyFirstStS2Mod.Scripts.Cards;

public class LieHuoGongXin : MyFirstCard
{
    private int _maxHpLoss = 12;

    public LieHuoGongXin() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        var scorch = target.Powers.OfType<Powers.ScorchPower>().FirstOrDefault();
        if (scorch is null || scorch.Amount <= 0)
        {
            return;
        }

        var hpLoss = Math.Min(_maxHpLoss, (int)Math.Floor(scorch.Amount / 2m));
        if (hpLoss <= 0)
        {
            return;
        }

        await CreatureCmd.Damage(
            choiceContext,
            [target],
            hpLoss,
            MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered | MegaCrit.Sts2.Core.ValueProps.ValueProp.Unblockable,
            null);
    }

    protected override void OnUpgrade()
    {
        _maxHpLoss = 24;
    }
}
