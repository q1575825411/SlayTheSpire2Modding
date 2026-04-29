using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class QuanJunChuJi : MyFirstCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move)
    ];

    public QuanJunChuJi() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var shaCards = RuntimeReflection.GetHandCards(Owner)
            .Where(card => card is ShaCard)
            .ToList();

        var extraDamage = 0;
        foreach (var sha in shaCards)
        {
            extraDamage += RuntimeReflection.GetCardDamageBaseValue(sha);
            await CardCmd.Exhaust(choiceContext, sha);
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + (int)Math.Floor(extraDamage * 0.5m))
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}
