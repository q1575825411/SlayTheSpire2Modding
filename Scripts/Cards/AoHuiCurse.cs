using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MyFirstStS2Mod.Scripts.Cards;

public sealed class AoHuiCurse : MyFirstCard
{
    private static readonly CardType ResolvedCurseType = ResolveCurseCardType();

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
        CardKeyword.Unplayable
    ];

    public AoHuiCurse()
        : base(0, ResolvedCurseType, CardRarity.Token, TargetType.None, shouldShowInCardLibrary: false)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
    }

    private static CardType ResolveCurseCardType()
    {
        return Enum.TryParse<CardType>("Curse", out var curseType)
            ? curseType
            : CardType.Skill;
    }
}
