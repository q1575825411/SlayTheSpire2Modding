using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class IceSha : ShaCard
{
    private int _coldAmount = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move),
        new PowerVar<Powers.ColdPower>(4)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..base.AdditionalHoverTips,
        HoverTipFactory.FromPower<Powers.ColdPower>()
    ];

    public IceSha() : base(4, CardRarity.Uncommon)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DealShaDamage(choiceContext, cardPlay, ResolveShaDamage());
        await PowerCmd.Apply<Powers.ColdPower>(cardPlay.Target!, _coldAmount, Owner, this);

        var extraScorch = BattleState.TryConsumeNextAttackExtraScorch(Owner);
        if (extraScorch > 0)
        {
            await PowerCmd.Apply<Powers.ScorchPower>(cardPlay.Target!, extraScorch, Owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["ColdPower"].UpgradeValueBy(2);
        _coldAmount = 6;
    }
}
