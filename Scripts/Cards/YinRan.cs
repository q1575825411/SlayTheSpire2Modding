using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MyFirstStS2Mod.Scripts.Cards;

public class YinRan : MyFirstCard
{
    private int _extraScorchAmount = 3;

    public YinRan() : base(0, CardType.Skill, CardRarity.Common, TargetType.None)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        BattleState.SetNextAttackExtraScorch(Owner, _extraScorchAmount);
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        _extraScorchAmount = 5;
    }
}
