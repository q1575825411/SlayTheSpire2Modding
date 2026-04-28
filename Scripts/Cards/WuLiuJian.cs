using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace MyFirstStS2Mod.Scripts.Cards;

public class WuLiuJian : WeaponCard
{
    public WuLiuJian() : base(2, CardRarity.Common)
    {
    }

    protected override Task OnEquipped()
    {
        return PowerCmd.Apply<Powers.WuLiuJianPower>(Owner, 1, Owner, this);
    }

    protected override Task OnUnequipped()
    {
        return RuntimeReflection.RemovePowersOfType(Owner, typeof(Powers.WuLiuJianPower));
    }
}
