using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class ThunderSha : ShaCard
{
    private int _energyOnHit = 1;
    private int _drawAmount;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(5, MegaCrit.Sts2.Core.ValueProps.ValueProp.Move),
        new CardsVar(1)
    ];

    public ThunderSha() : base(5, CardRarity.Uncommon)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        var hpBefore = target.CurrentHp;

        await DealShaDamage(choiceContext, cardPlay, ResolveShaDamage(target));
        await ApplyCommonShaScorch(choiceContext, target);

        if (_drawAmount > 0)
        {
            await CardPileCmd.Draw(choiceContext, _drawAmount, Owner);
        }

        if (target.CurrentHp < hpBefore)
        {
            Owner.PlayerCombatState?.GainEnergy(_energyOnHit);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3);
        _drawAmount = 1;
    }
}
