using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using STS2RitsuLib.Keywords;

namespace MyFirstStS2Mod.Scripts.Cards;

public class DongFeng : MyFirstCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public DongFeng() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
    }

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CombatState is null)
        {
            return Task.CompletedTask;
        }

        return DoubleScorch();
    }

    private async Task DoubleScorch()
    {
        foreach (var enemy in CombatState!.GetOpponentsOf(Owner).Where(creature => creature.IsAlive))
        {
            var scorch = enemy.Powers.OfType<Powers.ScorchPower>().FirstOrDefault();

            if (scorch is null || scorch.Amount <= 0)
            {
                continue;
            }

            await MegaCrit.Sts2.Core.Commands.PowerCmd.ModifyAmount(
                scorch,
                scorch.Amount,
                null,
                this
            );
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}