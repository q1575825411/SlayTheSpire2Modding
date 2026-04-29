using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MyFirstStS2Mod.Scripts.Relics;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class YuJin : MyFirstCard
{
    private int _drawAmount = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new PowerVar<Powers.ScorchPower>(2)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.ScorchPower>()
    ];

    public YuJin() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        if (target.Powers.OfType<Powers.ScorchPower>().Any())
        {
            await CardPileCmd.Draw(choiceContext, _drawAmount, Owner);
        }

        await PowerCmd.Apply<Powers.ScorchPower>(target, OtherRelicChecks.ModifyScorchAmount(Owner, target, DynamicVars["ScorchPower"].IntValue), Owner, this);
    }

    protected override void OnUpgrade()
    {
        _drawAmount = 2;
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
