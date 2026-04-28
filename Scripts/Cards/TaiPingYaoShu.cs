using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MyFirstStS2Mod.Scripts.Cards;

public class TaiPingYaoShu : TreasureCard
{
    public TaiPingYaoShu() : base(2, CardRarity.Common)
    {
    }

    protected override Task OnEquipped()
    {
        return PowerCmd.Apply<Powers.TaiPingYaoShuPower>(Owner, 1, Owner, this);
    }

    protected override Task OnUnequipped()
    {
        return RuntimeReflection.RemovePowersOfType(Owner, typeof(Powers.TaiPingYaoShuPower));
    }
}
