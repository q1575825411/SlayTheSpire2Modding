using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class ChiBi : MyFirstCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.ChiBiPower>(3)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.ChiBiPower>()
    ];

    public ChiBi() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.ChiBiPower>(Owner, DynamicVars["ChiBiPower"].IntValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ChiBiPower"].UpgradeValueBy(2);
    }
}
