using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class FengHuoLianYing : MyFirstCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.FengHuoLianYingPower>(2)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.FengHuoLianYingPower>()
    ];

    public FengHuoLianYing() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.FengHuoLianYingPower>(Owner, DynamicVars["FengHuoLianYingPower"].IntValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["FengHuoLianYingPower"].UpgradeValueBy(1);
    }
}
