using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MyFirstStS2Mod.Scripts.Cards;

public class BaGuaZhen : ArmorCard
{
    public BaGuaZhen() : base(2, CardRarity.Common)
    {
    }

    protected override Task OnEquipped()
    {
        return PowerCmd.Apply<Powers.BaGuaZhenPower>(Owner, 1, Owner, this);
    }

    protected override Task OnUnequipped()
    {
        return RuntimeReflection.RemovePowersOfType(Owner, typeof(Powers.BaGuaZhenPower));
    }
}
