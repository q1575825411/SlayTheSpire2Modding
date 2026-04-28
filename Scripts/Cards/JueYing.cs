using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MyFirstStS2Mod.Scripts.Cards;

public class JueYing : MountCard
{
    public JueYing() : base(2, CardRarity.Uncommon)
    {
    }

    protected override Task OnEquipped()
    {
        return PowerCmd.Apply<Powers.JueYingPower>(Owner, 1, Owner, this);
    }

    protected override Task OnUnequipped()
    {
        return RuntimeReflection.RemovePowersOfType(Owner, typeof(Powers.JueYingPower));
    }
}
