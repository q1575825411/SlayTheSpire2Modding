using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Keywords;

namespace MyFirstStS2Mod.Scripts.Cards;

public class Tao : MyFirstCard
{
    private const int EnergyCost = 2;
    private const CardType Type = CardType.Skill;
    private const CardRarity Rarity = CardRarity.Rare;
    private const TargetType TargetType = TargetType.None;

    private int _regenAmount = 3;
    private int _platingAmount = 3;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public Tao() : base(EnergyCost, Type, Rarity, TargetType)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<RegenPower>(Owner, _regenAmount, Owner, null);
        await PowerCmd.Apply<PlatingPower>(Owner, _platingAmount, Owner, null);
    }

    protected override void OnUpgrade()
    {
        _regenAmount = 4;
        _platingAmount = 4;
    }
}
