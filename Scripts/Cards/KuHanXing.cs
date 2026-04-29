using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class KuHanXing : MyFirstCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.ColdPower>(5)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.ColdPower>(),
        HoverTipFactory.FromPower<Powers.KuHanXingPower>()
    ];

    public KuHanXing() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.ColdPower>(cardPlay.Target!, DynamicVars["ColdPower"].IntValue, Owner, this);
        await PowerCmd.Apply<Powers.KuHanXingPower>(Owner, 1, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ColdPower"].UpgradeValueBy(3);
    }
}
