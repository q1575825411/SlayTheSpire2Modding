using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MyFirstStS2Mod.Scripts.Cards;

public class Sha : ShaCard
{
    public Sha() : base(8, CardRarity.Common)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DealShaDamage(choiceContext, cardPlay, ResolveShaDamage());

        var extraScorch = BattleState.TryConsumeNextAttackExtraScorch(Owner);
        if (extraScorch > 0)
        {
            await PowerCmd.Apply<Powers.ScorchPower>(cardPlay.Target!, extraScorch, Owner, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8);
    }
}
