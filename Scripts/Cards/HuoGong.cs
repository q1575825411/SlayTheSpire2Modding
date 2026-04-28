using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class HuoGong : MyFirstCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.ScorchPower>(8)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.ScorchPower>()
    ];

    public HuoGong() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.ScorchPower>(cardPlay.Target!, DynamicVars["ScorchPower"].IntValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ScorchPower"].UpgradeValueBy(4);
    }
}
