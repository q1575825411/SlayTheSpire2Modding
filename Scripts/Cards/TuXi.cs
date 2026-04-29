using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class TuXi : MyFirstCard
{
    private int _weakAmount = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5, ValueProp.Move),
        new PowerVar<WeakPower>(1)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<WeakPower>()
    ];

    public TuXi() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var sha = RuntimeReflection.FindFirstCardInHand<ShaCard>(Owner, this);
        if (sha is not null)
        {
            await CardCmd.Exhaust(choiceContext, sha);
        }

        await CreatureCmd.Damage(choiceContext, [cardPlay.Target!], DynamicVars.Damage.BaseValue, ValueProp.Move, Owner);
        await PowerCmd.Apply<WeakPower>(cardPlay.Target!, _weakAmount, Owner, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["WeakPower"].UpgradeValueBy(1);
        _weakAmount = 2;
    }
}
