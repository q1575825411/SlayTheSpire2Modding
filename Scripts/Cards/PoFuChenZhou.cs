using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MyFirstStS2Mod.Scripts.Cards;

public class PoFuChenZhou : MyFirstCard
{
    private bool _isExhaust = true;

    public override IEnumerable<CardKeyword> CanonicalKeywords => _isExhaust ? [CardKeyword.Exhaust] : [];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public PoFuChenZhou() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var exhaustCount = RuntimeReflection.GetExhaustPileCount(Owner);
        return PowerCmd.Apply<StrengthPower>(Owner, exhaustCount, Owner, this);
    }

    protected override void OnUpgrade()
    {
        _isExhaust = false;
    }
}
