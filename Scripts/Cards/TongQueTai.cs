using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace MyFirstStS2Mod.Scripts.Cards;

public class TongQueTai : TreasureCard
{
    public TongQueTai() : base(2, CardRarity.Uncommon)
    {
    }

    protected override async Task OnEquipped()
    {
        await PowerCmd.Apply<StrengthPower>(Owner, 1, Owner, null);
        await PowerCmd.Apply<DexterityPower>(Owner, 1, Owner, null);
        await PowerCmd.Apply<RegenPower>(Owner, 1, Owner, null);
        await PowerCmd.Apply<PlatingPower>(Owner, 1, Owner, null);
    }
}
