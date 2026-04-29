using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class XiangFan : MyFirstCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.XiangFanPower>(3)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.XiangFanPower>(),
        HoverTipFactory.FromPower<Powers.ColdPower>()
    ];

    public XiangFan() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return PowerCmd.Apply<Powers.XiangFanPower>(Owner, DynamicVars["XiangFanPower"].IntValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["XiangFanPower"].UpgradeValueBy(3);
    }
}
