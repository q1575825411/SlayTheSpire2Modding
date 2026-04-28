using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MyFirstStS2Mod.Scripts.Cards;

public class BaiYinShiZi : ArmorCard
{
    public BaiYinShiZi() : base(2, CardRarity.Rare)
    {
    }

    protected override Task OnEquipped()
    {
        return PowerCmd.Apply<Powers.BaiYinShiZiPower>(Owner, 1, Owner, this);
    }

    protected override async Task OnUnequipped()
    {
        await RuntimeReflection.RemovePowersOfType(Owner, typeof(Powers.BaiYinShiZiPower));
        await PowerCmd.Apply<RegenPower>(Owner, 7, Owner, null);
    }
}
