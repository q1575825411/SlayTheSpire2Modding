using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Cards.DynamicVars;

namespace MyFirstStS2Mod.Scripts.Cards;

public class HuoMu : MyFirstCard
{
    private int _maxBlock = 12;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(12, ValueProp.Move)
    ];

    public HuoMu() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var scorch = cardPlay.Target!.Powers.OfType<Powers.ScorchPower>().FirstOrDefault();
        if (scorch is null || scorch.Amount <= 0)
        {
            return;
        }

        var block = Math.Min(_maxBlock, (int)scorch.Amount);
        await CreatureCmd.GainBlock(Owner, block, ValueProp.Move, null);
    }

    protected override void OnUpgrade()
    {
        _maxBlock = 18;
        DynamicVars.Block.UpgradeValueBy(6);
    }
}
