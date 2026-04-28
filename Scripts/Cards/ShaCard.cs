using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Keywords;

namespace MyFirstStS2Mod.Scripts.Cards;

public abstract class ShaCard : MyFirstCard
{
    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.DrunkPower>()
    ];

    protected ShaCard(int damage, CardRarity rarity)
        : base(0, CardType.Attack, rarity, TargetType.AnyEnemy)
    {
        BaseDamage = damage;
    }

    protected int BaseDamage { get; private set; }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(BaseDamage, ValueProp.Move)
    ];

    protected int ResolveShaDamage()
    {
        var damage = DynamicVars.Damage.BaseValue;
        if (BattleState.TryConsumeNextShaDealsDoubleDamage(Owner))
        {
            damage *= 2;
        }

        return damage;
    }

    protected async Task DealShaDamage(PlayerChoiceContext choiceContext, CardPlay cardPlay, int damage)
    {
        await DamageCmd.Attack(damage)
            .FromCard(this)
            .Targeting(cardPlay.Target!)
            .Execute(choiceContext);
    }
}
