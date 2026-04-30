using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MyFirstStS2Mod.Scripts.Cards;

public sealed class TaoYuanJieYi : MyFirstCard
{
    public TaoYuanJieYi() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        RuntimeReflection.TryAddNewCardToHand<Tao>(Owner);
        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
    }
}
