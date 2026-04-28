using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class WanJianQiFa : MyFirstCard
{
    private int _shanBonusPerCard = 2;

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(2, ValueProp.Move)
    ];

    public WanJianQiFa() : base(0, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null)
        {
            return;
        }

        var hitCount = ResolveEnergyXValue() + 1;
        var shanCount = RuntimeReflection.GetHandCards(Owner).Count(card => card is Shan);
        var damagePerHit = DynamicVars.Damage.BaseValue + shanCount * _shanBonusPerCard;

        await DamageCmd.Attack(damagePerHit)
            .WithHitCount(hitCount)
            .FromCard(this)
            .TargetingAllOpponents(CombatState)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        _shanBonusPerCard = 3;
    }
}
