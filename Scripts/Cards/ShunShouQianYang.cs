using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;
using STS2RitsuLib.Keywords;

namespace MyFirstStS2Mod.Scripts.Cards;

public class ShunShouQianYang : MyFirstCard
{
    private bool _drawOnUpgrade;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];

    public ShunShouQianYang() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        foreach (var power in RuntimeReflection.GetPositivePowers(target))
        {
            try
            {
                await RuntimeReflection.ApplyPowerByType(
                    power.GetType(),
                    Owner,
                    RuntimeReflection.GetPowerAmount(power),
                    Owner,
                    this);
            }
            catch
            {
                // Best-effort copy for modded / special-case buffs without a compatible Apply path.
            }
        }

        if (_drawOnUpgrade)
        {
            await CardPileCmd.Draw(choiceContext, 1, Owner);
        }
    }

    protected override void OnUpgrade()
    {
        _drawOnUpgrade = true;
    }
}
