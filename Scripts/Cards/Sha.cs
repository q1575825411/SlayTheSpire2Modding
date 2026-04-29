using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MyFirstStS2Mod.Scripts.Cards;

public class Sha : ShaCard
{
    public Sha() : base(11, CardRarity.Common)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var result = await DealShaDamage(choiceContext, cardPlay, ResolveShaDamage(cardPlay.Target!));
        await ApplyCommonShaScorch(choiceContext, result.Target);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4);
    }
}
