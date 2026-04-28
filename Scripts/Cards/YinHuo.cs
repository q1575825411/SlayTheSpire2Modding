using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class YinHuo : MyFirstCard
{
    private int _singleTargetScorch = 2;
    private int _allTargetScorch = 3;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<Powers.ScorchPower>(2)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.ScorchPower>()
    ];

    public YinHuo() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        if (target.Powers.OfType<Powers.ScorchPower>().Any() && CombatState is not null)
        {
            foreach (var enemy in CombatState.GetOpponentsOf(Owner).Where(creature => creature.IsAlive))
            {
                await PowerCmd.Apply<Powers.ScorchPower>(enemy, _allTargetScorch, Owner, this);
            }

            return;
        }

        await PowerCmd.Apply<Powers.ScorchPower>(target, _singleTargetScorch, Owner, this);
    }

    protected override void OnUpgrade()
    {
        _singleTargetScorch = 3;
        _allTargetScorch = 4;
        DynamicVars["ScorchPower"].UpgradeValueBy(1);
    }
}
