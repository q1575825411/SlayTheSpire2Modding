using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class JuanTuChongLai : MyFirstCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.JuanTuChongLaiPower>(1)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.JuanTuChongLaiPower>()
    ];

    public JuanTuChongLai() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        return PowerCmd.Apply<Powers.JuanTuChongLaiPower>(Owner, DynamicVars["JuanTuChongLaiPower"].IntValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["JuanTuChongLaiPower"].UpgradeValueBy(1);
    }
}
