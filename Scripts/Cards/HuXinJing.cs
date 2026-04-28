using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace MyFirstStS2Mod.Scripts.Cards;

public class HuXinJing : ArmorCard
{
    public HuXinJing() : base(2, CardRarity.Uncommon)
    {
    }

    protected override Task OnEquipped()
    {
        return PowerCmd.Apply<Powers.HuXinJingPower>(Owner, 1, Owner, this);
    }

    protected override Task OnUnequipped()
    {
        return RuntimeReflection.RemovePowersOfType(Owner, typeof(Powers.HuXinJingPower));
    }
}
