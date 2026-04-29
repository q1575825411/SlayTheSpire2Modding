using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class XuShiDaiFa : MyFirstCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.XuShiDaiFaPower>(6)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.XuShiDaiFaPower>()
    ];

    public XuShiDaiFa() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return PowerCmd.Apply<Powers.XuShiDaiFaPower>(Owner, DynamicVars["XuShiDaiFaPower"].IntValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["XuShiDaiFaPower"].UpgradeValueBy(3);
    }
}
