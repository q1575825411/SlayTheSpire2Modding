using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class ThunderSha : ShaCard
{
    private bool _drawOnUpgrade;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move),
        new CardsVar(1)
    ];

    public ThunderSha() : base(4, CardRarity.Uncommon)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        var hpBefore = target.CurrentHp;

        await DealShaDamage(choiceContext, cardPlay, ResolveShaDamage(target));
        await ApplyCommonShaScorch(choiceContext, target);

        if (_drawOnUpgrade)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }

        if (target.CurrentHp < hpBefore)
        {
            Owner.PlayerCombatState?.GainEnergy(1);
        }
    }

    protected override void OnUpgrade()
    {
        _drawOnUpgrade = true;
    }
}
