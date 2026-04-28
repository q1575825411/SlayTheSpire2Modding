using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MyFirstStS2Mod.Scripts.Cards;

public class ChuanGuoYuXi : TreasureCard
{
    public ChuanGuoYuXi() : base(2, CardRarity.Rare)
    {
    }

    protected override Task OnEquipped()
    {
        return PowerCmd.Apply<Powers.ChuanGuoYuXiPower>(Owner, 1, Owner, this);
    }

    protected override Task OnUnequipped()
    {
        return RuntimeReflection.RemovePowersOfType(Owner, typeof(Powers.ChuanGuoYuXiPower));
    }
}
