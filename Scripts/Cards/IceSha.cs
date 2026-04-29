using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MyFirstStS2Mod.Scripts.Relics;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class IceSha : ShaCard
{
    private int _coldAmount = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move),
        new PowerVar<Powers.ColdPower>(4)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        ..base.AdditionalHoverTips,
        HoverTipFactory.FromPower<Powers.ColdPower>()
    ];

    public IceSha() : base(5, CardRarity.Uncommon)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var result = await DealShaDamage(choiceContext, cardPlay, ResolveShaDamage(cardPlay.Target!));
        await PowerCmd.Apply<Powers.ColdPower>(cardPlay.Target!, OtherRelicChecks.ModifyColdAmount(Owner, cardPlay.Target, _coldAmount), Owner, this);
        await ApplyCommonShaScorch(choiceContext, result.Target);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        DynamicVars["ColdPower"].UpgradeValueBy(4);
        _coldAmount = 8;
    }
}
