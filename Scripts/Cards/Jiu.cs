using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using STS2RitsuLib.Keywords;

namespace MyFirstStS2Mod.Scripts.Cards;

public class Jiu : MyFirstCard
{
    private const int EnergyCost = 1;
    private const CardType Type = CardType.Skill;
    private const CardRarity Rarity = CardRarity.Uncommon;
    private const TargetType TargetType = TargetType.None;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.DrunkPower>()
    ];

    public Jiu() : base(EnergyCost, Type, Rarity, TargetType)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        BattleState.SetNextShaDealsDoubleDamage(Owner);
        BattleState.ApplyDrunk(Owner);
        await PowerCmd.Apply<Powers.DrunkPower>(Owner, 1, Owner, null);
    }

    protected override void OnUpgrade()
    {
        Cost = 0;
    }
}
