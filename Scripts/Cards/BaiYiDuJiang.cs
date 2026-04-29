using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class BaiYiDuJiang : MyFirstCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.BaiYiDuJiangBurstPower>(1)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.BaiYiDuJiangLockPower>(),
        HoverTipFactory.FromPower<Powers.BaiYiDuJiangBurstPower>(),
        HoverTipFactory.FromPower<Powers.ColdPower>()
    ];

    public BaiYiDuJiang() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<Powers.BaiYiDuJiangLockPower>(Owner, 1, Owner, this);
        await PowerCmd.Apply<Powers.BaiYiDuJiangBurstPower>(Owner, DynamicVars["BaiYiDuJiangBurstPower"].IntValue, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BaiYiDuJiangBurstPower"].UpgradeValueBy(1);
    }
}
