using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MyFirstStS2Mod.Scripts.Cards;

public class JuShuiWeiYing : MyFirstCard
{
    public JuShuiWeiYing() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [
        HoverTipFactory.FromPower<Powers.ColdPower>(),
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target!;
        var cold = RuntimeReflection.GetPower<Powers.ColdPower>(target);
        if (cold is not null)
        {
            var remaining = RuntimeReflection.IsCardUpgraded(this) ? (int)Math.Ceiling(cold.Amount / 2m) : 0;
            await PowerCmd.Remove(cold);
            if (remaining > 0)
            {
                await PowerCmd.Apply<Powers.ColdPower>(target, remaining, Owner, this);
            }
        }

        await PowerCmd.Apply<StrengthPower>(Owner, 1, Owner, this);
        await PowerCmd.Apply<DexterityPower>(Owner, 1, Owner, this);
    }

    protected override void OnUpgrade()
    {
    }
}
