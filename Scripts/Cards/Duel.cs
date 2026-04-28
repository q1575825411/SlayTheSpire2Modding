using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class Duel : MyFirstCard
{
    private int _damageAmount = 5;
    private int _weakAmount = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5, ValueProp.Unblockable | ValueProp.Move),
        new PowerVar<WeakPower>(1)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<WeakPower>()
    ];

    public Duel() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hasShaInHand = RuntimeReflection.GetHandCards(Owner).Any(card => card is ShaCard);
        if (hasShaInHand)
        {
            await CreatureCmd.Damage(
                choiceContext,
                [cardPlay.Target!],
                _damageAmount,
                ValueProp.Unblockable | ValueProp.Move,
                Owner);
            return;
        }

        await PowerCmd.Apply<WeakPower>(cardPlay.Target!, _weakAmount, Owner, this);
    }

    protected override void OnUpgrade()
    {
        _damageAmount = 7;
        _weakAmount = 2;
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["WeakPower"].UpgradeValueBy(1);
    }
}
