using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MyFirstStS2Mod.Scripts.Cards;

public class MuNiuLiuMa : TreasureCard
{
    public MuNiuLiuMa() : base(2, CardRarity.Uncommon)
    {
    }

    protected override Task OnEquipped()
    {
        return PowerCmd.Apply<Powers.MuNiuLiuMaPower>(Owner, 1, Owner, this);
    }

    protected override async Task OnUnequipped()
    {
        await RuntimeReflection.RemovePowersOfType(Owner, typeof(Powers.MuNiuLiuMaPower));
        await RuntimeReflection.DiscardAllHandCards(new ThrowingPlayerChoiceContext(), Owner);
    }
}
