using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MyFirstStS2Mod.Scripts.Cards;

public class ChiTu : MountCard
{
    public ChiTu() : base(2, CardRarity.Uncommon)
    {
    }

    protected override Task OnEquipped()
    {
        return PowerCmd.Apply<Powers.ChiTuPower>(Owner, 1, Owner, this);
    }

    protected override Task OnUnequipped()
    {
        return RuntimeReflection.RemovePowersOfType(Owner, typeof(Powers.ChiTuPower));
    }
}
