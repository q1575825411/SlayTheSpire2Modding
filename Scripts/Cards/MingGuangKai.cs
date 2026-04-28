using MegaCrit.Sts2.Core.Entities.Cards;

namespace MyFirstStS2Mod.Scripts.Cards;

public class MingGuangKai : ArmorCard
{
    public MingGuangKai() : base(2, CardRarity.Common)
    {
    }

    protected override Task OnEquipped()
    {
        return RuntimeReflection.RemoveDebuffPowers(Owner);
    }

    protected override Task OnUnequipped()
    {
        Owner.PlayerCombatState?.GainEnergy(1);
        return Task.CompletedTask;
    }
}
