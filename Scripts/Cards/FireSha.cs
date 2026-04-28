using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class FireSha : ShaCard
{
    private int _scorchAmount = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(2, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move),
        new PowerVar<Powers.ScorchPower>(4)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..base.AdditionalHoverTips,
        HoverTipFactory.FromPower<Powers.ScorchPower>()
    ];

    public FireSha() : base(2, CardRarity.Uncommon)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DealShaDamage(choiceContext, cardPlay, ResolveShaDamage());
        await PowerCmd.Apply<Powers.ScorchPower>(cardPlay.Target!, _scorchAmount, Owner, this);

        var extraScorch = BattleState.TryConsumeNextAttackExtraScorch(Owner);
        if (extraScorch > 0)
        {
            await PowerCmd.Apply<Powers.ScorchPower>(cardPlay.Target!, extraScorch, Owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["ScorchPower"].UpgradeValueBy(4);
        _scorchAmount = 8;
    }
}
